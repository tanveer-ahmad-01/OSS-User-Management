# User Management Plugin System (UMPS)

A comprehensive, pluggable User Management System built with ASP.NET Core Web API that can integrate with any application across any domain. This system provides configurable and secure user management functionalities including authentication, authorization, role-based access control (RBAC), feature and module configuration, and comprehensive audit logging.

## Features

### Core Functionalities

- **User Management**: Complete CRUD operations for users with profile management
- **Authentication & Authorization**: 
  - JWT-based token authentication
  - Refresh token support
  - Password hashing using BCrypt
  - Multi-Factor Authentication (MFA) support (structure ready)
- **Role-Based Access Control (RBAC)**:
  - Hierarchical role system
  - Fine-grained permission control
  - Module and feature-level access control
- **Module & Feature Configuration**: 
  - Dynamic module creation
  - Nested submodules support
  - Feature-to-module mapping
- **Multi-Tenant Support**: Project-based user and role isolation
- **Audit Logging**: Comprehensive activity tracking for compliance
- **RESTful API**: Clean, well-documented API endpoints
- **Request Validation**: FluentValidation for all API requests
- **Middleware**:
  - Global error handling
  - Request logging
  - Rate limiting (100 requests/minute per IP)
  - Custom authentication flow

### Security Features

- JWT token-based authentication
- BCrypt password hashing
- HTTPS support
- CORS configuration
- SQL injection prevention via EF Core
- XSS protection through API design
- Audit trails for all critical operations

## Technology Stack

- **Framework**: ASP.NET Core 8.0
- **Database**: SQL Server (LocalDB for development)
- **ORM**: Entity Framework Core
- **Authentication**: JWT Bearer Tokens
- **Mapping**: AutoMapper
- **Logging**: Serilog
- **API Documentation**: Swagger/OpenAPI

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- SQL Server (LocalDB, Express, or Full version)
- Visual Studio 2022, Visual Studio Code, or JetBrains Rider

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd Backend/src
   ```

2. **Configure the application**
   
   Update `appsettings.json` with your database connection string:
   ```json
   {
     "AppSettings": {
       "Database": {
         "ConnectionString": "Your connection string here"
       },
       "Jwt": {
         "SecretKey": "Your secret key here (minimum 256 bits)"
       }
     }
   }
   ```

3. **Install dependencies and run migrations**
   ```bash
   dotnet restore
   dotnet ef database update
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Access Swagger UI**
   
   Navigate to: `https://localhost:5001` or `http://localhost:5000`

## API Documentation

The complete API documentation is available via Swagger UI when the application is running.

### Key Endpoints

#### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration
- `POST /api/auth/refresh-token` - Refresh access token
- `POST /api/auth/change-password` - Change user password
- `POST /api/auth/revoke-token` - Revoke refresh token

#### Users
- `GET /api/users` - Get all users (paginated)
- `GET /api/users/{id}` - Get user by ID
- `POST /api/users` - Create new user
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user
- `POST /api/users/{id}/assign-roles` - Assign roles to user
- `POST /api/users/{id}/revoke-roles` - Revoke roles from user

#### Roles
- `GET /api/roles` - Get all roles
- `GET /api/roles/{id}` - Get role by ID with permissions
- `POST /api/roles` - Create new role
- `PUT /api/roles/{id}` - Update role
- `DELETE /api/roles/{id}` - Delete role
- `POST /api/roles/{id}/assign-permissions` - Assign permissions to role
- `POST /api/roles/{id}/revoke-permissions` - Revoke permissions from role

#### Modules & Features
- `GET /api/modules` - Get all modules
- `GET /api/modules/{id}` - Get module by ID with features
- `POST /api/modules` - Create new module
- `PUT /api/modules/{id}` - Update module
- `DELETE /api/modules/{id}` - Delete module
- `GET /api/modules/{id}/features` - Get all features for a module
- `POST /api/modules/features` - Create new feature
- `PUT /api/modules/features/{id}` - Update feature
- `DELETE /api/modules/features/{id}` - Delete feature

#### Audit Logs
- `GET /api/auditlogs` - Get audit logs (filterable)

## Project Structure

