using UserManagement.Models;

namespace UserManagement.DTOs;

public class AuditLogResponse
{
    public Guid Id { get; set; }
    public AuditAction Action { get; set; }
    public Guid? UserId { get; set; }
    public string? Username { get; set; }
    public Guid? EntityId { get; set; }
    public string? EntityType { get; set; }
    public string? Details { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime Timestamp { get; set; }
    public string? ProjectId { get; set; }
}

public class AuditLogFilterRequest
{
    public AuditAction? Action { get; set; }
    public Guid? UserId { get; set; }
    public string? EntityType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? ProjectId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

public class AuditLogListResponse
{
    public List<AuditLogResponse> AuditLogs { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

