using UserManagement.Models;

namespace UserManagement.DTOs;

public class CreateUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public List<Guid>? RoleIds { get; set; }
    public string? ProjectId { get; set; }
}

public class UpdateUserRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Avatar { get; set; }
    public UserStatus? Status { get; set; }
}

public class UserResponse
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Avatar { get; set; }
    public UserStatus Status { get; set; }
    public bool EmailVerified { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ProjectId { get; set; }
    public List<RoleResponse> Roles { get; set; } = new();
}

public class UserListResponse
{
    public List<UserResponse> Users { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

