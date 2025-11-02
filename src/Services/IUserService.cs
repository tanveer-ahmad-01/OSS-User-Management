using UserManagement.DTOs;
using UserManagement.Models;

namespace UserManagement.Services;

public interface IUserService
{
    Task<UserResponse> CreateUserAsync(CreateUserRequest request, string? createdBy);
    Task<UserResponse> UpdateUserAsync(Guid userId, UpdateUserRequest request, string? updatedBy);
    Task<bool> DeleteUserAsync(Guid userId, string? deletedBy);
    Task<UserResponse> GetUserByIdAsync(Guid userId);
    Task<UserListResponse> GetUsersAsync(PaginationRequest request);
    Task<UserResponse> GetUserByEmailAsync(string email);
    Task<bool> AssignRolesToUserAsync(Guid userId, List<Guid> roleIds, string? assignedBy);
    Task<bool> RevokeRolesFromUserAsync(Guid userId, List<Guid> roleIds, string? revokedBy);
}

