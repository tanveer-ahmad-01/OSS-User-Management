using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.DTOs;
using UserManagement.Models;

namespace UserManagement.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IAuditService _auditService;

    public UserService(ApplicationDbContext context, IMapper mapper, IAuditService auditService)
    {
        _context = context;
        _mapper = mapper;
        _auditService = auditService;
    }

    public async Task<UserResponse> CreateUserAsync(CreateUserRequest request, string? createdBy)
    {
        // Check if user exists
        if (await _context.Users.AnyAsync(u => u.Email == request.Email || u.Username == request.Username))
        {
            throw new InvalidOperationException("User already exists");
        }

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            ProjectId = request.ProjectId,
            Status = UserStatus.Active,
            CreatedBy = createdBy
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Assign roles if provided
        if (request.RoleIds != null && request.RoleIds.Any())
        {
            await AssignRolesToUserAsync(user.Id, request.RoleIds, createdBy);
        }

        await _auditService.LogActivityAsync(
            AuditAction.UserCreated,
            user.Id,
            user.Id,
            "User",
            $"User created: {user.Email}",
            null,
            null
        );

        return await GetUserByIdAsync(user.Id);
    }

    public async Task<UserResponse> UpdateUserAsync(Guid userId, UpdateUserRequest request, string? updatedBy)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        if (!string.IsNullOrEmpty(request.FirstName))
            user.FirstName = request.FirstName;

        if (!string.IsNullOrEmpty(request.LastName))
            user.LastName = request.LastName;

        if (!string.IsNullOrEmpty(request.PhoneNumber))
            user.PhoneNumber = request.PhoneNumber;

        if (!string.IsNullOrEmpty(request.Avatar))
            user.Avatar = request.Avatar;

        if (request.Status.HasValue)
            user.Status = request.Status.Value;

        user.UpdatedBy = updatedBy;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _auditService.LogActivityAsync(
            AuditAction.UserUpdated,
            userId,
            userId,
            "User",
            $"User updated: {user.Email}",
            null,
            null
        );

        return await GetUserByIdAsync(userId);
    }

    public async Task<bool> DeleteUserAsync(Guid userId, string? deletedBy)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        await _auditService.LogActivityAsync(
            AuditAction.UserDeleted,
            userId,
            userId,
            "User",
            $"User deleted: {user.Email}",
            null,
            null
        );

        return true;
    }

    public async Task<UserResponse> GetUserByIdAsync(Guid userId)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        var userResponse = _mapper.Map<UserResponse>(user);
        userResponse.Roles = user.UserRoles.Select(ur => new RoleResponse
        {
            Id = ur.Role.Id,
            Name = ur.Role.Name,
            Description = ur.Role.Description,
            IsSystemRole = ur.Role.IsSystemRole,
            Priority = ur.Role.Priority,
            CreatedAt = ur.Role.CreatedAt,
            ProjectId = ur.Role.ProjectId
        }).ToList();

        return userResponse;
    }

    public async Task<UserListResponse> GetUsersAsync(PaginationRequest request)
    {
        var query = _context.Users.AsQueryable();

        // Filter by project if specified
        if (!string.IsNullOrEmpty(request.ProjectId))
        {
            query = query.Where(u => u.ProjectId == request.ProjectId);
        }

        // Search functionality
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(u =>
                u.Username.Contains(request.SearchTerm) ||
                u.Email.Contains(request.SearchTerm) ||
                (u.FirstName != null && u.FirstName.Contains(request.SearchTerm)) ||
                (u.LastName != null && u.LastName.Contains(request.SearchTerm))
            );
        }

        var totalCount = await query.CountAsync();

        var users = await query
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .OrderBy(u => u.Username)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        var userResponses = users.Select(u =>
        {
            var userResponse = _mapper.Map<UserResponse>(u);
            userResponse.Roles = u.UserRoles.Select(ur => new RoleResponse
            {
                Id = ur.Role.Id,
                Name = ur.Role.Name,
                Description = ur.Role.Description,
                IsSystemRole = ur.Role.IsSystemRole,
                Priority = ur.Role.Priority,
                CreatedAt = ur.Role.CreatedAt,
                ProjectId = ur.Role.ProjectId
            }).ToList();
            return userResponse;
        }).ToList();

        return new UserListResponse
        {
            Users = userResponses,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<UserResponse> GetUserByEmailAsync(string email)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        return await GetUserByIdAsync(user.Id);
    }

    public async Task<bool> AssignRolesToUserAsync(Guid userId, List<Guid> roleIds, string? assignedBy)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        // Remove existing roles for this user
        var existingRoles = await _context.UserRoles.Where(ur => ur.UserId == userId).ToListAsync();
        
        foreach (var roleId in roleIds)
        {
            if (!existingRoles.Any(ur => ur.RoleId == roleId))
            {
                _context.UserRoles.Add(new UserRole
                {
                    UserId = userId,
                    RoleId = roleId,
                    AssignedBy = assignedBy
                });

                await _auditService.LogActivityAsync(
                    AuditAction.RoleAssigned,
                    userId,
                    userId,
                    "User",
                    $"Role assigned to user",
                    null,
                    null
                );
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RevokeRolesFromUserAsync(Guid userId, List<Guid> roleIds, string? revokedBy)
    {
        var userRoles = await _context.UserRoles
            .Where(ur => ur.UserId == userId && roleIds.Contains(ur.RoleId))
            .ToListAsync();

        _context.UserRoles.RemoveRange(userRoles);
        await _context.SaveChangesAsync();

        foreach (var roleId in roleIds)
        {
            await _auditService.LogActivityAsync(
                AuditAction.RoleRevoked,
                userId,
                userId,
                "User",
                $"Role revoked from user",
                null,
                null
            );
        }

        return true;
    }
}

