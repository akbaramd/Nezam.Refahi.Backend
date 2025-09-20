# Nezam Refahi Backend API Documentation

## Overview

This document provides comprehensive information about all available API endpoints in the Nezam Refahi Backend system. The API is organized into modules following a modular architecture pattern.

## Base URL
```
https://api.nezamrefahi.com
```

## Authentication
The API uses JWT Bearer token authentication for protected endpoints.

### Authorization Header
```
Authorization: Bearer {your_jwt_token}
```

## Response Format

All API responses follow a consistent format:

```json
{
  "isSuccess": true,
  "data": {...},
  "errors": [],
  "message": "Success message"
}
```

---

# Authentication Module

Base URL: `/api/v1/auth`

## 1. Send OTP Code

**Endpoint:** `POST /api/v1/auth/otp`

**Description:** Sends an OTP (One-Time Password) code to the user's phone number for authentication purposes.

### Request Body
```json
{
  "nationalCode": "1234567890",
  "purpose": "login",
  "deviceId": "device-123",
  "scope": "app"
}
```

### Request Fields
| Field | Type | Required | Description | Default |
|-------|------|----------|-------------|---------|
| nationalCode | string | Yes | User's national ID code | - |
| purpose | string | No | Purpose of OTP (login, register) | "login" |
| deviceId | string | No | Unique device identifier | Extracted from header |
| scope | string | No | Application scope (app, panel) | "app" |

### Response Model
```json
{
  "isSuccess": true,
  "data": {
    "expiryMinutes": 5,
    "maskedPhoneNumber": "09*****1234",
    "isRegistered": true,
    "isLocked": false,
    "remainingLockoutMinutes": 0,
    "challengeId": "uuid-string"
  }
}
```

### Response Fields
| Field | Type | Description |
|-------|------|-------------|
| expiryMinutes | int | OTP expiry time in minutes |
| maskedPhoneNumber | string | Masked phone number |
| isRegistered | bool | Whether user is already registered |
| isLocked | bool | Whether phone is locked due to failed attempts |
| remainingLockoutMinutes | int | Remaining lockout time if locked |
| challengeId | string | Unique challenge identifier |

---

## 2. Verify OTP Code

**Endpoint:** `POST /api/v1/auth/otp/verify`

**Description:** Verifies an OTP code and returns authentication tokens if successful.

### Request Body
```json
{
  "challengeId": "uuid-string",
  "otpCode": "123456",
  "scope": "app"
}
```

### Request Fields
| Field | Type | Required | Description | Default |
|-------|------|----------|-------------|---------|
| challengeId | string | Yes | Challenge ID from SendOTP response | - |
| otpCode | string | Yes | 6-digit OTP code | - |
| purpose | string | No | Purpose of verification | "login" |
| deviceId | string | No | Device identifier | Extracted from header |
| scope | string | No | Application scope (app, panel) | "app" |

### Response Model
```json
{
  "isSuccess": true,
  "data": {
    "userId": "uuid",
    "accessToken": "jwt-token",
    "refreshToken": "refresh-token",
    "expiryMinutes": 60,
    "isRegistered": true,
    "requiresRegistrationCompletion": false
  }
}
```

### Response Fields
| Field | Type | Description |
|-------|------|-------------|
| userId | Guid | User's unique identifier |
| accessToken | string | JWT access token |
| refreshToken | string | Refresh token for token renewal |
| expiryMinutes | int | Access token expiry time |
| isRegistered | bool | Whether user is registered |
| requiresRegistrationCompletion | bool | Whether registration completion needed |

---

## 3. Get Current User Profile

**Endpoint:** `GET /api/v1/auth/profile`

**Description:** Retrieves the profile information of the currently authenticated user.

**Authentication:** Required

### Response Model
```json
{
  "isSuccess": true,
  "data": {
    "id": "uuid",
    "name": "John Doe",
    "firstName": "John",
    "lastName": "Doe",
    "nationalId": "1234567890",
    "phone": "09123456789",
    "roles": ["User", "Admin"],
    "claims": [
      {
        "type": "permission",
        "value": "read:users"
      }
    ],
    "preferences": [
      {
        "key": "language",
        "value": "fa",
        "category": "Localization"
      }
    ]
  }
}
```

