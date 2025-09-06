# Production-Grade Token System

This document describes the production-grade token system implemented following DDD + EF Core best practices.

## Overview

The token system implements a secure, scalable authentication mechanism with:

- **JWT Access Tokens**: 5-15 minute TTL, stateless validation
- **Refresh Token Rotation**: 30-90 day TTL with one-time use and reuse detection
- **Proper Hashing**: HMAC-SHA256 with salt and pepper for refresh tokens
- **Session Management**: Device binding, IP tracking, and session families
- **Security Features**: Reuse detection, emergency revocation, idle timeout

## Architecture

### Core Components

1. **ITokenService** - Main service interface for token operations
2. **UserToken Entity** - Domain entity with proper hashing and validation
3. **TokenService** - Production implementation with RS256/ES256 support
4. **UserTokenRepository** - Repository with comprehensive token management
5. **TokenCleanupService** - Background service for maintenance

### Token Types

#### Access Tokens (JWT)
- **Lifespan**: 5-15 minutes (configurable)
- **Storage**: Stateless (not stored in DB)
- **Revocation**: JTI-based deny list in distributed cache
- **Signing**: RS256/ES256 (production) or HS256 (development)
- **Claims**: sub, jti, iat, exp, roles, minimal user data

#### Refresh Tokens
- **Lifespan**: 30-90 days (configurable)
- **Storage**: Hashed in database with salt and pepper
- **Rotation**: One-time use with automatic rotation
- **Binding**: Device fingerprint, IP address, user agent
- **Security**: Reuse detection, session family management

## Security Features

### Token Hashing
```csharp
// Refresh tokens are hashed using HMAC-SHA256
var hashedToken = HMACSHA256(pepper + salt, rawToken);
```

### Reuse Detection
- Each refresh token can only be used once
- Reuse triggers session family revocation
- Compromised sessions are immediately invalidated

### Device Binding
- Refresh tokens are bound to device fingerprints
- IP address and user agent validation
- Prevents token theft and unauthorized access

### Emergency Revocation
- JWT tokens can be revoked via distributed cache
- User-level token revocation (logout all devices)
- Device-level token revocation (logout specific device)

## Configuration

### JWT Configuration
```json
{
  "Jwt": {
    "Issuer": "Nezam.Refahi",
    "Audience": "Nezam.Refahi.Users",
    "Key": "your-symmetric-key-for-development",
    "RsaPrivateKeyPath": "path/to/private-key.pem"
  },
  "TokenService": {
    "Pepper": "your-pepper-for-token-hashing"
  }
}
```

### Database Configuration
The system requires the following database schema updates:

```sql
-- New columns for UserTokens table
ALTER TABLE [identity].[UserTokens] ADD
    [DeviceFingerprint] NVARCHAR(256) NULL,
    [UserAgent] NVARCHAR(512) NULL,
    [SessionFamilyId] UNIQUEIDENTIFIER NULL,
    [ParentTokenId] UNIQUEIDENTIFIER NULL,
    [LastUsedAt] DATETIME2 NULL,
    [Salt] NVARCHAR(64) NULL;

-- New indexes for performance
CREATE INDEX IX_UserTokens_SessionFamilyId ON [identity].[UserTokens] ([SessionFamilyId]) 
    WHERE [SessionFamilyId] IS NOT NULL;

CREATE INDEX IX_UserTokens_DeviceBinding ON [identity].[UserTokens] ([UserId], [DeviceFingerprint], [TokenType]) 
    WHERE [DeviceFingerprint] IS NOT NULL;

CREATE INDEX IX_UserTokens_LastUsedAt ON [identity].[UserTokens] ([LastUsedAt]) 
    WHERE [LastUsedAt] IS NOT NULL;

CREATE INDEX IX_UserTokens_ParentTokenId ON [identity].[UserTokens] ([ParentTokenId]) 
    WHERE [ParentTokenId] IS NOT NULL;
```

## Usage Examples

