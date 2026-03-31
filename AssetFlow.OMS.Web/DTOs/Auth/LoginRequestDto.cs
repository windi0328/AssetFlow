using System.ComponentModel.DataAnnotations;

namespace AssetFlow.OMS.Web.DTOs.Auth;

public sealed class LoginRequestDto
{
    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
