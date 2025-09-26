using MCA.SharedKernel.Domain;
using System.Security.Cryptography;
using System.Text;

namespace Nezam.Refahi.Identity.Domain.Entities;

/// <summary>
/// Production-grade token entity implementing DDD + EF Core best practices
/// Supports JWT access tokens and refresh token rotation with proper hashing
/// </summary>
public class UserToken : Entity<Guid>
{
    // ========================================================================
    // Core Properties
    // ========================================================================
    
    /// <summary>
    /// The ID of the user this token belongs to
    /// </summary>
    public Guid UserId { get; private set; }
    
    /// <summary>
    /// The hashed token value (never store raw tokens)
    /// For JWT: stores the jti claim for revocation tracking
    /// For RefreshToken: stores HMAC-SHA256 hash of the raw token
    /// </summary>
    public string TokenValue { get; private set; } = string.Empty;
    
    /// <summary>
    /// The type of token ("AccessToken", "RefreshToken", "OTP", etc.)
    /// </summary>
    public string TokenType { get; private set; } = string.Empty;
    
    /// <summary>
    /// When the token expires (UTC)
    /// </summary>
    public DateTime ExpiresAt { get; private set; }
    
    /// <summary>
    /// Whether the token has been used (for one-time use tokens)
    /// </summary>
    public bool IsUsed { get; private set; }
    
    /// <summary>
    /// Whether the token has been revoked
    /// </summary>
    public bool IsRevoked { get; private set; }
    
    // ========================================================================
    // Security and Session Management
    // ========================================================================
    
    /// <summary>
    /// Device fingerprint for token binding and session management
    /// </summary>
    public string? DeviceFingerprint { get; private set; }
    
    /// <summary>
    /// IP address from which the token was requested
    /// </summary>
    public string? IpAddress { get; private set; }
    
    /// <summary>
    /// User agent from which the token was requested
    /// </summary>
    public string? UserAgent { get; private set; }
    
    /// <summary>
    /// Session family ID for refresh token rotation and reuse detection
    /// All tokens in the same session family share this ID
    /// </summary>
    public Guid? SessionFamilyId { get; private set; }
    
    /// <summary>
    /// Parent token ID for refresh token rotation tracking
    /// </summary>
    public Guid? ParentTokenId { get; private set; }
    
    /// <summary>
    /// When the token was last used (for idle timeout)
    /// </summary>
    public DateTime? LastUsedAt { get; private set; }
    
    /// <summary>
    /// Salt used for token hashing (for refresh tokens)
    /// </summary>
    public string? Salt { get; private set; }
    
    // ========================================================================
    // Navigation Properties
    // ========================================================================
    
    /// <summary>
    /// Navigation property - EF Core will use this for the relationship
    /// </summary>
    public User? User { get; private set; }
    
    // ========================================================================
    // Constructors
    // ========================================================================
    
    /// <summary>
    /// Private constructor for EF Core
    /// </summary>
    private UserToken() : base() { }
    
