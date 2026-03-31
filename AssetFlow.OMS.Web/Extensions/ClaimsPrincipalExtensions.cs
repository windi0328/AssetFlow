using System.Security.Claims;

namespace AssetFlow.OMS.Web.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal principal)
    {
        string? value = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out int userId) ? userId : 0;
    }

    public static string GetUserName(this ClaimsPrincipal principal)
    {
        return principal.Identity?.Name ?? "system";
    }
}
