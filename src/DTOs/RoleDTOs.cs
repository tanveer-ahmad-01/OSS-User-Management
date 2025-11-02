namespace UserManagement.DTOs;

public class CreateRoleRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Priority { get; set; }
    public List<Guid>? PermissionIds { get; set; }
    public string? ProjectId { get; set; }
}

public class UpdateRoleRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? Priority { get; set; }
}

public class RoleResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }
    public int Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ProjectId { get; set; }
    public List<PermissionResponse> Permissions { get; set; } = new();
}

public class AssignRoleRequest
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}

public class RevokeRoleRequest
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}

