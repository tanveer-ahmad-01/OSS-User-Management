namespace UserManagement.Models;

public enum UserStatus
{
    Active = 1,
    Inactive = 2,
    Suspended = 3
}

public enum AuditAction
{
    UserCreated = 1,
    UserUpdated = 2,
    UserDeleted = 3,
    LoginSuccess = 4,
    LoginFailed = 5,
    PasswordChanged = 6,
    RoleAssigned = 7,
    RoleRevoked = 8,
    PermissionGranted = 9,
    PermissionRevoked = 10,
    ModuleCreated = 11,
    ModuleUpdated = 12,
    ModuleDeleted = 13
}

public enum LoginMethod
{
    EmailPassword = 1,
    Google = 2,
    GitHub = 3,
    Microsoft = 4,
    Facebook = 5,
    LDAP = 6,
    SAML = 7,
    OAuth = 8,
    ApiKey = 9
}

public enum PermissionType
{
    Read = 1,
    Write = 2,
    Delete = 3,
    Execute = 4
}

