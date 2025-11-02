using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.DTOs;
using UserManagement.Models;

namespace UserManagement.Services;

public class ModuleService : IModuleService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IAuditService _auditService;

    public ModuleService(ApplicationDbContext context, IMapper mapper, IAuditService auditService)
    {
        _context = context;
        _mapper = mapper;
        _auditService = auditService;
    }

    public async Task<ModuleResponse> CreateModuleAsync(CreateModuleRequest request, string? createdBy)
    {
        if (await _context.Modules.AnyAsync(m => m.Code == request.Code && m.ProjectId == request.ProjectId))
        {
            throw new InvalidOperationException("Module with this code already exists");
        }

        var module = new Module
        {
            Name = request.Name,
            Description = request.Description,
            Code = request.Code,
            ParentModuleId = request.ParentModuleId,
            Order = request.Order,
            ProjectId = request.ProjectId,
            CreatedBy = createdBy
        };

        _context.Modules.Add(module);
        await _context.SaveChangesAsync();

        await _auditService.LogActivityAsync(
            AuditAction.ModuleCreated,
            null,
            module.Id,
            "Module",
            $"Module created: {module.Name}",
            null,
            null
        );

        return await GetModuleByIdAsync(module.Id);
    }

    public async Task<ModuleResponse> UpdateModuleAsync(Guid moduleId, UpdateModuleRequest request, string? updatedBy)
    {
        var module = await _context.Modules.FindAsync(moduleId);
        if (module == null)
        {
            throw new KeyNotFoundException("Module not found");
        }

        if (!string.IsNullOrEmpty(request.Name))
            module.Name = request.Name;

        if (!string.IsNullOrEmpty(request.Description))
            module.Description = request.Description;

        if (request.Order.HasValue)
            module.Order = request.Order.Value;

        if (request.IsActive.HasValue)
            module.IsActive = request.IsActive.Value;

        module.UpdatedBy = updatedBy;
        module.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _auditService.LogActivityAsync(
            AuditAction.ModuleUpdated,
            null,
            moduleId,
            "Module",
            $"Module updated: {module.Name}",
            null,
            null
        );

        return await GetModuleByIdAsync(moduleId);
    }

    public async Task<bool> DeleteModuleAsync(Guid moduleId, string? deletedBy)
    {
        var module = await _context.Modules.FindAsync(moduleId);
        if (module == null)
        {
            throw new KeyNotFoundException("Module not found");
        }

        _context.Modules.Remove(module);
        await _context.SaveChangesAsync();

        await _auditService.LogActivityAsync(
            AuditAction.ModuleDeleted,
            null,
            moduleId,
            "Module",
            $"Module deleted: {module.Name}",
            null,
            null
        );

        return true;
    }

    public async Task<ModuleResponse> GetModuleByIdAsync(Guid moduleId)
    {
        var module = await _context.Modules
            .Include(m => m.SubModules)
            .Include(m => m.Features)
            .FirstOrDefaultAsync(m => m.Id == moduleId);

        if (module == null)
        {
            throw new KeyNotFoundException("Module not found");
        }

        return _mapper.Map<ModuleResponse>(module);
    }

    public async Task<List<ModuleResponse>> GetModulesAsync(string? projectId)
    {
        var query = _context.Modules
            .Include(m => m.SubModules)
            .Include(m => m.Features)
            .AsQueryable();

        if (!string.IsNullOrEmpty(projectId))
        {
            query = query.Where(m => m.ProjectId == projectId);
        }

        var modules = await query.OrderBy(m => m.Order).ToListAsync();
        return _mapper.Map<List<ModuleResponse>>(modules);
    }

    public async Task<FeatureResponse> CreateFeatureAsync(CreateFeatureRequest request, string? createdBy)
    {
        if (await _context.Features.AnyAsync(f => f.Code == request.Code && f.ModuleId == request.ModuleId))
        {
            throw new InvalidOperationException("Feature with this code already exists");
        }

        var feature = new Feature
        {
            Name = request.Name,
            Description = request.Description,
            Code = request.Code,
            ModuleId = request.ModuleId,
            ProjectId = request.ProjectId,
            CreatedBy = createdBy
        };

        _context.Features.Add(feature);
        await _context.SaveChangesAsync();

        // Create default permissions for the feature
        var permissionTypes = Enum.GetValues<PermissionType>();
        foreach (var permissionType in permissionTypes)
        {
            _context.Permissions.Add(new Permission
            {
                FeatureId = feature.Id,
                Type = permissionType,
                Description = $"{permissionType} permission for {feature.Name}"
            });
        }

        await _context.SaveChangesAsync();

        return _mapper.Map<FeatureResponse>(feature);
    }

    public async Task<FeatureResponse> UpdateFeatureAsync(Guid featureId, UpdateFeatureRequest request, string? updatedBy)
    {
        var feature = await _context.Features.FindAsync(featureId);
        if (feature == null)
        {
            throw new KeyNotFoundException("Feature not found");
        }

        if (!string.IsNullOrEmpty(request.Name))
            feature.Name = request.Name;

        if (!string.IsNullOrEmpty(request.Description))
            feature.Description = request.Description;

        if (request.IsActive.HasValue)
            feature.IsActive = request.IsActive.Value;

        feature.UpdatedBy = updatedBy;
        feature.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return _mapper.Map<FeatureResponse>(feature);
    }

    public async Task<bool> DeleteFeatureAsync(Guid featureId, string? deletedBy)
    {
        var feature = await _context.Features.FindAsync(featureId);
        if (feature == null)
        {
            throw new KeyNotFoundException("Feature not found");
        }

        _context.Features.Remove(feature);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<FeatureResponse>> GetFeaturesAsync(Guid moduleId)
    {
        var features = await _context.Features
            .Where(f => f.ModuleId == moduleId)
            .ToListAsync();

        return _mapper.Map<List<FeatureResponse>>(features);
    }
}

