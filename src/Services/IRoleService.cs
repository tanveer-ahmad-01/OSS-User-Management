using UserManagement.DTOs;

namespace UserManagement.Services;

public interface IRoleService
{
    Task<RoleResponse> CreateRoleAsync(CreateRoleRequest request, string? createdBy);
    Task<RoleResponse> UpdateRoleAsync(Guid roleId, UpdateRoleRequest request, string? updatedBy);
    Task<bool> DeleteRoleAsync(Guid roleId, string? deletedBy);
    Task<RoleResponse> GetRoleByIdAsync(Guid roleId);
    Task<List<RoleResponse>> GetRolesAsync(string? projectId);
    Task<bool> AssignPermissionsToRoleAsync(Guid roleId, List<Guid> permissionIds, string? grantedBy);
    Task<bool> RevokePermissionsFromRoleAsync(Guid roleId, List<Guid> permissionIds, string? revokedBy);
}

