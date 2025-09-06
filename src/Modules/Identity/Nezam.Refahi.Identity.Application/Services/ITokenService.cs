// -----------------------------------------------------------------------------
// ITokenService.cs - Production-grade DDD + EF Core token system
// -----------------------------------------------------------------------------

using Nezam.Refahi.Identity.Domain.Entities;

namespace Nezam.Refahi.Identity.Application.Services;

/// <summary>
/// Production-grade token service implementing DDD + EF Core best practices
/// Supports JWT access tokens (5-15 min TTL) and refresh token rotation
/// </summary>
public interface ITokenService
{
  // ========================================================================
  // JWT Access Token Operations (Stateless, 5-15 min TTL)
  // ========================================================================
  
  /// <summary>
  /// Generates a JWT access token with RS256/ES256 signing
  /// Lifespan: 5-15 minutes, stateless validation
  /// </summary>
  /// <param name="user">The user to generate a token for</param>
  /// <param name="jwtId">OUT: the generated jti claim value for revocation tracking</param>
  /// <param name="expiryMinutes">Token expiry in minutes (default: 15)</param>
  /// <returns>The serialized JWT token string</returns>
  string GenerateAccessToken(User user, out string jwtId, int expiryMinutes = 15);

  /// <summary>
  /// Validates a JWT access token with full signature and lifetime validation
  /// Returns true when signature, issuer, audience and lifetime are all valid
  /// </summary>
  /// <param name="token">The token to validate</param>
  /// <param name="jwtId">OUT: the token's jti claim (empty when validation fails)</param>
  /// <param name="userId">OUT: the user ID from sub claim (null when validation fails)</param>
  /// <returns>True if the token is structurally valid; false otherwise</returns>
  bool ValidateAccessToken(string token, out string? jwtId, out Guid? userId);

  // ========================================================================
  // Refresh Token Operations (Stateful, 30-90 day TTL with rotation)
  // ========================================================================
  
  /// <summary>
  /// Generates a secure refresh token with proper hashing
  /// Entropy: 256-bit random (base64url), hashed at rest
  /// </summary>
  /// <param name="userId">User ID for the token</param>
  /// <param name="deviceFingerprint">Device fingerprint for binding</param>
  /// <param name="ipAddress">IP address for binding</param>
  /// <param name="userAgent">User agent for binding</param>
  /// <param name="expiryDays">Token expiry in days (default: 30)</param>
  /// <returns>Tuple of (rawToken, hashedToken, tokenId)</returns>
  Task<(string RawToken, string HashedToken, Guid TokenId)> GenerateRefreshTokenAsync(
    Guid userId, 
    string? deviceFingerprint = null, 
    string? ipAddress = null, 
    string? userAgent = null, 
    int expiryDays = 30);

  /// <summary>
  /// Validates and rotates a refresh token (one-time use)
  /// Implements reuse detection and session family management
  /// </summary>
  /// <param name="rawToken">The raw refresh token to validate</param>
  /// <param name="deviceFingerprint">Current device fingerprint</param>
  /// <param name="ipAddress">Current IP address</param>
  /// <param name="userAgent">Current user agent</param>
  /// <returns>Token validation result with new tokens if valid</returns>
  Task<RefreshTokenValidationResult> ValidateAndRotateRefreshTokenAsync(
    string rawToken, 
    string? deviceFingerprint = null, 
    string? ipAddress = null, 
    string? userAgent = null);

  // ========================================================================
  // Token Revocation and Cleanup
  // ========================================================================
  
  /// <summary>
  /// Revokes a specific JWT by its jti claim
  /// Adds to distributed cache deny-list for emergency revocation
  /// </summary>
  /// <param name="jwtId">JWT ID to revoke</param>
  /// <param name="remainingLifetime">Remaining token lifetime for cache TTL</param>
  Task RevokeJwtAsync(string jwtId, TimeSpan remainingLifetime);

  /// <summary>
  /// Revokes all refresh tokens for a user (logout from all devices)
  /// </summary>
  /// <param name="userId">User ID to revoke tokens for</param>
  Task RevokeAllUserRefreshTokensAsync(Guid userId);

  /// <summary>
  /// Revokes refresh tokens for a specific device
  /// </summary>
  /// <param name="userId">User ID</param>
  /// <param name="deviceFingerprint">Device fingerprint to revoke</param>
  Task RevokeDeviceRefreshTokensAsync(Guid userId, string deviceFingerprint);

  /// <summary>
  /// Cleans up expired tokens and performs maintenance
  /// </summary>
  Task<int> CleanupExpiredTokensAsync();
}

/// <summary>
/// Result of refresh token validation and rotation
/// </summary>
public record RefreshTokenValidationResult
{
  public bool IsValid { get; init; }
  public string? ErrorMessage { get; init; }
  public Guid? UserId { get; init; }
  public string? NewAccessToken { get; init; }
  public string? NewRefreshToken { get; init; }
  public string? NewJwtId { get; init; }
  public bool IsReuseDetected { get; init; }
  public bool IsSessionCompromised { get; init; }
}
