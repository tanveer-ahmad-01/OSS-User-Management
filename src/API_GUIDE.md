# API Integration Guide

This guide provides detailed information for integrating the User Management Plugin System (UMPS) into your application.

## Table of Contents

1. [Authentication Flow](#authentication-flow)
2. [API Usage Patterns](#api-usage-patterns)
3. [Error Handling](#error-handling)
4. [Best Practices](#best-practices)
5. [Integration Examples](#integration-examples)

## Authentication Flow

### 1. User Registration

First, register a new user in your system:

```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "johndoe",
  "email": "john@example.com",
  "password": "SecureP@ssw0rd123!",
  "firstName": "John",
  "lastName": "Doe",
  "projectId": "your-project-id"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Registration successful",
  "data": {
    "id": "guid-here",
    "username": "johndoe",
    "email": "john@example.com",
    "status": 1,
    "roles": []
  }
}
```

### 2. User Login

Authenticate and receive access tokens:

```http
POST /api/auth/login
Content-Type: application/json

{
  "emailOrUsername": "john@example.com",
  "password": "SecureP@ssw0rd123!",
  "projectId": "your-project-id"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "base64-refresh-token",
    "expiresAt": "2024-01-01T12:00:00Z",
    "user": { /* user details */ }
  }
}
```

### 3. Using Access Tokens

Include the access token in the Authorization header for all protected endpoints:

```http
GET /api/users
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### 4. Refreshing Tokens

When the access token expires, refresh it:

```http
POST /api/auth/refresh-token
Content-Type: application/json

{
  "refreshToken": "base64-refresh-token"
}
```

## API Usage Patterns

### CRUD Operations

#### Create User

```http
POST /api/users
Authorization: Bearer {access_token}
Content-Type: application/json

{
  "username": "newuser",
  "email": "newuser@example.com",
  "password": "Password123!",
  "firstName": "New",
  "lastName": "User",
  "roleIds": ["role-guid-1", "role-guid-2"],
  "projectId": "project-123"
}
```

#### Get All Users (Paginated)

```http
GET /api/users?pageNumber=1&pageSize=10&searchTerm=john&projectId=project-123
Authorization: Bearer {access_token}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "users": [ /* array of users */ ],
    "totalCount": 100,
    "pageNumber": 1,
    "pageSize": 10
  }
}
```

#### Update User

```http
PUT /api/users/{userId}
Authorization: Bearer {access_token}
Content-Type: application/json

{
  "firstName": "Updated",
  "lastName": "Name",
  "phoneNumber": "+1234567890"
}
```

#### Delete User

```http
DELETE /api/users/{userId}
Authorization: Bearer {access_token}
```

### Role Management

#### Create Role

```http
POST /api/roles
Authorization: Bearer {access_token}
Content-Type: application/json

{
  "name": "Content Editor",
  "description": "Can create and edit content",
  "priority": 5,
  "permissionIds": ["perm-id-1", "perm-id-2"],
  "projectId": "project-123"
}
```

#### Assign Roles to User

```http
POST /api/users/{userId}/assign-roles
Authorization: Bearer {access_token}
Content-Type: application/json

["role-id-1", "role-id-2"]
```

#### Get Role with Permissions

```http
GET /api/roles/{roleId}
Authorization: Bearer {access_token}
```

**Response includes all permissions for the role.**

### Module Management

#### Create Module

```http
POST /api/modules
Authorization: Bearer {access_token}
Content-Type: application/json

{
  "name": "Content Management",
  "description": "Manage articles and posts",
  "code": "CMS",
  "order": 1,
  "projectId": "project-123"
}
```

#### Create Submodule

```http
POST /api/modules
Authorization: Bearer {access_token}
Content-Type: application/json

{
  "name": "Article Editor",
  "description": "Edit articles",
  "code": "ART_EDIT",
  "parentModuleId": "parent-module-guid",
  "order": 1,
  "projectId": "project-123"
}
```

#### Create Feature

```http
POST /api/modules/features
Authorization: Bearer {access_token}
Content-Type: application/json

{
  "name": "Publish Article",
  "description": "Ability to publish articles",
  "code": "PUB_ART",
  "moduleId": "module-guid",
  "projectId": "project-123"
}
```

**Note:** Creating a feature automatically creates all permission types (Read, Write, Delete, Execute) for that feature.

## Error Handling

### Standard Error Response

All API errors follow this structure:

```json
{
  "success": false,
  "message": "Error description",
  "errors": [
    "Detailed error message 1",
    "Detailed error message 2"
  ]
}
```

### HTTP Status Codes

| Code | Meaning | When Used |
|------|---------|-----------|
| 200 | OK | Successful GET, PUT requests |
| 201 | Created | Successful POST requests |
| 400 | Bad Request | Invalid request data |
| 401 | Unauthorized | Invalid or missing token |
| 403 | Forbidden | Insufficient permissions |
| 404 | Not Found | Resource doesn't exist |
| 409 | Conflict | Resource already exists |
| 500 | Internal Server Error | Server error |

### Common Error Scenarios

#### Authentication Errors

**Invalid Credentials:**
```json
{
  "success": false,
  "message": "Invalid credentials",
  "errors": ["Invalid credentials"]
}
```
**Status:** 401 Unauthorized

**Expired Token:**
```json
{
  "success": false,
  "message": "Token has expired",
  "errors": ["Token has expired"]
}
```
**Status:** 401 Unauthorized

#### Validation Errors

**Duplicate Email:**
```json
{
  "success": false,
  "message": "User already exists",
  "errors": ["User already exists"]
}
```
**Status:** 409 Conflict

**Not Found:**
```json
{
  "success": false,
  "message": "User not found",
  "errors": ["User not found"]
}
```
**Status:** 404 Not Found

## Best Practices

### 1. Token Management

- Store tokens securely (use secure storage in mobile apps, HttpOnly cookies in web)
- Always refresh tokens before they expire
- Implement automatic token refresh logic
- Revoke tokens on logout or suspicious activity

### 2. Error Handling

- Always check the `success` field in responses
- Implement retry logic for transient errors (500 status)
- Handle 401 errors by prompting re-authentication
- Log errors for debugging but don't expose sensitive details to users

### 3. Performance

- Use pagination for list endpoints
- Implement client-side caching for static data
- Use appropriate page sizes (10-50 items)
- Filter data on the server side when possible

### 4. Security

- Never store passwords in plain text
- Always use HTTPS in production
- Implement rate limiting on authentication endpoints
- Validate all input on both client and server
- Sanitize user inputs

### 5. Audit Trails

All operations are automatically logged. Query audit logs for compliance:

```http
GET /api/auditlogs?userId={guid}&action=1&startDate=2024-01-01
Authorization: Bearer {access_token}
```

## Integration Examples

### JavaScript/TypeScript (Fetch API)

```javascript
class UMPSClient {
  constructor(baseUrl, projectId) {
    this.baseUrl = baseUrl;
    this.projectId = projectId;
    this.accessToken = null;
    this.refreshToken = null;
  }

  async login(email, password) {
    const response = await fetch(`${this.baseUrl}/api/auth/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        emailOrUsername: email,
        password: password,
        projectId: this.projectId
      })
    });

    const data = await response.json();
    
    if (data.success) {
      this.accessToken = data.data.accessToken;
      this.refreshToken = data.data.refreshToken;
      return data.data;
    }
    
    throw new Error(data.message);
  }

  async getUsers(pageNumber = 1, pageSize = 10) {
    const response = await fetch(
      `${this.baseUrl}/api/users?pageNumber=${pageNumber}&pageSize=${pageSize}&projectId=${this.projectId}`,
      {
        headers: {
          'Authorization': `Bearer ${this.accessToken}`
        }
      }
    );

    const data = await response.json();
    return data.data;
  }

  async refreshAccessToken() {
    const response = await fetch(`${this.baseUrl}/api/auth/refresh-token`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        refreshToken: this.refreshToken
      })
    });

    const data = await response.json();
    
    if (data.success) {
      this.accessToken = data.data.accessToken;
      this.refreshToken = data.data.refreshToken;
      return data.data;
    }
    
    throw new Error(data.message);
  }
}

// Usage
const client = new UMPSClient('https://api.example.com', 'project-123');
await client.login('user@example.com', 'password');
const users = await client.getUsers();
```

### C# (.NET)

```csharp
public class UMPSClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly string _projectId;
    private string? _accessToken;
    private string? _refreshToken;

    public UMPSClient(string baseUrl, string projectId)
    {
        _baseUrl = baseUrl;
        _projectId = projectId;
        _httpClient = new HttpClient();
    }

    public async Task<LoginResponse> LoginAsync(string email, string password)
    {
        var request = new LoginRequest
        {
            EmailOrUsername = email,
            Password = password,
            ProjectId = _projectId
        };

        var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/auth/login", request);
        var data = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>();

        if (data?.Success == true && data.Data != null)
        {
            _accessToken = data.Data.AccessToken;
            _refreshToken = data.Data.RefreshToken;
            return data.Data;
        }

        throw new Exception(data?.Message ?? "Login failed");
    }

    public async Task<UserListResponse> GetUsersAsync(int pageNumber = 1, int pageSize = 10)
    {
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);

        var response = await _httpClient.GetAsync(
            $"{_baseUrl}/api/users?pageNumber={pageNumber}&pageSize={pageSize}&projectId={_projectId}");
        
        var data = await response.Content.ReadFromJsonAsync<ApiResponse<UserListResponse>>();
        return data?.Data ?? new UserListResponse();
    }
}
```

### Python

```python
import requests
from typing import Optional

class UMPSClient:
    def __init__(self, base_url: str, project_id: str):
        self.base_url = base_url
        self.project_id = project_id
        self.access_token: Optional[str] = None
        self.refresh_token: Optional[str] = None
    
    def login(self, email: str, password: str):
        response = requests.post(
            f"{self.base_url}/api/auth/login",
            json={
                "emailOrUsername": email,
                "password": password,
                "projectId": self.project_id
            }
        )
        data = response.json()
        
        if data["success"]:
            self.access_token = data["data"]["accessToken"]
            self.refresh_token = data["data"]["refreshToken"]
            return data["data"]
        else:
            raise Exception(data["message"])
    
    def get_users(self, page_number: int = 1, page_size: int = 10):
        headers = {"Authorization": f"Bearer {self.access_token}"}
        response = requests.get(
            f"{self.base_url}/api/users",
            params={
                "pageNumber": page_number,
                "pageSize": page_size,
                "projectId": self.project_id
            },
            headers=headers
        )
        data = response.json()
        return data["data"]

# Usage
client = UMPSClient("https://api.example.com", "project-123")
client.login("user@example.com", "password")
users = client.get_users()
```

## Additional Resources

- Swagger UI: Access at `https://your-api-url/swagger` when running
- API Version: v1
- Base URL: Configure in your integration
- Support: support@usermanagement.com

