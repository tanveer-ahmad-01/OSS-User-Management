using UserManagement.DTOs;

namespace UserManagement.Services;

public interface IModuleService
{
    Task<ModuleResponse> CreateModuleAsync(CreateModuleRequest request, string? createdBy);
    Task<ModuleResponse> UpdateModuleAsync(Guid moduleId, UpdateModuleRequest request, string? updatedBy);
    Task<bool> DeleteModuleAsync(Guid moduleId, string? deletedBy);
    Task<ModuleResponse> GetModuleByIdAsync(Guid moduleId);
    Task<List<ModuleResponse>> GetModulesAsync(string? projectId);
    Task<FeatureResponse> CreateFeatureAsync(CreateFeatureRequest request, string? createdBy);
    Task<FeatureResponse> UpdateFeatureAsync(Guid featureId, UpdateFeatureRequest request, string? updatedBy);
    Task<bool> DeleteFeatureAsync(Guid featureId, string? deletedBy);
    Task<List<FeatureResponse>> GetFeaturesAsync(Guid moduleId);
}

