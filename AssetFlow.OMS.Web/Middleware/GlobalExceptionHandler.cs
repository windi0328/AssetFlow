using AssetFlow.OMS.Web.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AssetFlow.OMS.Web.Middleware;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IProblemDetailsService _problemDetailsService;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IProblemDetailsService problemDetailsService)
    {
        _logger = logger;
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        int statusCode = exception is AppException appException
            ? appException.StatusCode
            : StatusCodes.Status500InternalServerError;

        if (statusCode >= 500)
        {
            _logger.LogError(exception, "Unhandled exception occurred while processing request {Path}.", httpContext.Request.Path);
        }
        else
        {
            _logger.LogWarning(exception, "Request failed for {Path}: {Message}", httpContext.Request.Path, exception.Message);
        }

        ProblemDetails problemDetails = new()
        {
            Title = statusCode >= 500 ? "Server error" : "Request failed",
            Detail = exception.Message,
            Status = statusCode,
            Instance = httpContext.Request.Path
        };

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });
    }
}