    /// <summary>
    /// Creates a new JWT access token (stores jti for revocation)
    /// </summary>
    /// <param name="userId">ID of the user</param>
    /// <param name="jwtId">JWT ID (jti claim) for revocation tracking</param>
    /// <param name="expiresInMinutes">Minutes until token expiration (5-15 min)</param>
    /// <param name="deviceFingerprint">Device fingerprint for binding</param>
    /// <param name="ipAddress">IP address for binding</param>
    /// <param name="userAgent">User agent for binding</param>
    public static UserToken CreateAccessToken(
        Guid userId, 
        string jwtId, 
        int expiresInMinutes, 
        string? deviceFingerprint = null, 
        string? ipAddress = null, 
        string? userAgent = null)
    {
        if (string.IsNullOrWhiteSpace(jwtId))
            throw new ArgumentException("JWT ID cannot be empty", nameof(jwtId));
            
  
        return new UserToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenValue = jwtId, // Store jti for revocation tracking
            TokenType = "AccessToken",
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiresInMinutes),
            DeviceFingerprint = deviceFingerprint,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            IsUsed = false,
            IsRevoked = false
        };
    }
    
    /// <summary>
    /// Creates a new refresh token with proper hashing
    /// </summary>
    /// <param name="userId">ID of the user</param>
    /// <param name="rawToken">Raw token value (will be hashed)</param>
    /// <param name="expiresInDays">Days until token expiration (30-90 days)</param>
    /// <param name="sessionFamilyId">Session family ID for rotation tracking</param>
    /// <param name="parentTokenId">Parent token ID for rotation chain</param>
    /// <param name="deviceFingerprint">Device fingerprint for binding</param>
    /// <param name="ipAddress">IP address for binding</param>
    /// <param name="userAgent">User agent for binding</param>
    /// <param name="pepper">Pepper for token hashing</param>
    public static UserToken CreateRefreshToken(
        Guid userId, 
        string rawToken, 
        int expiresInDays, 
        Guid? sessionFamilyId = null, 
        Guid? parentTokenId = null,
        string? deviceFingerprint = null, 
        string? ipAddress = null, 
        string? userAgent = null,
        string pepper = "")
    {
        if (string.IsNullOrWhiteSpace(rawToken))
            throw new ArgumentException("Raw token cannot be empty", nameof(rawToken));
            

        // Generate salt and hash the token
        var salt = GenerateSalt();
        var hashedToken = HashToken(rawToken, salt, pepper);
        
        return new UserToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenValue = hashedToken,
            TokenType = "RefreshToken",
            ExpiresAt = DateTime.UtcNow.AddDays(expiresInDays),
            SessionFamilyId = sessionFamilyId ?? Guid.NewGuid(),
            ParentTokenId = parentTokenId,
            DeviceFingerprint = deviceFingerprint,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Salt = salt,
            IsUsed = false,
            IsRevoked = false
        };
    }
    
    // ========================================================================
    // Business Logic Methods
    // ========================================================================
    
    /// <summary>
    /// Marks the token as used (for one-time use tokens like refresh tokens)
    /// </summary>
    public void MarkAsUsed()
    {
        if (IsUsed)
            throw new InvalidOperationException("Token has already been used");
            
        IsUsed = true;
        LastUsedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Revokes the token
    /// </summary>
    public void Revoke()
    {
        if (IsRevoked)
            return; // Already revoked
            
        IsRevoked = true;
    }
    
    /// <summary>
    /// Updates the last used timestamp (for idle timeout tracking)
    /// </summary>
    public void UpdateLastUsed()
    {
        LastUsedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Validates a raw token against the stored hash
    /// </summary>
    /// <param name="rawToken">Raw token to validate</param>
    /// <param name="pepper">Pepper used in hashing</param>
    /// <returns>True if the token matches the hash</returns>
    public bool ValidateToken(string rawToken, string pepper = "")
    {
        if (string.IsNullOrWhiteSpace(rawToken) || string.IsNullOrWhiteSpace(Salt))
            return false;
            
        var expectedHash = HashToken(rawToken, Salt, pepper);
        return ConstantTimeEquals(TokenValue, expectedHash);
    }
    
    /// <summary>
    /// Checks if the token is valid (not expired, not used, not revoked)
    /// </summary>
    /// <returns>True if the token is valid, false otherwise</returns>
    public bool IsValid()
    {
        return !IsExpired() && !IsUsed && !IsRevoked;
    }
    
    /// <summary>
    /// Checks if the token is expired
    /// </summary>
    /// <returns>True if the token is expired, false otherwise</returns>
    public bool IsExpired()
    {
        return DateTime.UtcNow > ExpiresAt;
    }
    
    /// <summary>
    /// Checks if the token has exceeded idle timeout
    /// </summary>
    /// <param name="idleTimeoutDays">Idle timeout in days (default: 7)</param>
    /// <returns>True if idle timeout exceeded</returns>
    public bool IsIdleTimeoutExceeded(int idleTimeoutDays = 7)
    {
        if (LastUsedAt == null)
            return false;
            
        return DateTime.UtcNow > LastUsedAt.Value.AddDays(idleTimeoutDays);
    }
    
    /// <summary>
    /// Checks if the token binding matches the provided context
    /// </summary>
    /// <param name="deviceFingerprint">Current device fingerprint</param>
    /// <param name="ipAddress">Current IP address</param>
    /// <param name="userAgent">Current user agent</param>
    /// <returns>True if binding matches</returns>
    public bool ValidateBinding(string? deviceFingerprint = null, string? ipAddress = null, string? userAgent = null)
    {
        // Device fingerprint is required for refresh tokens
        if (TokenType == "RefreshToken" && !string.IsNullOrEmpty(DeviceFingerprint))
        {
            if (string.IsNullOrEmpty(deviceFingerprint) || !ConstantTimeEquals(DeviceFingerprint, deviceFingerprint))
                return false;
        }
        
        // IP address binding (optional but recommended)
        if (!string.IsNullOrEmpty(IpAddress) && !string.IsNullOrEmpty(ipAddress))
        {
            if (!ConstantTimeEquals(IpAddress, ipAddress))
                return false;
        }
        
        // User agent binding (optional)
        if (!string.IsNullOrEmpty(UserAgent) && !string.IsNullOrEmpty(userAgent))
        {
            if (!ConstantTimeEquals(UserAgent, userAgent))
                return false;
        }
        
        return true;
    }
    
    // ========================================================================
    // Private Helper Methods
    // ========================================================================
    
    /// <summary>
    /// Generates a cryptographically secure salt
    /// </summary>
    private static string GenerateSalt()
    {
        var saltBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(saltBytes);
        return Convert.ToBase64String(saltBytes);
    }
    
    /// <summary>
    /// Hashes a token with salt and pepper using HMAC-SHA256
    /// </summary>
    private static string HashToken(string rawToken, string salt, string pepper)
    {
        var key = Encoding.UTF8.GetBytes(pepper + salt);
        var message = Encoding.UTF8.GetBytes(rawToken);
        
        using var hmac = new HMACSHA256(key);
        var hashBytes = hmac.ComputeHash(message);
        return Convert.ToBase64String(hashBytes);
    }
    
    /// <summary>
    /// Constant-time string comparison to prevent timing attacks
    /// </summary>
    private static bool ConstantTimeEquals(string a, string b)
    {
        if (a.Length != b.Length)
            return false;
            
        var result = 0;
        for (var i = 0; i < a.Length; i++)
        {
            result |= a[i] ^ b[i];
        }
        return result == 0;
    }
    public void ExtendExpiration(int additionalMinutes)
    {
        if (additionalMinutes <= 0)
            throw new ArgumentException("Additional minutes must be positive", nameof(additionalMinutes));
            
        ExpiresAt = ExpiresAt.AddMinutes(additionalMinutes);
    }
}
