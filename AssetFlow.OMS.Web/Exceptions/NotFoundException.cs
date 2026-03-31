namespace AssetFlow.OMS.Web.Exceptions;

public sealed class NotFoundException : AppException
{
    public NotFoundException(string message) : base(message, StatusCodes.Status404NotFound)
    {
    }
}
