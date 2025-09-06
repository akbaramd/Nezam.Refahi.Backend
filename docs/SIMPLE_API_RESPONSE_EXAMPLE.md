# Simple API Response Example

## Professional Response Structure

The API now returns a clean, simple response with separate DTOs for claims and preferences:

```json
{
  "isSuccess": true,
  "data": {
    "id": "user-guid",
    "name": "John Doe",
    "firstName": "John",
    "lastName": "Doe",
    "nationalId": "1234567890",
    "phone": "+1234567890",
    "roles": [
      "Admin",
      "User"
    ],
    "claims": [
      {
        "value": "admin.access"
      },
      {
        "value": "users.read"
      },
      {
        "value": "users.write"
      }
    ],
    "preferences": [
      {
        "key": "language",
        "value": "en"
      },
      {
        "key": "notifications",
        "value": "true"
      },
      {
        "key": "theme",
        "value": "dark"
      }
    ]
  }
}
```

## Key Features

### ✅ **Simple User Data**
- `id`: User identifier
- `name`: Full name
- `firstName`: First name
- `lastName`: Last name
- `nationalId`: National ID
- `phone`: Phone number

### ✅ **Simple Roles**
- Just an array of role names
- No extra metadata

### ✅ **Simple Claims DTO**
- `ClaimDto` with only `value` property
- Clean, minimal structure

### ✅ **Simple Preferences DTO**
- `PreferenceDto` with `key` and `value` properties
- Easy to work with

## Benefits

1. **Clean Structure**: Separate DTOs for different data types
2. **Simple Data**: Only essential information
3. **Professional**: Like major API providers
4. **Easy to Use**: Frontend-friendly structure
5. **Consistent**: Uniform data format
