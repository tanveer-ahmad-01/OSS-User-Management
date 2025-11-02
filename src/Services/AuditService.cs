using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.DTOs;
using UserManagement.Models;

namespace UserManagement.Services;

public class AuditService : IAuditService
{
    private readonly ApplicationDbContext _context;

    public AuditService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task LogActivityAsync(
        AuditAction action,
        Guid? userId,
        Guid? entityId,
        string? entityType,
        string? details,
        string? ipAddress,
        string? userAgent)
    {
        var auditLog = new AuditLog
        {
            Action = action,
            UserId = userId,
            EntityId = entityId,
            EntityType = entityType,
            Details = details,
            IpAddress = ipAddress,
            UserAgent = userAgent
        };

        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();
    }

    public async Task<AuditLogListResponse> GetAuditLogsAsync(AuditLogFilterRequest request)
    {
        var query = _context.AuditLogs
            .Include(a => a.User)
            .AsQueryable();

        if (request.Action.HasValue)
        {
            query = query.Where(a => a.Action == request.Action.Value);
        }

        if (request.UserId.HasValue)
        {
            query = query.Where(a => a.UserId == request.UserId.Value);
        }

        if (!string.IsNullOrEmpty(request.EntityType))
        {
            query = query.Where(a => a.EntityType == request.EntityType);
        }

        if (request.StartDate.HasValue)
        {
            query = query.Where(a => a.Timestamp >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            query = query.Where(a => a.Timestamp <= request.EndDate.Value);
        }

        if (!string.IsNullOrEmpty(request.ProjectId))
        {
            query = query.Where(a => a.ProjectId == request.ProjectId);
        }

        var totalCount = await query.CountAsync();

        var auditLogs = await query
            .OrderByDescending(a => a.Timestamp)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        var auditLogResponses = auditLogs.Select(a => new AuditLogResponse
        {
            Id = a.Id,
            Action = a.Action,
            UserId = a.UserId,
            Username = a.User?.Username,
            EntityId = a.EntityId,
            EntityType = a.EntityType,
            Details = a.Details,
            IpAddress = a.IpAddress,
            UserAgent = a.UserAgent,
            Timestamp = a.Timestamp,
            ProjectId = a.ProjectId
        }).ToList();

        return new AuditLogListResponse
        {
            AuditLogs = auditLogResponses,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}

