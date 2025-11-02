using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserManagement.DTOs;
using UserManagement.Services;

namespace UserManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            var response = await _authService.LoginAsync(request, ipAddress, userAgent);

            return Ok(new ApiResponse<LoginResponse>
            {
                Success = true,
                Message = "Login successful",
                Data = response
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponse<LoginResponse>
            {
                Success = false,
                Message = ex.Message,
                Errors = new List<string> { ex.Message }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<LoginResponse>
            {
                Success = false,
                Message = "Login failed",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var response = await _authService.RegisterAsync(request, ipAddress);

            return Ok(new ApiResponse<UserResponse>
            {
                Success = true,
                Message = "Registration successful",
                Data = response
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ApiResponse<UserResponse>
            {
                Success = false,
                Message = ex.Message,
                Errors = new List<string> { ex.Message }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<UserResponse>
            {
                Success = false,
                Message = "Registration failed",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<ApiResponse<RefreshTokenResponse>>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var response = await _authService.RefreshTokenAsync(request, ipAddress);

            return Ok(new ApiResponse<RefreshTokenResponse>
            {
                Success = true,
                Message = "Token refreshed successfully",
                Data = response
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponse<RefreshTokenResponse>
            {
                Success = false,
                Message = ex.Message,
                Errors = new List<string> { ex.Message }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<RefreshTokenResponse>
            {
                Success = false,
                Message = "Token refresh failed",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _authService.ChangePasswordAsync(userId, request);

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Password changed successfully",
                Data = result
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<bool>
            {
                Success = false,
                Message = ex.Message,
                Errors = new List<string> { ex.Message }
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponse<bool>
            {
                Success = false,
                Message = ex.Message,
                Errors = new List<string> { ex.Message }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<bool>
            {
                Success = false,
                Message = "Failed to change password",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPost("revoke-token")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> RevokeToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var result = await _authService.RevokeTokenAsync(request.RefreshToken);

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Token revoked successfully",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<bool>
            {
                Success = false,
                Message = "Failed to revoke token",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPost("revoke-all-tokens")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> RevokeAllTokens()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _authService.RevokeAllUserTokensAsync(userId);

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "All tokens revoked successfully",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<bool>
            {
                Success = false,
                Message = "Failed to revoke tokens",
                Errors = new List<string> { ex.Message }
            });
        }
    }
}

