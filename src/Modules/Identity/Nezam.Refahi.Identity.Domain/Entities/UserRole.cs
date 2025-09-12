using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Identity.Domain.Entities;

/// <summary>
/// Represents the many-to-many relationship between User and Role
/// </summary>
public class UserRole : Entity<Guid>
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime AssignedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public string? AssignedBy { get; private set; }
    public string? Notes { get; private set; }
    
    // Navigation properties
    public User User { get; private set; } = null!;
    public Role Role { get; private set; } = null!;

    // Private constructor for EF Core
    public UserRole() : base() { }

    public UserRole(Guid userId, Guid roleId, DateTime? expiresAt = null, string? assignedBy = null, string? notes = null) : base(Guid.NewGuid())
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        if (roleId == Guid.Empty)
            throw new ArgumentException("Role ID cannot be empty", nameof(roleId));

        UserId = userId;
        RoleId = roleId;
        IsActive = true;
        AssignedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
        AssignedBy = assignedBy?.Trim();
        Notes = notes?.Trim();
    }

    /// <summary>
    /// Activates the user role assignment
    /// </summary>
    public void Activate()
    {
        if (!IsActive)
        {
            IsActive = true;
            // Note: In a real implementation, you would raise a domain event here
        }
    }

    /// <summary>
    /// Deactivates the user role assignment
    /// </summary>
    public void Deactivate()
    {
        if (IsActive)
        {
            IsActive = false;
            // Note: In a real implementation, you would raise a domain event here
        }
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
    /// Checks if the role assignment is expired
    /// </summary>
    public bool IsExpired()
    {
        return ExpiresAt.HasValue && ExpiresAt.Value <= DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if the role assignment is valid (active and not expired)
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
    /// Checks if the role assignment will expire soon (within the specified time)
    /// </summary>
    public bool WillExpireSoon(TimeSpan timeThreshold)
    {
        if (!ExpiresAt.HasValue)
            return false;

        var timeRemaining = GetTimeUntilExpiration();
        return timeRemaining.HasValue && timeRemaining.Value <= timeThreshold;
    }

    public override string ToString()
    {
        return $"User {UserId} -> Role {RoleId} (Active: {IsActive}, Expires: {ExpiresAt})";
    }
}