### Generate Tokens
```csharp
// Generate access token
var accessToken = _tokenService.GenerateAccessToken(user, out var jwtId, 15);

// Generate refresh token
var (rawToken, hashedToken, tokenId) = await _tokenService.GenerateRefreshTokenAsync(
    userId, deviceFingerprint, ipAddress, userAgent, 30);
```

### Validate and Rotate Refresh Token
```csharp
var result = await _tokenService.ValidateAndRotateRefreshTokenAsync(
    refreshToken, deviceFingerprint, ipAddress, userAgent);

if (result.IsValid)
{
    // Use new tokens
    var newAccessToken = result.NewAccessToken;
    var newRefreshToken = result.NewRefreshToken;
}
```

### Revoke Tokens
```csharp
// Revoke specific JWT
await _tokenService.RevokeJwtAsync(jwtId, remainingLifetime);

// Revoke all user tokens
await _tokenService.RevokeAllUserRefreshTokensAsync(userId);

// Revoke device tokens
await _tokenService.RevokeDeviceRefreshTokensAsync(userId, deviceFingerprint);
```

## API Endpoints

### Refresh Token Endpoint
```http
POST /api/auth/refresh
Content-Type: application/json

{
  "refreshToken": "your-refresh-token",
  "deviceFingerprint": "device-fingerprint",
  "ipAddress": "192.168.1.1",
  "userAgent": "Mozilla/5.0..."
}
```

### Response
```json
{
  "success": true,
  "data": {
    "accessToken": "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "new-refresh-token",
    "expiryMinutes": 15,
    "userId": "user-guid",
    "isSessionCompromised": false
  },
  "message": "Tokens refreshed successfully"
}
```

## Monitoring and Maintenance

### Token Statistics
```csharp
var stats = await _userTokenRepository.GetTokenStatisticsAsync(userId);
// Returns: active tokens, revoked tokens, expired tokens, etc.
```

### Background Cleanup
The `TokenCleanupService` runs every hour to:
- Remove expired tokens
- Clean up idle tokens (7+ days unused)
- Remove old revoked tokens (30+ days old)

### Logging
The system provides comprehensive logging for:
- Token generation and validation
- Security violations (reuse detection)
- Cleanup operations
- Performance metrics

## Security Considerations

1. **Never store raw refresh tokens** - Always hash them
2. **Use strong pepper values** - Store in secure configuration
3. **Implement proper device binding** - Validate device fingerprints
4. **Monitor for reuse attempts** - Log and alert on suspicious activity
5. **Regular cleanup** - Remove old tokens to prevent data bloat
6. **Use HTTPS only** - Never transmit tokens over unencrypted connections

## Migration Guide

### From Old System
1. Update database schema with new columns
2. Migrate existing tokens (if any)
3. Update application code to use new ITokenService
4. Configure JWT settings and pepper
5. Deploy and test

### Testing
```csharp
// Test token generation
var accessToken = _tokenService.GenerateAccessToken(user, out var jwtId);

// Test token validation
var isValid = _tokenService.ValidateAccessToken(accessToken, out var validatedJwtId, out var userId);

// Test refresh token rotation
var result = await _tokenService.ValidateAndRotateRefreshTokenAsync(refreshToken);
```

## Performance Considerations

- **Database Indexes**: Optimized for common query patterns
- **Caching**: JWT deny list uses distributed cache
- **Batch Operations**: Bulk token revocation and cleanup
- **Connection Pooling**: Efficient database connection usage
- **Async Operations**: All operations are async for scalability

## Troubleshooting

### Common Issues

1. **Token validation fails**: Check JWT configuration and signing keys
2. **Refresh token not found**: Verify token hashing and pepper configuration
3. **Device binding fails**: Ensure device fingerprint is consistent
4. **Performance issues**: Check database indexes and query patterns

### Debug Logging
Enable debug logging to see detailed token operations:
```json
{
  "Logging": {
    "LogLevel": {
      "Nezam.Refahi.Identity.Infrastructure.Services.TokenService": "Debug"
    }
  }
}
```
