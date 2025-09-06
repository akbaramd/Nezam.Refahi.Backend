# Swagger Authorization Operation Filter

## Overview

This custom operation filter provides selective authorization display in Swagger UI, showing the lock icon only for endpoints that require authentication and hiding it for anonymous endpoints.

## Features

### ✅ **Selective Authorization Display**
- **Authorized Endpoints**: Show lock icon (🔒) and require Bearer token
- **Anonymous Endpoints**: No lock icon (🌐) and publicly accessible
- **Smart Detection**: Automatically detects authorization requirements based on attributes and Minimal API metadata

### ✅ **Multi-Framework Support**
- **Controller APIs**: Supports `[Authorize]` and `[AllowAnonymous]` attributes
- **Minimal APIs**: Supports `.RequireAuthorization()` method calls
- **Mixed Projects**: Works with both approaches in the same project

### ✅ **Attribute Priority Logic**
1. **Method-level `[AllowAnonymous]`** - Overrides everything (endpoint is public)
2. **Method-level `[Authorize]`** - Requires authentication
3. **Minimal API `.RequireAuthorization()`** - Requires authentication
4. **Controller-level `[AllowAnonymous]`** - Controller allows anonymous access by default
5. **Controller-level `[Authorize]`** - Controller requires authentication by default
6. **No attributes/metadata** - Assumes no authorization required (public)

### ✅ **Visual Indicators**
- **🔒 Lock Icon**: For endpoints requiring authentication
- **🌐 Globe Icon**: For publicly accessible endpoints
- **Description Updates**: Automatically adds authorization status to endpoint descriptions

## Implementation

### **AuthorizationOperationFilter.cs**
```csharp
public class AuthorizationOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Smart detection of authorization requirements
        // Applies appropriate security requirements
        // Adds visual indicators to descriptions
    }
}
```

### **Integration in NezamWebApiModule.cs**
```csharp
// Add custom operation filter for selective authorization display
options.OperationFilter<AuthorizationOperationFilter>();
```

## Usage Examples

### **Authorized Endpoint**
```csharp
[HttpGet("profile")]
[Authorize] // This will show lock icon in Swagger
public async Task<ActionResult> GetProfile()
{
    // Endpoint requires authentication
}
```

### **Anonymous Endpoint**
```csharp
[HttpGet("public")]
[AllowAnonymous] // This will NOT show lock icon in Swagger
public async Task<ActionResult> GetPublicData()
{
    // Endpoint is publicly accessible
}
```

### **Controller-Level Authorization**
```csharp
[ApiController]
[Authorize] // All endpoints require auth by default
public class SecureController : ControllerBase
{
    [HttpGet("data")]
    // This will show lock icon (inherits from controller)
    public async Task<ActionResult> GetData() { }
    
    [HttpGet("public")]
    [AllowAnonymous] // This will NOT show lock icon (overrides controller)
    public async Task<ActionResult> GetPublicData() { }
}
```

## Benefits

1. **Clear Visual Distinction**: Easy to see which endpoints require authentication
2. **Better Developer Experience**: No confusion about endpoint accessibility
3. **Automatic Detection**: No manual configuration needed
4. **Consistent Behavior**: Works with all authorization patterns
5. **Professional Appearance**: Clean, organized Swagger documentation

## Swagger UI Result

- **Authorized endpoints** will show:
  - 🔒 Lock icon
  - "Authorize" button functionality
  - Bearer token requirement
  - Description: "🔒 This endpoint requires authentication."

- **Anonymous endpoints** will show:
  - 🌐 Globe icon (in description)
  - No "Authorize" button
  - No security requirements
  - Description: "🌐 This endpoint is publicly accessible."

This implementation follows the pattern described in the Medium article and provides a professional, user-friendly Swagger documentation experience.
