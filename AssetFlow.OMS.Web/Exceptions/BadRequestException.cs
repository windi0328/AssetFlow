namespace AssetFlow.OMS.Web.Exceptions;

public sealed class BadRequestException : AppException
{
    public BadRequestException(string message) : base(message, StatusCodes.Status400BadRequest)
    {
    }
}
