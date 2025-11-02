namespace UserManagement.Configurations;

public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; set; } = 60;
    public int RefreshTokenExpirationDays { get; set; } = 7;
}

public class DatabaseSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string Provider { get; set; } = "SqlServer";
}

public class ApplicationSettings
{
    public string ApplicationName { get; set; } = "User Management Plugin System";
    public string Version { get; set; } = "1.0";
    public bool EnableSwagger { get; set; } = true;
    public bool EnableDetailedErrors { get; set; } = false;
}

public class SecuritySettings
{
    public int MaxLoginAttempts { get; set; } = 5;
    public int LockoutDurationMinutes { get; set; } = 30;
    public bool RequireEmailVerification { get; set; } = false;
    public int PasswordMinLength { get; set; } = 8;
    public bool RequireStrongPassword { get; set; } = true;
}

public class AppSettings
{
    public JwtSettings Jwt { get; set; } = new();
    public DatabaseSettings Database { get; set; } = new();
    public ApplicationSettings Application { get; set; } = new();
    public SecuritySettings Security { get; set; } = new();
}

