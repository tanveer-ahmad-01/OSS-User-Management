using System.Diagnostics;
using System.Text;

namespace UserManagement.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestBody = await GetRequestBody(context.Request);
        
        _logger.LogInformation(
            "Incoming request: {Method} {Path} | Body: {RequestBody}",
            context.Request.Method,
            context.Request.Path,
            requestBody
        );

        var originalBodyStream = context.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            
            var responseBodyContent = await GetResponseBody(context.Response);
            
            _logger.LogInformation(
                "Completed request: {Method} {Path} | Status: {StatusCode} | Duration: {ElapsedMilliseconds}ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds
            );

            await responseBody.CopyToAsync(originalBodyStream);
        }
    }

    private static async Task<string> GetRequestBody(HttpRequest request)
    {
        request.EnableBuffering();
        var buffer = new byte[Convert.ToInt32(request.ContentLength ?? 0)];
        await request.Body.ReadAsync(buffer, 0, buffer.Length);
        var bodyAsText = Encoding.UTF8.GetString(buffer);
        request.Body.Position = 0;
        return bodyAsText;
    }

    private static async Task<string> GetResponseBody(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        var text = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);
        return text;
    }
}

