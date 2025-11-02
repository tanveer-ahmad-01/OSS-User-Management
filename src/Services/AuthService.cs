using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UserManagement.Configurations;
using UserManagement.Data;
using UserManagement.DTOs;
using UserManagement.Models;
using BCrypt.Net;

namespace UserManagement.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;
    private readonly IAuditService _auditService;
    private readonly IUserService _userService;

    public AuthService(
        ApplicationDbContext context,
        IJwtService jwtService,
        IMapper mapper,
        IAuditService auditService,
        IUserService userService)
    {
        _context = context;
        _jwtService = jwtService;
        _mapper = mapper;
        _auditService = auditService;
        _userService = userService;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, string? ipAddress, string? userAgent)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == request.EmailOrUsername || u.Username == request.EmailOrUsername);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            await _auditService.LogActivityAsync(
                AuditAction.LoginFailed,
                null,
                null,
                "User",
                $"Failed login attempt: {request.EmailOrUsername}",
                ipAddress,
                userAgent
            );
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        if (user.Status != UserStatus.Active)
        {
            throw new UnauthorizedAccessException("Account is not active");
        }

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Generate tokens
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Save refresh token
        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IpAddress = ipAddress
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        // Log activity
        await _auditService.LogActivityAsync(
            AuditAction.LoginSuccess,
            user.Id,
            user.Id,
            "User",
            "Successful login",
            ipAddress,
            userAgent
        );

        var userResponse = await _userService.GetUserByIdAsync(user.Id);
        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            User = userResponse
        };
    }

    public async Task<UserResponse> RegisterAsync(RegisterRequest request, string? ipAddress)
    {
        // Check if user exists
        if (await _context.Users.AnyAsync(u => u.Email == request.Email || u.Username == request.Username))
        {
            throw new InvalidOperationException("User already exists");
        }

        // Create user
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            ProjectId = request.ProjectId,
            Status = UserStatus.Active
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Log activity
        await _auditService.LogActivityAsync(
            AuditAction.UserCreated,
            user.Id,
            user.Id,
            "User",
            $"User registered: {user.Email}",
            ipAddress,
            null
        );

        return await _userService.GetUserByIdAsync(user.Id);
    }

    public async Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request, string? ipAddress)
    {
        var tokenEntity = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

        if (tokenEntity == null || !tokenEntity.IsActive)
        {
            throw new UnauthorizedAccessException("Invalid refresh token");
        }

        var user = tokenEntity.User;

        // Revoke old token
        tokenEntity.RevokedAt = DateTime.UtcNow;
        tokenEntity.ReplacedByToken = _jwtService.GenerateRefreshToken();

        // Generate new tokens
        var newAccessToken = _jwtService.GenerateAccessToken(user);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        var newTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IpAddress = ipAddress
        };

        _context.RefreshTokens.Add(newTokenEntity);
        await _context.SaveChangesAsync();

        return new RefreshTokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Current password is incorrect");
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _auditService.LogActivityAsync(
            AuditAction.PasswordChanged,
            userId,
            userId,
            "User",
            "Password changed",
            null,
            null
        );

        return true;
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        var tokenEntity = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (tokenEntity == null)
        {
            return false;
        }

        tokenEntity.RevokedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RevokeAllUserTokensAsync(Guid userId)
    {
        var tokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.RevokedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public Task<bool> ValidateTokenAsync(string token)
    {
        return Task.FromResult(_jwtService.ValidateToken(token));
    }
}

