using Nezam.Refahi.Domain.BoundedContexts.Shared.Entities;
using System;

namespace Nezam.Refahi.Domain.BoundedContexts.Identity.Entities;

/// <summary>
/// Represents a token associated with a user for authentication purposes
/// </summary>
public class UserToken : BaseEntity
{
    /// <summary>
    /// The ID of the user this token belongs to
    /// </summary>
    public Guid UserId { get; private set; }
    
    /// <summary>
    /// The token value (could be JWT, OTP code, refresh token, etc.)
    /// </summary>
    public string TokenValue { get; private set; } = string.Empty;
    
    /// <summary>
    /// The type of token (e.g., "OTP", "JWT", "RefreshToken")
    /// </summary>
    public string TokenType { get; private set; } = string.Empty;
    
    /// <summary>
    /// When the token expires
    /// </summary>
    public DateTime ExpiresAt { get; private set; }
    
    /// <summary>
    /// Whether the token has been used
    /// </summary>
    public bool IsUsed { get; private set; }
    
    /// <summary>
    /// Whether the token has been revoked
    /// </summary>
    public bool IsRevoked { get; private set; }
    
    /// <summary>
    /// Optional device identifier for the device that requested the token
    /// </summary>
    public string? DeviceId { get; private set; }
    
    /// <summary>
    /// Optional IP address from which the token was requested
    /// </summary>
    public string? IpAddress { get; private set; }
    
    // Navigation property - EF Core will use this for the relationship
    public User? User { get; private set; }
    
    // Private constructor for EF Core
    private UserToken() : base() { }
    
    /// <summary>
    /// Creates a new token for a user
    /// </summary>
    /// <param name="userId">ID of the user</param>
    /// <param name="tokenValue">The token value</param>
    /// <param name="tokenType">Type of token (e.g., "OTP", "JWT", "RefreshToken")</param>
    /// <param name="expiresInMinutes">Minutes until token expiration</param>
    /// <param name="deviceId">Optional device identifier</param>
    /// <param name="ipAddress">Optional IP address</param>
    public UserToken(
        Guid userId, 
        string tokenValue, 
        string tokenType, 
        int expiresInMinutes, 
        string? deviceId = null, 
        string? ipAddress = null) : base()
    {
        if (string.IsNullOrWhiteSpace(tokenValue))
            throw new ArgumentException("Token value cannot be empty", nameof(tokenValue));
            
        if (string.IsNullOrWhiteSpace(tokenType))
            throw new ArgumentException("Token type cannot be empty", nameof(tokenType));
            
        if (expiresInMinutes <= 0)
            throw new ArgumentException("Expiration time must be positive", nameof(expiresInMinutes));
            
        UserId = userId;
        TokenValue = tokenValue;
        TokenType = tokenType;
        ExpiresAt = DateTime.UtcNow.AddMinutes(expiresInMinutes);
        IsUsed = false;
        IsRevoked = false;
        DeviceId = deviceId;
        IpAddress = ipAddress;
    }
    
    /// <summary>
    /// Marks the token as used
    /// </summary>
    public void MarkAsUsed()
    {
        IsUsed = true;
        UpdateModifiedAt();
    }
    
    /// <summary>
    /// Revokes the token
    /// </summary>
    public void Revoke()
    {
        IsRevoked = true;
        UpdateModifiedAt();
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
    /// Extends the token's expiration time
    /// </summary>
    /// <param name="additionalMinutes">Additional minutes to extend the token's validity</param>
    public void ExtendExpiration(int additionalMinutes)
    {
        if (additionalMinutes <= 0)
            throw new ArgumentException("Additional minutes must be positive", nameof(additionalMinutes));
            
        ExpiresAt = ExpiresAt.AddMinutes(additionalMinutes);
        UpdateModifiedAt();
    }
}
