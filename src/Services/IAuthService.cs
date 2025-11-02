using UserManagement.DTOs;
using UserManagement.Models;

namespace UserManagement.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, string? ipAddress, string? userAgent);
    Task<UserResponse> RegisterAsync(RegisterRequest request, string? ipAddress);
    Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request, string? ipAddress);
    Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    Task<bool> RevokeTokenAsync(string refreshToken);
    Task<bool> RevokeAllUserTokensAsync(Guid userId);
    Task<bool> ValidateTokenAsync(string token);
}

