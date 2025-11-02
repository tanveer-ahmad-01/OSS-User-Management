using System.Collections.Concurrent;

namespace UserManagement.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly ConcurrentDictionary<string, List<DateTime>> _requests;
    private readonly int _maxRequests;
    private readonly TimeSpan _timeWindow;

    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger, int maxRequests = 100, int timeWindowMinutes = 1)
    {
        _next = next;
        _logger = logger;
        _requests = new ConcurrentDictionary<string, List<DateTime>>();
        _maxRequests = maxRequests;
        _timeWindow = TimeSpan.FromMinutes(timeWindowMinutes);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip rate limiting for swagger and health checks
        if (IsSkipEndpoint(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var key = clientIp;

        // Clean up old requests
        CleanupOldRequests(key);

        // Check rate limit
        if (HasExceededRateLimit(key))
        {
            _logger.LogWarning("Rate limit exceeded for IP: {IP}", clientIp);
            context.Response.StatusCode = 429;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"success\":false,\"message\":\"Too many requests. Please try again later.\"}");
            return;
        }

        // Record this request
        RecordRequest(key);

        await _next(context);
    }

    private bool HasExceededRateLimit(string key)
    {
        if (!_requests.TryGetValue(key, out var timestamps))
        {
            return false;
        }

        return timestamps.Count >= _maxRequests;
    }

    private void RecordRequest(string key)
    {
        _requests.AddOrUpdate(key,
            new List<DateTime> { DateTime.UtcNow },
            (k, v) =>
            {
                v.Add(DateTime.UtcNow);
                return v;
            });
    }

    private void CleanupOldRequests(string key)
    {
        if (!_requests.TryGetValue(key, out var timestamps))
        {
            return;
        }

        var cutoff = DateTime.UtcNow - _timeWindow;
        timestamps.RemoveAll(t => t < cutoff);

        // Remove empty entries
        if (timestamps.Count == 0)
        {
            _requests.TryRemove(key, out _);
        }
    }

    private static bool IsSkipEndpoint(PathString path)
    {
        var skipPaths = new[]
        {
            "/swagger",
            "/health"
        };

        return skipPaths.Any(p => path.StartsWithSegments(p, StringComparison.OrdinalIgnoreCase));
    }
}

