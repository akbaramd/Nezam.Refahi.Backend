# Simplified Profile Response Example

## Overview
The enhanced profile system now returns simplified DTOs with only the essential data needed by the client.

## Simplified Response Structure

```json
{
  "isSuccess": true,
  "data": {
    "id": "user-guid",
    "phoneNumber": "+1234567890",
    "fullName": "John Doe",
    "firstName": "John",
    "lastName": "Doe",
    "maskedNationalId": "12******90",
    "isAuthenticated": true,
    "isProfileComplete": true,
    "isPhoneVerified": true,
    "isActive": true,
    "lastLoginAt": "2024-01-01T10:00:00Z",
    "lastAuthenticatedAt": "2024-01-01T10:00:00Z",
    
    "roles": ["Admin", "User"],
    "userRoles": [
      {
        "name": "Admin",
        "isActive": true,
        "isSystemRole": true
      },
      {
        "name": "User", 
        "isActive": true,
        "isSystemRole": true
      }
    ],
    
    "claims": [
      {
        "type": "permission",
        "value": "users.read",
        "isActive": true
      },
      {
        "type": "permission",
        "value": "users.write",
        "isActive": true
      },
      {
        "type": "scope",
        "value": "admin",
        "isActive": true
      }
    ],
    
    "claimValues": ["users.read", "users.write", "admin"],
    "permissions": ["users.read", "users.write"],
    "scopes": ["admin"],
    
    "preferences": [
      {
        "key": "THEME",
        "value": "dark",
        "category": "UI",
        "isActive": true
      },
      {
        "key": "NOTIFICATIONS_EMAIL",
        "value": "true",
        "category": "Notifications",
        "isActive": true
      }
    ],
    
    "preferencesByCategory": {
      "UI": [
        {
          "key": "THEME",
          "value": "dark",
          "category": "UI",
          "isActive": true
        }
      ],
      "Notifications": [
        {
          "key": "NOTIFICATIONS_EMAIL",
          "value": "true",
          "category": "Notifications",
          "isActive": true
        }
      ]
    },
    
    "failedAttempts": 0,
    "isLocked": false,
    "lockReason": null,
    "unlockAt": null,
    "lastDeviceFingerprint": "device-fingerprint",
    "lastIpAddress": "192.168.1.1",
    "lastUserAgent": "Mozilla/5.0..."
  }
}
```

## Key Simplifications

### UserRoleDto - Simplified to 3 properties:
- `name`: Role name
- `isActive`: Whether the role is active
- `isSystemRole`: Whether it's a system role

### UserClaimDto - Simplified to 3 properties:
- `type`: Claim type (permission, role, scope, etc.)
- `value`: Claim value
- `isActive`: Whether the claim is active

### UserPreferenceDto - Simplified to 4 properties:
- `key`: Preference key
- `value`: Preference value
- `category`: Preference category
- `isActive`: Whether the preference is active

## Benefits of Simplification

1. **Reduced Payload Size**: Smaller response size for better performance
2. **Client-Friendly**: Only essential data that clients actually need
3. **Easier to Use**: Simpler structure for frontend development
4. **Better Performance**: Less data to serialize/deserialize
5. **Cleaner API**: Focus on what matters most

## Usage

The simplified profile system maintains all the functionality while providing a much cleaner and more efficient API response. Clients get exactly what they need without unnecessary metadata or complex nested objects.
