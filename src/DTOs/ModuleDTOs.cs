namespace UserManagement.DTOs;

public class CreateModuleRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Code { get; set; } = string.Empty;
    public Guid? ParentModuleId { get; set; }
    public int Order { get; set; }
    public string? ProjectId { get; set; }
}

public class UpdateModuleRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? Order { get; set; }
    public bool? IsActive { get; set; }
}

public class ModuleResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Code { get; set; } = string.Empty;
    public Guid? ParentModuleId { get; set; }
    public int Order { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ProjectId { get; set; }
    public List<ModuleResponse> SubModules { get; set; } = new();
    public List<FeatureResponse> Features { get; set; } = new();
}

public class CreateFeatureRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Code { get; set; } = string.Empty;
    public Guid ModuleId { get; set; }
    public string? ProjectId { get; set; }
}

public class UpdateFeatureRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
}

public class FeatureResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Code { get; set; } = string.Empty;
    public Guid ModuleId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ProjectId { get; set; }
}

public class AssignPermissionRequest
{
    public Guid RoleId { get; set; }
    public List<Guid> PermissionIds { get; set; } = new();
}

public class PermissionResponse
{
    public Guid Id { get; set; }
    public Guid FeatureId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? Description { get; set; }
    public FeatureResponse? Feature { get; set; }
}