```
src/
├── Controllers/          # API Controllers
│   ├── AuthController.cs
│   ├── UsersController.cs
│   ├── RolesController.cs
│   ├── ModulesController.cs
│   └── AuditLogsController.cs
├── Data/
│   ├── ApplicationDbContext.cs
│   └── ApplicationDbContextFactory.cs
├── DTOs/                 # Data Transfer Objects
│   ├── UserDTOs.cs
│   ├── AuthDTOs.cs
│   ├── RoleDTOs.cs
│   ├── ModuleDTOs.cs
│   ├── AuditDTOs.cs
│   └── CommonDTOs.cs
├── Extensions/
│   ├── ServiceExtensions.cs
│   └── MiddlewareExtensions.cs
├── Mappings/
│   └── AutoMapperProfile.cs
├── Middleware/
│   ├── ErrorHandlingMiddleware.cs
│   ├── RequestLoggingMiddleware.cs
│   ├── AuthenticationMiddleware.cs
│   └── RateLimitingMiddleware.cs
├── Models/               # Domain Models
│   ├── User.cs
│   ├── Role.cs
│   ├── Module.cs
│   ├── Feature.cs
│   ├── Permission.cs
│   ├── AuditLog.cs
│   ├── Session.cs
│   ├── RefreshToken.cs
│   └── Enums.cs
├── Services/             # Business Logic
│   ├── IAuthService.cs / AuthService.cs
│   ├── IJwtService.cs / JwtService.cs
│   ├── IUserService.cs / UserService.cs
│   ├── IRoleService.cs / RoleService.cs
│   ├── IModuleService.cs / ModuleService.cs
│   └── IAuditService.cs / AuditService.cs
├── Validators/
│   ├── CreateUserRequestValidator.cs
│   ├── UpdateUserRequestValidator.cs
│   ├── LoginRequestValidator.cs
│   ├── RegisterRequestValidator.cs
│   ├── ChangePasswordRequestValidator.cs
│   ├── CreateRoleRequestValidator.cs
│   ├── UpdateRoleRequestValidator.cs
│   ├── CreateModuleRequestValidator.cs
│   ├── UpdateModuleRequestValidator.cs
│   └── CreateFeatureRequestValidator.cs
├── Configurations/
│   └── JwtSettings.cs
├── Migrations/
│   └── [Entity Framework migrations]
├── Program.cs
├── appsettings.json
├── README.md
├── API_GUIDE.md
└── UserManagement.csproj
```

## Configuration

### Database Connection

Configure your database connection in `appsettings.json`:

```json
"Database": {
  "ConnectionString": "Server=(localdb)\\mssqllocaldb;Database=UserManagementDB;Trusted_Connection=True;TrustServerCertificate=True",
  "Provider": "SqlServer"
}
```

### JWT Settings

```json
"Jwt": {
  "SecretKey": "your-super-secret-key-minimum-256-bits-long",
  "Issuer": "UserManagementSystem",
  "Audience": "UserManagementClients",
  "AccessTokenExpirationMinutes": 60,
  "RefreshTokenExpirationDays": 7
}
```

### Security Settings

```json
"Security": {
  "MaxLoginAttempts": 5,
  "LockoutDurationMinutes": 30,
  "RequireEmailVerification": false,
  "PasswordMinLength": 8,
  "RequireStrongPassword": true
}
```

## Usage Examples

### Register a User

```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "john_doe",
  "email": "john@example.com",
  "password": "SecureP@ss123",
  "firstName": "John",
  "lastName": "Doe",
  "projectId": "project-123"
}
```

### Login

```http
POST /api/auth/login
Content-Type: application/json

{
  "emailOrUsername": "john@example.com",
  "password": "SecureP@ss123",
  "projectId": "project-123"
}
```

### Create a Module

```http
POST /api/modules
Authorization: Bearer {access_token}
Content-Type: application/json

{
  "name": "Inventory Management",
  "description": "Manage inventory items",
  "code": "INV_MGT",
  "order": 1,
  "projectId": "project-123"
}
```

### Create a Role with Permissions

```http
POST /api/roles
Authorization: Bearer {access_token}
Content-Type: application/json

{
  "name": "Inventory Manager",
  "description": "Can manage inventory",
  "priority": 10,
  "permissionIds": ["perm-id-1", "perm-id-2"],
  "projectId": "project-123"
}
```

## Multi-Tenant Architecture

The system supports multi-tenancy through the `ProjectId` field. All users, roles, modules, and features are scoped to a project, allowing the same system to serve multiple applications or projects securely.

## Extensibility

The system is designed to be extensible:

- **Custom Login Methods**: Structure is in place for OAuth, SAML, LDAP integration
- **Custom Permissions**: Add new permission types in the `PermissionType` enum
- **Custom Audit Actions**: Extend `AuditAction` enum for new events
- **Custom Features**: Add any domain-specific features through the module system

## Security Considerations

1. **Never commit** `appsettings.json` with real secrets to version control
2. Use environment variables or Azure Key Vault for production secrets
3. Enable HTTPS in production
4. Regularly rotate JWT secret keys
5. Implement rate limiting for production deployments
6. Enable detailed error logging only in development

## Future Enhancements

- Email verification functionality
- Password reset via email
- Social login integration (Google, GitHub, Microsoft, Facebook)
- LDAP/Active Directory integration
- SAML SSO support
- Biometric authentication
- AI-based anomaly detection
- Custom rule engine for access control
- Integration with third-party IAM tools

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License.

## Support

For support, email stanveer.ahmad002@gmail.com or create an issue in the repository.

## Acknowledgments

- Built following industry best practices
- Inspired by OAuth 2.0 and OWASP security guidelines
- Designed with scalability and maintainability in mind

