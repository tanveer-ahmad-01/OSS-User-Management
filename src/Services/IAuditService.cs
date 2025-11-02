using UserManagement.DTOs;
using UserManagement.Models;

namespace UserManagement.Services;

public interface IAuditService
{
    Task LogActivityAsync(
        AuditAction action,
        Guid? userId,
        Guid? entityId,
        string? entityType,
        string? details,
        string? ipAddress,
        string? userAgent);

    Task<AuditLogListResponse> GetAuditLogsAsync(AuditLogFilterRequest request);
}

