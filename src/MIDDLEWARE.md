# Middleware Documentation

This document describes all middleware components in the User Management Plugin System.

## Overview

The system includes four custom middleware components that enhance security, logging, and error handling:

1. **ErrorHandlingMiddleware** - Global exception handling
2. **RequestLoggingMiddleware** - Request/response logging
3. **AuthenticationMiddleware** - Custom authentication flow
4. **RateLimitingMiddleware** - Protection against brute force attacks

## ErrorHandlingMiddleware

### Purpose
Provides centralized exception handling across all API endpoints.

### Functionality
- Catches all unhandled exceptions
- Maps exceptions to appropriate HTTP status codes
- Returns consistent error response format
- Logs errors for debugging

### Exception Mapping
| Exception Type | HTTP Status Code | Description |
|----------------|------------------|-------------|
| KeyNotFoundException | 404 Not Found | Resource doesn't exist |
| UnauthorizedAccessException | 401 Unauthorized | Authentication failed |
| InvalidOperationException | 409 Conflict | Business rule violation |
| ArgumentException | 400 Bad Request | Invalid input |
| All others | 500 Internal Server Error | Server error |

### Example Response
```json
{
  "success": false,
  "message": "Resource not found",
  "errors": ["User not found"]
}
```

### Configuration
Registered in `Program.cs`:
```csharp
app.UseGlobalExceptionHandler();
```

## RequestLoggingMiddleware

### Purpose
Logs all incoming requests and outgoing responses with timing information.

### Functionality
- Logs request method, path, and body
- Logs response status code and duration
- Captures request/response content
- Uses Serilog for structured logging

### Log Format
```
Incoming request: POST /api/users | Body: {"username":"john"...}
Completed request: POST /api/users | Status: 201 | Duration: 125ms
```

### Configuration
Registered in `Program.cs`:
```csharp
app.UseRequestLogging();
```

### Performance Impact
Minimal impact due to efficient memory stream handling. Disable for production if needed for higher throughput.

## AuthenticationMiddleware

### Purpose
Provides additional authentication layer and token validation.

### Functionality
- Validates JWT tokens before reaching controllers
- Creates claims principal from token
- Bypasses authentication for public endpoints
- Extracts tokens from Authorization header

### Public Endpoints (No Authentication Required)
- `/swagger/*` - API documentation
- `/api/auth/login` - User login
- `/api/auth/register` - User registration
- `/api/auth/refresh-token` - Token refresh

### Configuration
Registered in `Program.cs`:
```csharp
app.UseCustomAuthentication();
```

### Ordering
Must be registered **before** the built-in `UseAuthentication()` middleware.

## RateLimitingMiddleware

### Purpose
Protects the API from brute force attacks and excessive usage.

### Functionality
- Limits requests per IP address
- Default: 100 requests per minute per IP
- Automatically cleans up old request records
- Skips rate limiting for certain endpoints

### Endpoints Excluded from Rate Limiting
- `/swagger/*` - API documentation
- `/health` - Health checks

### Configuration
Registered in `Program.cs`:
```csharp
app.UseCustomRateLimiting();
```

### Customization
You can customize the rate limiting in `RateLimitingMiddleware.cs`:
```csharp
public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger, 
    int maxRequests = 100, int timeWindowMinutes = 1)
```

### Response When Limit Exceeded
```json
{
  "success": false,
  "message": "Too many requests. Please try again later."
}
```
**HTTP Status:** 429 Too Many Requests

## Middleware Order

The middleware is registered in the following order in `Program.cs`:

1. **GlobalExceptionHandler** - Catches all exceptions
2. **RequestLogging** - Logs all requests
3. **HTTPS Redirection** - Redirects HTTP to HTTPS
4. **CORS** - Handles cross-origin requests
5. **CustomAuthentication** - Custom token validation
6. **UseAuthentication** - Built-in ASP.NET Core authentication
7. **UseAuthorization** - Authorization checks
8. **RateLimiting** - Prevents abuse
9. **MapControllers** - Routes to controllers

This order ensures:
- Errors are caught before responses are written
- Requests are logged before processing
- Authentication is checked before authorization
- Rate limiting is applied last to protect all routes

## Best Practices

### Development
- All middleware is active in development
- Detailed logging helps debugging
- Rate limiting may be disabled for testing

### Production
- Keep error handling middleware enabled
- Consider disabling request body logging for performance
- Adjust rate limiting based on expected traffic
- Monitor logs for suspicious patterns

### Testing
- Rate limiting can impact automated tests
- Use test-specific IP ranges or bypass for test environments
- Mock middleware if needed for unit tests

## Extending Middleware

### Adding Custom Middleware

1. Create a new middleware class in `Middleware/` folder:
```csharp
public class CustomMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CustomMiddleware> _logger;

    public CustomMiddleware(RequestDelegate next, ILogger<CustomMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Your logic here
        await _next(context);
    }
}
```

2. Add extension method in `Extensions/MiddlewareExtensions.cs`:
```csharp
public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder builder)
{
    return builder.UseMiddleware<CustomMiddleware>();
}
```

3. Register in `Program.cs`:
```csharp
app.UseCustomMiddleware();
```

## Troubleshooting

### Issues

**Middleware not executing:**
- Check registration order in `Program.cs`
- Ensure middleware is registered before `MapControllers`

**Rate limiting blocking legitimate requests:**
- Increase rate limit in constructor
- Add IP to allowed list
- Check for proxy/CDN adding shared IP

**Authentication not working:**
- Verify JWT secret key is configured
- Check token expiration time
- Ensure Authorization header format: `Bearer <token>`

**Error responses not formatted correctly:**
- Verify ErrorHandlingMiddleware is first
- Check for exceptions in middleware itself
- Ensure JSON serialization is configured

## Security Considerations

- Rate limiting helps prevent DDoS attacks
- Error messages don't expose sensitive information
- Logging includes IP addresses for security audit
- Request bodies are logged; remove sensitive data if needed
- Middleware operates before authentication for performance

