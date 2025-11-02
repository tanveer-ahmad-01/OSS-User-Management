using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.DTOs;
using UserManagement.Models;

namespace UserManagement.Services;

public class RoleService : IRoleService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IAuditService _auditService;

    public RoleService(ApplicationDbContext context, IMapper mapper, IAuditService auditService)
    {
        _context = context;
        _mapper = mapper;
        _auditService = auditService;
    }

    public async Task<RoleResponse> CreateRoleAsync(CreateRoleRequest request, string? createdBy)
    {
        // Check if role exists
        if (await _context.Roles.AnyAsync(r => r.Name == request.Name && r.ProjectId == request.ProjectId))
        {
            throw new InvalidOperationException("Role already exists");
        }

        var role = new Role
        {
            Name = request.Name,
            Description = request.Description,
            Priority = request.Priority,
            ProjectId = request.ProjectId,
            CreatedBy = createdBy
        };

        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        // Assign permissions if provided
        if (request.PermissionIds != null && request.PermissionIds.Any())
        {
            await AssignPermissionsToRoleAsync(role.Id, request.PermissionIds, createdBy);
        }

        await _auditService.LogActivityAsync(
            AuditAction.RoleAssigned,
            null,
            role.Id,
            "Role",
            $"Role created: {role.Name}",
            null,
            null
        );

        return await GetRoleByIdAsync(role.Id);
    }

    public async Task<RoleResponse> UpdateRoleAsync(Guid roleId, UpdateRoleRequest request, string? updatedBy)
    {
        var role = await _context.Roles.FindAsync(roleId);
        if (role == null)
        {
            throw new KeyNotFoundException("Role not found");
        }

        if (!string.IsNullOrEmpty(request.Name))
            role.Name = request.Name;

        if (!string.IsNullOrEmpty(request.Description))
            role.Description = request.Description;

        if (request.Priority.HasValue)
            role.Priority = request.Priority.Value;

        role.UpdatedBy = updatedBy;
        role.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetRoleByIdAsync(roleId);
    }

    public async Task<bool> DeleteRoleAsync(Guid roleId, string? deletedBy)
    {
        var role = await _context.Roles.FindAsync(roleId);
        if (role == null)
        {
            throw new KeyNotFoundException("Role not found");
        }

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<RoleResponse> GetRoleByIdAsync(Guid roleId)
    {
        var role = await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                    .ThenInclude(p => p.Feature)
            .FirstOrDefaultAsync(r => r.Id == roleId);

        if (role == null)
        {
            throw new KeyNotFoundException("Role not found");
        }

        var roleResponse = _mapper.Map<RoleResponse>(role);
        roleResponse.Permissions = role.RolePermissions.Select(rp => new PermissionResponse
        {
            Id = rp.Permission.Id,
            FeatureId = rp.Permission.FeatureId,
            Type = rp.Permission.Type.ToString(),
            Description = rp.Permission.Description,
            Feature = new FeatureResponse
            {
                Id = rp.Permission.Feature.Id,
                Name = rp.Permission.Feature.Name,
                Description = rp.Permission.Feature.Description,
                Code = rp.Permission.Feature.Code,
                ModuleId = rp.Permission.Feature.ModuleId,
                IsActive = rp.Permission.Feature.IsActive,
                CreatedAt = rp.Permission.Feature.CreatedAt,
                ProjectId = rp.Permission.Feature.ProjectId
            }
        }).ToList();

        return roleResponse;
    }

    public async Task<List<RoleResponse>> GetRolesAsync(string? projectId)
    {
        var query = _context.Roles.AsQueryable();

        if (!string.IsNullOrEmpty(projectId))
        {
            query = query.Where(r => r.ProjectId == projectId);
        }

        var roles = await query.ToListAsync();
        return _mapper.Map<List<RoleResponse>>(roles);
    }

    public async Task<bool> AssignPermissionsToRoleAsync(Guid roleId, List<Guid> permissionIds, string? grantedBy)
    {
        var role = await _context.Roles.FindAsync(roleId);
        if (role == null)
        {
            throw new KeyNotFoundException("Role not found");
        }

        foreach (var permissionId in permissionIds)
        {
            var permission = await _context.Permissions.FindAsync(permissionId);
            if (permission == null)
            {
                continue;
            }

            if (!await _context.RolePermissions.AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId))
            {
                _context.RolePermissions.Add(new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permissionId,
                    GrantedBy = grantedBy
                });

                await _auditService.LogActivityAsync(
                    AuditAction.PermissionGranted,
                    null,
                    permissionId,
                    "Permission",
                    $"Permission granted to role: {role.Name}",
                    null,
                    null
                );
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RevokePermissionsFromRoleAsync(Guid roleId, List<Guid> permissionIds, string? revokedBy)
    {
        var rolePermissions = await _context.RolePermissions
            .Where(rp => rp.RoleId == roleId && permissionIds.Contains(rp.PermissionId))
            .ToListAsync();

        _context.RolePermissions.RemoveRange(rolePermissions);
        await _context.SaveChangesAsync();

        return true;
    }
}

