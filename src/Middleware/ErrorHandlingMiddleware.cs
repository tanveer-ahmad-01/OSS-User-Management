using System.Net;
using System.Text.Json;
using UserManagement.DTOs;

namespace UserManagement.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = exception switch
        {
            KeyNotFoundException => HttpStatusCode.NotFound,
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            InvalidOperationException => HttpStatusCode.Conflict,
            ArgumentException => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.InternalServerError
        };

        // Log detailed error information
        var errorDetails = new
        {
            ExceptionType = exception.GetType().Name,
            Message = exception.Message,
            Path = context.Request.Path,
            Method = context.Request.Method,
            UserId = context.User?.Identity?.Name,
            IpAddress = context.Connection.RemoteIpAddress?.ToString(),
            StackTrace = exception.StackTrace,
            InnerException = exception.InnerException?.Message
        };

        // Log based on severity
        if (code == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, 
                "Internal Server Error: {Message} | Path: {Path} | Method: {Method} | UserId: {UserId} | IP: {IpAddress}",
                exception.Message, context.Request.Path, context.Request.Method,
                context.User?.Identity?.Name, context.Connection.RemoteIpAddress?.ToString());
        }
        else
        {
            _logger.LogWarning("Client Error: {Message} | Path: {Path} | Method: {Method} | UserId: {UserId}",
                exception.Message, context.Request.Path, context.Request.Method,
                context.User?.Identity?.Name);
        }

        var response = new ApiResponse<object>
        {
            Success = false,
            Message = GetErrorMessage(exception),
            Errors = new List<string> { GetUserFriendlyMessage(exception) }
        };

        var result = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        await context.Response.WriteAsync(result);
    }

    private static string GetErrorMessage(Exception exception)
    {
        return exception switch
        {
            KeyNotFoundException => "Resource not found",
            UnauthorizedAccessException => "Unauthorized access",
            InvalidOperationException => "Operation failed",
            ArgumentException => "Invalid request",
            _ => "An error occurred while processing your request"
        };
    }

    private static string GetUserFriendlyMessage(Exception exception)
    {
        // Don't expose internal details to the client
        return exception switch
        {
            KeyNotFoundException => exception.Message,
            UnauthorizedAccessException => "You are not authorized to perform this action",
            InvalidOperationException => exception.Message,
            ArgumentException => exception.Message,
            _ => "An unexpected error occurred. Please try again later or contact support."
        };
    }
}

