using MCA.SharedKernel.Domain;
using Nezam.Refahi.Identity.Domain.ValueObjects;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Identity.Domain.Entities;

/// <summary>
/// Represents a claim directly assigned to a user (not through roles)
/// </summary>
public class UserClaim : Entity<Guid>
{
    public Guid UserId { get; private set; }
    public Claim Claim { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public DateTime AssignedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public string? AssignedBy { get; private set; }
    public string? Notes { get; private set; }
    
    // Navigation properties
    public User User { get; private set; } = null!;

    // Private constructor for EF Core
    public UserClaim() : base() { }

    public UserClaim(Guid userId, Claim claim, DateTime? expiresAt = null, string? assignedBy = null, string? notes = null) : base(Guid.NewGuid())
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        if (claim == null)
            throw new ArgumentNullException(nameof(claim));

        UserId = userId;
        Claim = claim;
        IsActive = true;
        AssignedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
        AssignedBy = assignedBy?.Trim();
        Notes = notes?.Trim();
    }

    /// <summary>
    /// Activates the user claim
    /// </summary>
    public void Activate()
    {
        if (!IsActive)
        {
            IsActive = true;
        }
    }

    /// <summary>
    /// Deactivates the user claim
    /// </summary>
    public void Deactivate()
    {
        if (IsActive)
        {
            IsActive = false;
        }
    }

    /// <summary>
    /// Updates the claim value
    /// </summary>
    public void UpdateClaim(Claim newClaim)
    {
        if (newClaim == null)
            throw new ArgumentNullException(nameof(newClaim));

        Claim = newClaim;
    }

    /// <summary>
    /// Updates the expiration date
    /// </summary>
    public void UpdateExpiration(DateTime? expiresAt)
    {
        ExpiresAt = expiresAt;
    }

    /// <summary>
    /// Updates the notes
    /// </summary>
    public void UpdateNotes(string? notes)
    {
        Notes = notes?.Trim();
    }

    /// <summary>
    /// Checks if the claim assignment is expired
    /// </summary>
    public bool IsExpired()
    {
        return ExpiresAt.HasValue && ExpiresAt.Value <= DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if the claim assignment is valid (active and not expired)
    /// </summary>
    public bool IsValid()
    {
        return IsActive && !IsExpired();
    }

    /// <summary>
    /// Gets the time remaining until expiration
    /// </summary>
    public TimeSpan? GetTimeUntilExpiration()
    {
        if (!ExpiresAt.HasValue)
            return null;

        var timeRemaining = ExpiresAt.Value - DateTime.UtcNow;
        return timeRemaining > TimeSpan.Zero ? timeRemaining : TimeSpan.Zero;
    }

    /// <summary>
    /// Checks if the claim assignment will expire soon (within the specified time)
    /// </summary>
    public bool WillExpireSoon(TimeSpan timeThreshold)
    {
        if (!ExpiresAt.HasValue)
            return false;

        var timeRemaining = GetTimeUntilExpiration();
        return timeRemaining.HasValue && timeRemaining.Value <= timeThreshold;
    }

    /// <summary>
    /// Checks if this claim matches the given type and value
    /// </summary>
    public bool Matches(string claimType, string claimValue)
    {
        if (string.IsNullOrWhiteSpace(claimType) || string.IsNullOrWhiteSpace(claimValue))
            return false;

        return Claim.Type.Equals(claimType, StringComparison.OrdinalIgnoreCase) &&
               Claim.Value.Equals(claimValue, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if this claim matches the given claim
    /// </summary>
    public bool Matches(Claim claim)
    {
        if (claim == null)
            return false;

        return Claim.Type == claim.Type && Claim.Value == claim.Value;
    }

    public override string ToString()
    {
        return $"User {UserId}: {Claim} (Active: {IsActive}, Expires: {ExpiresAt})";
    }
}
