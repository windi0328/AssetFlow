using AssetFlow.OMS.Web.DTOs.Auth;
using System.Security.Claims;

namespace AssetFlow.OMS.Web.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);

    Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);

    Task<AuthResponseDto> GetCurrentUserAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default);
}
