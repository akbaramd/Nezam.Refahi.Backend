using MCA.SharedKernel.Domain;
using MCA.SharedKernel.Domain.AggregateRoots;
using Nezam.Refahi.Identity.Domain.ValueObjects;

namespace Nezam.Refahi.Identity.Domain.Entities;

/// <summary>
/// Represents a refresh session for managing refresh tokens
/// </summary>
public class RefreshSession : Entity<Guid>
{
    /// <summary>
    /// User ID this session belongs to
    /// </summary>
    public Guid UserId { get; private set; }
    
    /// <summary>
    /// Client identifier for this session
    /// </summary>
    public string ClientId { get; private set; } = string.Empty;
    
    /// <summary>
    /// Device fingerprint for this session
    /// </summary>
    public DeviceFingerprint? DeviceFingerprint { get; private set; }
    
    /// <summary>
    /// Current refresh token hash
    /// </summary>
    public HashedSecret CurrentTokenHash { get; private set; } = null!;
    
    /// <summary>
    /// Rotation counter for this session
    /// </summary>
    public int Rotation { get; private set; }
    
    /// <summary>
    /// When this session was created
    /// </summary>
    public DateTime CreatedAt { get; private set; }
    
    /// <summary>
    /// When this session was last used
    /// </summary>
    public DateTime LastUsedAt { get; private set; }
    
    /// <summary>
    /// When this session was revoked (if applicable)
    /// </summary>
    public DateTime? RevokedAt { get; private set; }
    
    /// <summary>
    /// Reason for revocation (if applicable)
    /// </summary>
    public string? RevokeReason { get; private set; }
    
    /// <summary>
    /// Whether this session is currently active
    /// </summary>
    public bool IsActive => RevokedAt == null;

    // Private constructor for EF Core
    private RefreshSession() : base() { }

    /// <summary>
    /// Creates a new refresh session
    /// </summary>
    public RefreshSession(
        Guid userId,
        string clientId,
        HashedSecret tokenHash,
        DeviceFingerprint? deviceFingerprint = null) : base()
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
            
        if (string.IsNullOrWhiteSpace(clientId))
            throw new ArgumentException("Client ID cannot be empty", nameof(clientId));
            
        if (tokenHash == null)
            throw new ArgumentNullException(nameof(tokenHash));

        UserId = userId;
        ClientId = clientId;
        DeviceFingerprint = deviceFingerprint;
        CurrentTokenHash = tokenHash;
        Rotation = 1;
        LastUsedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Rotates the refresh token for this session
    /// </summary>
    /// <param name="newTokenHash">New token hash</param>
    public void Rotate(HashedSecret newTokenHash)
    {
        if (newTokenHash == null)
            throw new ArgumentNullException(nameof(newTokenHash));

        if (!IsActive)
            throw new InvalidOperationException("Cannot rotate token for inactive session");

        CurrentTokenHash = newTokenHash;
        Rotation++;
        LastUsedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the session as used (touch)
    /// </summary>
    public void Touch()
    {
        if (!IsActive)
            throw new InvalidOperationException("Cannot touch inactive session");

        LastUsedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Revokes this session
    /// </summary>
    /// <param name="reason">Reason for revocation</param>
    public void Revoke(string reason)
    {
        if (!IsActive)
            return; // Already revoked

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Revoke reason cannot be empty", nameof(reason));

        RevokedAt = DateTime.UtcNow;
        RevokeReason = reason;
    }

    /// <summary>
    /// Checks if the provided token hash matches the current one
    /// </summary>
    /// <param name="tokenHash">Token hash to verify</param>
    /// <returns>True if matches, false otherwise</returns>
    public bool IsValidToken(HashedSecret tokenHash)
    {
        if (tokenHash == null) return false;
        if (!IsActive) return false;
        
        return CurrentTokenHash.Hash == tokenHash.Hash && 
               CurrentTokenHash.Algorithm == tokenHash.Algorithm;
    }

    /// <summary>
    /// Gets the session age in minutes
    /// </summary>
    public int SessionAgeMinutes => (int)(DateTime.UtcNow - CreatedAt).TotalMinutes;

    /// <summary>
    /// Gets the time since last use in minutes
    /// </summary>
    public int MinutesSinceLastUse => (int)(DateTime.UtcNow - LastUsedAt).TotalMinutes;
}
