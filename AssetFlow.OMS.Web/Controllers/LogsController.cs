using AssetFlow.OMS.Web.DTOs.AuditLogs;
using AssetFlow.OMS.Web.Models.Enums;
using AssetFlow.OMS.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetFlow.OMS.Web.Controllers;

[ApiController]
[Authorize(Roles = nameof(UserRole.Admin))]
[Route("api/[controller]")]
public sealed class LogsController : ControllerBase
{
    private readonly IAuditLogService _auditLogService;

    public LogsController(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuditLogResponseDto>>> GetRecent([FromQuery] int take = 20, CancellationToken cancellationToken = default)
    {
        return Ok(await _auditLogService.GetRecentAsync(Math.Clamp(take, 1, 100), cancellationToken));
    }
}