---

## 4. User Logout

**Endpoint:** `POST /api/v1/auth/logout`

**Description:** Logs out the user and revokes both access and refresh tokens.

**Authentication:** Required

### Request Body
```json
{
  "refreshToken": "refresh-token-string"
}
```

### Request Fields
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| refreshToken | string | No | Refresh token to revoke |

### Response Model
```json
{
  "isSuccess": true,
  "data": {
    "message": "Logged out successfully"
  }
}
```

---

# User Management Module

Base URL: `/api/v1/users`

**Authentication:** All endpoints require authentication.

## 1. Get Users (Paginated)

**Endpoint:** `GET /api/v1/users`

**Description:** Returns a paginated list of users with optional search filter.

### Query Parameters
| Parameter | Type | Required | Description | Default |
|-----------|------|----------|-------------|---------|
| pageNumber | int | No | Page number (1-based) | 1 |
| pageSize | int | No | Number of items per page | 20 |
| search | string | No | Search term for filtering | - |

### Response Model
```json
{
  "isSuccess": true,
  "data": {
    "items": [
      {
        "id": "uuid",
        "firstName": "John",
        "lastName": "Doe",
        "nationalId": "1234567890",
        "phoneNumber": "09123456789",
        "isPhoneVerified": true,
        "phoneVerifiedAt": "2023-01-01T00:00:00Z",
        "isActive": true,
        "lastLoginAt": "2023-01-01T00:00:00Z",
        "lastAuthenticatedAt": "2023-01-01T00:00:00Z",
        "failedAttempts": 0,
        "lockedAt": null,
        "lockReason": null,
        "unlockAt": null,
        "lastIpAddress": "192.168.1.1",
        "lastUserAgent": "Mozilla/5.0...",
        "lastDeviceFingerprint": "device-fingerprint",
        "createdAtUtc": "2023-01-01T00:00:00Z",
        "createdBy": "system",
        "updatedAtUtc": "2023-01-01T00:00:00Z",
        "updatedBy": "system"
      }
    ],
    "pageNumber": 1,
    "pageSize": 20,
    "totalCount": 100,
    "totalPages": 5,
    "hasPreviousPage": false,
    "hasNextPage": true
  }
}
```

---

## 2. Get User Detail

**Endpoint:** `GET /api/v1/users/{id}`

**Description:** Returns full user DTO with roles, claims, preferences and tokens.

### Path Parameters
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| id | Guid | Yes | User's unique identifier |

### Response Model
```json
{
  "isSuccess": true,
  "data": {
    "id": "uuid",
    "firstName": "John",
    "lastName": "Doe",
    "nationalId": "1234567890",
    "phoneNumber": "09123456789",
    "isPhoneVerified": true,
    "phoneVerifiedAt": "2023-01-01T00:00:00Z",
    "isActive": true,
    "lastLoginAt": "2023-01-01T00:00:00Z",
    "roles": [
      {
        "id": "uuid",
        "roleName": "Admin",
        "assignedAt": "2023-01-01T00:00:00Z"
      }
    ],
    "claims": [
      {
        "id": "uuid",
        "type": "permission",
        "value": "read:users"
      }
    ],
    "preferences": [
      {
        "id": "uuid",
        "key": "language",
        "value": "fa",
        "category": "Localization"
      }
    ]
  }
}
```

---

## 3. Create User

**Endpoint:** `POST /api/v1/users`

**Description:** Creates a new user with the provided information.

### Request Body
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "09123456789",
  "nationalId": "1234567890"
}
```

### Request Fields
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| firstName | string | Yes | User's first name |
| lastName | string | Yes | User's last name |
| phoneNumber | string | Yes | User's phone number |
| nationalId | string | Yes | User's national ID |

### Response Model
```json
{
  "isSuccess": true,
  "data": "uuid"
}
```

---

## 4. Update User

**Endpoint:** `PUT /api/v1/users/{id}`

**Description:** Updates an existing user with the provided information.

### Path Parameters
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| id | Guid | Yes | User's unique identifier |

### Request Body
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "09123456789",
  "nationalId": "1234567890"
}
```

### Response Model
```json
{
  "isSuccess": true,
  "data": null
}
```

---

## 5. Delete User

