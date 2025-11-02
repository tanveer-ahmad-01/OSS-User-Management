using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserManagement.DTOs;
using UserManagement.Services;

namespace UserManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<UserListResponse>>> GetUsers([FromQuery] PaginationRequest request)
    {
        try
        {
            var response = await _userService.GetUsersAsync(request);

            return Ok(new ApiResponse<UserListResponse>
            {
                Success = true,
                Message = "Users retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<UserListResponse>
            {
                Success = false,
                Message = "Failed to retrieve users",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> GetUser(Guid id)
    {
        try
        {
            var response = await _userService.GetUserByIdAsync(id);

            return Ok(new ApiResponse<UserResponse>
            {
                Success = true,
                Message = "User retrieved successfully",
                Data = response
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<UserResponse>
            {
                Success = false,
                Message = ex.Message,
                Errors = new List<string> { ex.Message }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<UserResponse>
            {
                Success = false,
                Message = "Failed to retrieve user",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<UserResponse>>> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _userService.CreateUserAsync(request, currentUserId);

            return CreatedAtAction(nameof(GetUser), new { id = response.Id }, new ApiResponse<UserResponse>
            {
                Success = true,
                Message = "User created successfully",
                Data = response
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ApiResponse<UserResponse>
            {
                Success = false,
                Message = ex.Message,
                Errors = new List<string> { ex.Message }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<UserResponse>
            {
                Success = false,
                Message = "Failed to create user",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
    {
        try
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _userService.UpdateUserAsync(id, request, currentUserId);

            return Ok(new ApiResponse<UserResponse>
            {
                Success = true,
                Message = "User updated successfully",
                Data = response
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<UserResponse>
            {
                Success = false,
                Message = ex.Message,
                Errors = new List<string> { ex.Message }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<UserResponse>
            {
                Success = false,
                Message = "Failed to update user",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteUser(Guid id)
    {
        try
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _userService.DeleteUserAsync(id, currentUserId);

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "User deleted successfully",
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
                Message = "Failed to delete user",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPost("{id}/assign-roles")]
    public async Task<ActionResult<ApiResponse<bool>>> AssignRoles(Guid id, [FromBody] List<Guid> roleIds)
    {
        try
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _userService.AssignRolesToUserAsync(id, roleIds, currentUserId);

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Roles assigned successfully",
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
                Message = "Failed to assign roles",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPost("{id}/revoke-roles")]
    public async Task<ActionResult<ApiResponse<bool>>> RevokeRoles(Guid id, [FromBody] List<Guid> roleIds)
    {
        try
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _userService.RevokeRolesFromUserAsync(id, roleIds, currentUserId);

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Roles revoked successfully",
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
                Message = "Failed to revoke roles",
                Errors = new List<string> { ex.Message }
            });
        }
    }
}

