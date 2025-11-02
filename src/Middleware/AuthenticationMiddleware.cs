using System.Security.Claims;
using UserManagement.Services;

namespace UserManagement.Middleware;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthenticationMiddleware> _logger;

    public AuthenticationMiddleware(RequestDelegate next, ILogger<AuthenticationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IJwtService jwtService)
    {
        // Skip authentication for public endpoints
        if (IsPublicEndpoint(context.Request.Path))
        {
            await _next(context);
            return;
        }

        // If already authenticated by JWT middleware, continue
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            await _next(context);
            return;
        }

        // Try to extract token from header
        var token = ExtractTokenFromHeader(context.Request);

        if (!string.IsNullOrEmpty(token))
        {
            if (jwtService.ValidateToken(token))
            {
                // Create a claims principal from the token
                var userId = jwtService.GetUserIdFromToken(token);
                if (userId.HasValue)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString())
                    };

                    var identity = new ClaimsIdentity(claims, "Bearer");
                    context.User = new ClaimsPrincipal(identity);
                }
            }
            else
            {
                _logger.LogWarning("Invalid token provided: {Token}", token);
            }
        }

        await _next(context);
    }

    private static bool IsPublicEndpoint(PathString path)
    {
        var publicPaths = new[]
        {
            "/swagger",
            "/api/auth/login",
            "/api/auth/register",
            "/api/auth/refresh-token"
        };

        return publicPaths.Any(p => path.StartsWithSegments(p, StringComparison.OrdinalIgnoreCase));
    }

    private static string? ExtractTokenFromHeader(HttpRequest request)
    {
        var authHeader = request.Headers["Authorization"].FirstOrDefault();
        
        if (string.IsNullOrEmpty(authHeader))
            return null;

        if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return authHeader.Substring("Bearer ".Length).Trim();
        }

        return null;
    }
}