**Endpoint:** `DELETE /api/v1/users/{id}`

**Description:** Deletes a user. By default performs soft delete, but can perform hard delete if specified.

### Path Parameters
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| id | Guid | Yes | User's unique identifier |

### Query Parameters
| Parameter | Type | Required | Description | Default |
|-----------|------|----------|-------------|---------|
| softDelete | bool | No | Whether to perform soft delete | true |
| deleteReason | string | No | Reason for deletion | null |

### Response Model
```json
{
  "isSuccess": true,
  "data": null
}
```

---

# Settings Management Module

Base URL: `/api/v1/settings`

## 1. Create Section

**Endpoint:** `POST /api/v1/settings/sections`

**Description:** Create a new settings section.

### Request Body
```json
{
  "name": "Security",
  "description": "Security related settings",
  "displayOrder": 1
}
```

### Request Fields
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| name | string | Yes | Section name |
| description | string | Yes | Section description |
| displayOrder | int | No | Display order for sorting |

---

## 2. Create Category

**Endpoint:** `POST /api/v1/settings/categories`

**Description:** Create a new settings category.

### Request Body
```json
{
  "name": "Authentication",
  "description": "Authentication settings",
  "sectionId": "uuid",
  "displayOrder": 1
}
```

---

## 3. Set Setting

**Endpoint:** `POST /api/v1/settings`

**Description:** Create or update a setting.

### Request Body
```json
{
  "key": "max-login-attempts",
  "value": "5",
  "description": "Maximum login attempts before lockout",
  "categoryId": "uuid",
  "type": "Integer",
  "isRequired": true,
  "displayOrder": 1
}
```

---

## 4. Update Setting

**Endpoint:** `PUT /api/v1/settings/{settingId}`

**Description:** Update an existing setting value.

### Request Body
```json
{
  "value": "10",
  "description": "Updated description"
}
```

---

## 5. Bulk Update Settings

**Endpoint:** `PUT /api/v1/settings/bulk`

**Description:** Bulk update multiple settings.

### Request Body
```json
{
  "updates": [
    {
      "settingId": "uuid",
      "value": "new-value"
    },
    {
      "settingId": "uuid",
      "value": "another-value"
    }
  ]
}
```

### Response Model
```json
{
  "isSuccess": true,
  "data": {
    "successfulUpdates": [
      {
        "settingId": "uuid",
        "key": "setting-key",
        "oldValue": "old",
        "newValue": "new"
      }
    ],
    "failedUpdates": [
      {
        "settingId": "uuid",
        "key": "setting-key",
        "error": "Validation failed"
      }
    ],
    "totalProcessed": 2,
    "successCount": 1,
    "failureCount": 1
  }
}
```

---

## 6. Get Settings

**Endpoint:** `GET /api/v1/settings`

**Description:** Get settings with filters and pagination.

### Query Parameters
| Parameter | Type | Required | Description | Default |
|-----------|------|----------|-------------|---------|
| sectionName | string | No | Filter by section name | - |
| categoryName | string | No | Filter by category name | - |
| searchTerm | string | No | Search term for key | - |
| type | string | No | Filter by setting type | - |
| onlyActive | bool | No | Include only active settings | true |
| pageNumber | int | No | Page number (1-based) | 1 |
| pageSize | int | No | Page size | 20 |
| sortBy | string | No | Sort field | "DisplayOrder" |
| sortDescending | bool | No | Sort descending | false |

### Response Model
```json
{
  "isSuccess": true,
  "data": {
    "settings": [
      {
        "id": "uuid",
        "key": "max-login-attempts",
        "value": "5",
        "description": "Maximum login attempts",
        "type": "Integer",
        "isRequired": true,
        "displayOrder": 1,
        "categoryInfo": {
          "id": "uuid",
          "name": "Authentication"
        },
        "sectionInfo": {
          "id": "uuid",
          "name": "Security"
        }
      }
    ],
    "pagination": {
      "pageNumber": 1,
      "pageSize": 20,
      "totalPages": 5,
      "hasPreviousPage": false,
      "hasNextPage": true
    },
    "totalCount": 100,
    "filters": {
      "sectionName": null,
      "categoryName": null,
      "searchTerm": null,
      "type": null,
      "onlyActive": true
    }
  }
}
```

