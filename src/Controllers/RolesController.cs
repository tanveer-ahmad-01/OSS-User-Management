using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserManagement.DTOs;
using UserManagement.Services;

namespace UserManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<RoleResponse>>>> GetRoles([FromQuery] string? projectId)
    {
        try
        {
            var response = await _roleService.GetRolesAsync(projectId);

            return Ok(new ApiResponse<List<RoleResponse>>
            {
                Success = true,
                Message = "Roles retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<List<RoleResponse>>
            {
                Success = false,
                Message = "Failed to retrieve roles",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<RoleResponse>>> GetRole(Guid id)
    {
        try
        {
            var response = await _roleService.GetRoleByIdAsync(id);

            return Ok(new ApiResponse<RoleResponse>
            {
                Success = true,
                Message = "Role retrieved successfully",
                Data = response
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<RoleResponse>
            {
                Success = false,
                Message = ex.Message,
                Errors = new List<string> { ex.Message }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<RoleResponse>
            {
                Success = false,
                Message = "Failed to retrieve role",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<RoleResponse>>> CreateRole([FromBody] CreateRoleRequest request)
    {
        try
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _roleService.CreateRoleAsync(request, currentUserId);

            return CreatedAtAction(nameof(GetRole), new { id = response.Id }, new ApiResponse<RoleResponse>
            {
                Success = true,
                Message = "Role created successfully",
                Data = response
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ApiResponse<RoleResponse>
            {
                Success = false,
                Message = ex.Message,
                Errors = new List<string> { ex.Message }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<RoleResponse>
            {
                Success = false,
                Message = "Failed to create role",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<RoleResponse>>> UpdateRole(Guid id, [FromBody] UpdateRoleRequest request)
    {
        try
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _roleService.UpdateRoleAsync(id, request, currentUserId);

            return Ok(new ApiResponse<RoleResponse>
            {
                Success = true,
                Message = "Role updated successfully",
                Data = response
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<RoleResponse>
            {
                Success = false,
                Message = ex.Message,
                Errors = new List<string> { ex.Message }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<RoleResponse>
            {
                Success = false,
                Message = "Failed to update role",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteRole(Guid id)
    {
        try
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _roleService.DeleteRoleAsync(id, currentUserId);

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Role deleted successfully",
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
                Message = "Failed to delete role",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPost("{id}/assign-permissions")]
    public async Task<ActionResult<ApiResponse<bool>>> AssignPermissions(Guid id, [FromBody] List<Guid> permissionIds)
    {
        try
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _roleService.AssignPermissionsToRoleAsync(id, permissionIds, currentUserId);

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Permissions assigned successfully",
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
                Message = "Failed to assign permissions",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPost("{id}/revoke-permissions")]
    public async Task<ActionResult<ApiResponse<bool>>> RevokePermissions(Guid id, [FromBody] List<Guid> permissionIds)
    {
        try
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _roleService.RevokePermissionsFromRoleAsync(id, permissionIds, currentUserId);

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Permissions revoked successfully",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<bool>
            {
                Success = false,
                Message = "Failed to revoke permissions",
                Errors = new List<string> { ex.Message }
            });
        }
    }
}

