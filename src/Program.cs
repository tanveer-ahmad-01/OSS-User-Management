using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Log4Net
builder.Logging.AddLog4Net("log4net.config");
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Configure application settings
builder.Services.AddApplicationSettings(builder.Configuration);

// Configure database
builder.Services.AddDatabase(builder.Configuration);

// Configure JWT authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

// Add application services
builder.Services.AddApplicationServices();

// Configure AutoMapper
builder.Services.AddAutoMapperConfig();

// Configure FluentValidation
builder.Services.AddFluentValidationConfig();

// Configure CORS
builder.Services.AddCorsConfig();

// Configure Swagger
builder.Services.AddSwaggerConfig();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Management Plugin System API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at app's root
    });
}

// Configure global exception handler
app.UseGlobalExceptionHandler();

// Configure request logging
app.UseRequestLogging();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

// Configure custom authentication
app.UseCustomAuthentication();

app.UseAuthentication();
app.UseAuthorization();

// Configure rate limiting
app.UseCustomRateLimiting();

app.MapControllers();

// Migrate database on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();
        
        context.Database.Migrate();
        logger.LogInformation("Database migrated successfully");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database");
    }
}

var startupLogger = app.Services.GetRequiredService<ILogger<Program>>();
startupLogger.LogInformation("User Management Plugin System API is starting up");

app.Run();

