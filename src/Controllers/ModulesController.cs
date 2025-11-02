using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserManagement.DTOs;
using UserManagement.Services;

namespace UserManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ModulesController : ControllerBase
{
    private readonly IModuleService _moduleService;

    public ModulesController(IModuleService moduleService)
    {
        _moduleService = moduleService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ModuleResponse>>>> GetModules([FromQuery] string? projectId)
    {
        try
        {
            var response = await _moduleService.GetModulesAsync(projectId);

            return Ok(new ApiResponse<List<ModuleResponse>>
            {
                Success = true,
                Message = "Modules retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<List<ModuleResponse>>
            {
                Success = false,
                Message = "Failed to retrieve modules",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ModuleResponse>>> GetModule(Guid id)
    {
        try
        {
            var response = await _moduleService.GetModuleByIdAsync(id);

            return Ok(new ApiResponse<ModuleResponse>
            {
                Success = true,
                Message = "Module retrieved successfully",
                Data = response
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<ModuleResponse>
            {
                Success = false,
                Message = ex.Message,
                Errors = new List<string> { ex.Message }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<ModuleResponse>
            {
                Success = false,
                Message = "Failed to retrieve module",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ModuleResponse>>> CreateModule([FromBody] CreateModuleRequest request)
    {
        try
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _moduleService.CreateModuleAsync(request, currentUserId);

            return CreatedAtAction(nameof(GetModule), new { id = response.Id }, new ApiResponse<ModuleResponse>
            {
                Success = true,
                Message = "Module created successfully",
                Data = response
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ApiResponse<ModuleResponse>
            {
                Success = false,
                Message = ex.Message,
                Errors = new List<string> { ex.Message }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<ModuleResponse>
            {
                Success = false,
                Message = "Failed to create module",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ModuleResponse>>> UpdateModule(Guid id, [FromBody] UpdateModuleRequest request)
    {
        try
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _moduleService.UpdateModuleAsync(id, request, currentUserId);

            return Ok(new ApiResponse<ModuleResponse>
            {
                Success = true,
                Message = "Module updated successfully",
                Data = response
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<ModuleResponse>
            {
                Success = false,
                Message = ex.Message,
                Errors = new List<string> { ex.Message }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<ModuleResponse>
            {
                Success = false,
                Message = "Failed to update module",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteModule(Guid id)
    {
        try
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _moduleService.DeleteModuleAsync(id, currentUserId);

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Module deleted successfully",
                Data = result
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<bool>
            {
                Success = false,
                Message = ex.Message,
                Errors = new List<string> { ex.Message }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<bool>
            {
                Success = false,
                Message = "Failed to delete module",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpGet("{moduleId}/features")]
    public async Task<ActionResult<ApiResponse<List<FeatureResponse>>>> GetFeatures(Guid moduleId)
    {
        try
        {
            var response = await _moduleService.GetFeaturesAsync(moduleId);

            return Ok(new ApiResponse<List<FeatureResponse>>
            {
                Success = true,
                Message = "Features retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<List<FeatureResponse>>
            {
                Success = false,
                Message = "Failed to retrieve features",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPost("features")]
    public async Task<ActionResult<ApiResponse<FeatureResponse>>> CreateFeature([FromBody] CreateFeatureRequest request)
    {
        try
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _moduleService.CreateFeatureAsync(request, currentUserId);

            return Ok(new ApiResponse<FeatureResponse>
            {
                Success = true,
                Message = "Feature created successfully",
                Data = response
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ApiResponse<FeatureResponse>
            {
                Success = false,
                Message = ex.Message,
                Errors = new List<string> { ex.Message }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<FeatureResponse>
            {
                Success = false,
                Message = "Failed to create feature",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPut("features/{id}")]
    public async Task<ActionResult<ApiResponse<FeatureResponse>>> UpdateFeature(Guid id, [FromBody] UpdateFeatureRequest request)
    {
        try
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _moduleService.UpdateFeatureAsync(id, request, currentUserId);

            return Ok(new ApiResponse<FeatureResponse>
            {
                Success = true,
                Message = "Feature updated successfully",
                Data = response
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<FeatureResponse>
            {
                Success = false,
                Message = ex.Message,
                Errors = new List<string> { ex.Message }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<FeatureResponse>
            {
                Success = false,
                Message = "Failed to update feature",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpDelete("features/{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteFeature(Guid id)
    {
        try
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _moduleService.DeleteFeatureAsync(id, currentUserId);

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Feature deleted successfully",
                Data = result
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<bool>
            {
                Success = false,
                Message = ex.Message,
                Errors = new List<string> { ex.Message }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<bool>
            {
                Success = false,
                Message = "Failed to delete feature",
                Errors = new List<string> { ex.Message }
            });
        }
    }
}

