using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.DTOs;
using UserManagement.Services;

namespace UserManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuditLogsController : ControllerBase
{
    private readonly IAuditService _auditService;

    public AuditLogsController(IAuditService auditService)
    {
        _auditService = auditService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<AuditLogListResponse>>> GetAuditLogs([FromQuery] AuditLogFilterRequest request)
    {
        try
        {
            var response = await _auditService.GetAuditLogsAsync(request);

            return Ok(new ApiResponse<AuditLogListResponse>
            {
                Success = true,
                Message = "Audit logs retrieved successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<AuditLogListResponse>
            {
                Success = false,
                Message = "Failed to retrieve audit logs",
                Errors = new List<string> { ex.Message }
            });
        }
    }
}