---

## 7. Get Settings Organized

**Endpoint:** `GET /api/v1/settings/organized`

**Description:** Get settings organized by section and category.

### Query Parameters
| Parameter | Type | Required | Description | Default |
|-----------|------|----------|-------------|---------|
| sectionName | string | No | Filter by section name | - |
| onlyActive | bool | No | Include only active settings | true |

### Response Model
```json
{
  "isSuccess": true,
  "data": {
    "sections": [
      {
        "name": "Security",
        "description": "Security settings",
        "displayOrder": 1,
        "categories": [
          {
            "name": "Authentication",
            "description": "Auth settings",
            "displayOrder": 1,
            "settings": [
              {
                "key": "max-login-attempts",
                "value": "5",
                "description": "Max attempts",
                "type": "Integer"
              }
            ]
          }
        ]
      }
    ]
  }
}
```

---

## 8. Get Setting by Key

**Endpoint:** `GET /api/v1/settings/{key}`

**Description:** Get a specific setting by its key.

### Path Parameters
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| key | string | Yes | Setting key (alphanumeric and hyphens only) |

### Query Parameters
| Parameter | Type | Required | Description | Default |
|-----------|------|----------|-------------|---------|
| includeMetadata | bool | No | Include category/section info | false |

### Response Model
```json
{
  "isSuccess": true,
  "data": {
    "found": true,
    "setting": {
      "key": "max-login-attempts",
      "value": "5",
      "description": "Maximum login attempts",
      "type": "Integer",
      "isRequired": true
    },
    "category": {
      "name": "Authentication",
      "description": "Auth settings"
    },
    "section": {
      "name": "Security",
      "description": "Security settings"
    }
  }
}
```

---

# Error Handling

## Standard Error Responses

### 400 Bad Request
```json
{
  "isSuccess": false,
  "data": null,
  "errors": [
    "Validation error message"
  ],
  "message": "Bad Request"
}
```

### 401 Unauthorized
```json
{
  "isSuccess": false,
  "data": null,
  "errors": [
    "Authentication required"
  ],
  "message": "Unauthorized"
}
```

### 404 Not Found
```json
{
  "isSuccess": false,
  "data": null,
  "errors": [
    "Resource not found"
  ],
  "message": "Not Found"
}
```

### 500 Internal Server Error
```json
{
  "isSuccess": false,
  "data": null,
  "errors": [
    "Internal server error occurred"
  ],
  "message": "Internal Server Error"
}
```

---

# Common Data Types

## UserDto
```json
{
  "id": "uuid",
  "firstName": "string",
  "lastName": "string",
  "nationalId": "string",
  "phoneNumber": "string",
  "isPhoneVerified": "boolean",
  "phoneVerifiedAt": "datetime?",
  "isActive": "boolean",
  "lastLoginAt": "datetime?",
  "lastAuthenticatedAt": "datetime?",
  "failedAttempts": "integer",
  "lockedAt": "datetime?",
  "lockReason": "string?",
  "unlockAt": "datetime?",
  "lastIpAddress": "string?",
  "lastUserAgent": "string?",
  "lastDeviceFingerprint": "string?",
  "createdAtUtc": "datetime",
  "createdBy": "string?",
  "updatedAtUtc": "datetime?",
  "updatedBy": "string?"
}
```

## PaginatedResult<T>
```json
{
  "items": ["array of T"],
  "pageNumber": "integer",
  "pageSize": "integer",
  "totalCount": "integer",
  "totalPages": "integer",
  "hasPreviousPage": "boolean",
  "hasNextPage": "boolean"
}
```

## ClaimDto
```json
{
  "type": "string",
  "value": "string"
}
```

## UserPreferenceDto
```json
{
  "key": "string",
  "value": "string",
  "category": "string"
}
```

---

# Rate Limiting

- OTP requests: 5 requests per phone number per 10 minutes
- Login attempts: 5 attempts per phone number per 30 minutes
- General API: 1000 requests per hour per authenticated user

---

# Versioning

The API uses URL versioning with the format `/api/v{version}/` where version is the major version number (e.g., v1, v2).

Current version: **v1**

---

# Support

For API support and questions, please contact the development team or refer to the project documentation.