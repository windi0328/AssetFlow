using System.ComponentModel.DataAnnotations;

namespace AssetFlow.OMS.Web.DTOs.Auth;

public sealed class RegisterRequestDto
{
    [Required]
    [StringLength(50, MinimumLength = 4)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string DisplayName { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = string.Empty;
}
