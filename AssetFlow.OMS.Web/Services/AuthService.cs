using AssetFlow.OMS.Web.DTOs.Auth;
using AssetFlow.OMS.Web.Exceptions;
using AssetFlow.OMS.Web.Models;
using AssetFlow.OMS.Web.Models.Enums;
using AssetFlow.OMS.Web.Options;
using AssetFlow.OMS.Web.Repositories.Interfaces;
using AssetFlow.OMS.Web.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AssetFlow.OMS.Web.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
    private readonly JwtOptions _jwtOptions;

    public AuthService(
        IUserRepository userRepository,
        IAuditLogRepository auditLogRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher<ApplicationUser> passwordHasher,
        IOptions<JwtOptions> jwtOptions)
    {
        _userRepository = userRepository;
        _auditLogRepository = auditLogRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
    {
        ApplicationUser? existingUser = await _userRepository.GetByUserNameAsync(request.UserName.Trim(), cancellationToken);
        if (existingUser is not null)
        {
            throw new ConflictException("The user name is already registered.");
        }

        ApplicationUser user = new()
        {
            UserName = request.UserName.Trim(),
            DisplayName = request.DisplayName.Trim(),
            Role = UserRole.User,
            CreatedAtUtc = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _auditLogRepository.AddAsync(new AuditLog
        {
            UserId = user.Id,
            UserName = user.UserName,
            Action = AuditAction.Register,
            EntityName = nameof(ApplicationUser),
            EntityId = user.UserName,
            Detail = $"User {user.UserName} registered.",
            CreatedAtUtc = DateTime.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return BuildAuthResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        ApplicationUser user = await _userRepository.GetByUserNameAsync(request.UserName.Trim(), cancellationToken)
            ?? throw new BadRequestException("Invalid user name or password.");

        PasswordVerificationResult result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            throw new BadRequestException("Invalid user name or password.");
        }

        await _auditLogRepository.AddAsync(new AuditLog
        {
            UserId = user.Id,
            UserName = user.UserName,
            Action = AuditAction.Login,
            EntityName = nameof(ApplicationUser),
            EntityId = user.Id.ToString(),
            Detail = $"User {user.UserName} logged in.",
            CreatedAtUtc = DateTime.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return BuildAuthResponse(user);
    }

    public async Task<AuthResponseDto> GetCurrentUserAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default)
    {
        string? idValue = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(idValue, out int userId))
        {
            throw new BadRequestException("Invalid authentication context.");
        }

        ApplicationUser user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException("Authenticated user could not be found.");

        return BuildAuthResponse(user);
    }

    private AuthResponseDto BuildAuthResponse(ApplicationUser user)
    {
        DateTime expiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes);
        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_jwtOptions.Key));

        List<Claim> claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.GivenName, user.DisplayName),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        ];

        SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);
        JwtSecurityToken token = new(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return new AuthResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAtUtc = expiresAtUtc,
            UserId = user.Id,
            UserName = user.UserName,
            DisplayName = user.DisplayName,
            Role = user.Role.ToString()
        };
    }
}
