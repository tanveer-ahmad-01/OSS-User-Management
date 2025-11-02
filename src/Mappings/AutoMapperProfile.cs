using AutoMapper;
using UserManagement.DTOs;
using UserManagement.Models;

namespace UserManagement.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // User mappings
        CreateMap<User, UserResponse>();
        CreateMap<User, UserListResponse>();

        // Role mappings
        CreateMap<Role, RoleResponse>();

        // Module mappings
        CreateMap<Module, ModuleResponse>();

        // Feature mappings
        CreateMap<Feature, FeatureResponse>();

        // Permission mappings
        CreateMap<Permission, PermissionResponse>();

        // AuditLog mappings
        CreateMap<AuditLog, AuditLogResponse>();
    }
}

