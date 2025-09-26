using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Membership.Domain.Entities;

/// <summary>
/// Junction entity representing the many-to-many relationship between Member and Capability
/// Tracks capability assignments with validity periods and provenance
/// </summary>
public sealed class MemberCapability : Entity<Guid>
{
    public Guid MemberId { get; private set; }
    public string CapabilityId { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    public DateTime? ValidFrom { get; private set; }
    public DateTime? ValidTo { get; private set; }
    public string? AssignedBy { get; private set; }      // Who assigned this capability
    public DateTime AssignedAt { get; private set; }    // When was it assigned
    public string? Notes { get; private set; }          // Additional context

    // Navigation properties - Only Member, no Capability relation
    public Member Member { get; private set; } = null!;
    // Capability relation removed - only store the key

    // Private constructor for EF Core
    private MemberCapability() : base() { }

    public MemberCapability(Guid memberId, string capabilityId,
        DateTime? validFrom = null, DateTime? validTo = null,
        string? assignedBy = null, string? notes = null)
        : base(Guid.NewGuid())
    {
        if (memberId == Guid.Empty)
            throw new ArgumentException("Member ID cannot be empty", nameof(memberId));
        if (capabilityId == string.Empty)
            throw new ArgumentException("Capability ID cannot be empty", nameof(capabilityId));

        MemberId = memberId;
        CapabilityId = capabilityId;
        ValidFrom = validFrom;
        ValidTo = validTo;
        AssignedBy = assignedBy?.Trim();
        AssignedAt = DateTime.UtcNow;
        Notes = notes?.Trim();
    }

    /// <summary>
    /// Checks if the capability assignment is currently valid (active and within validity period)
    /// </summary>
    public bool IsValid()
    {
        if (!IsActive)
            return false;

        var now = DateTimeOffset.UtcNow;

        if (ValidFrom.HasValue && now < ValidFrom.Value)
            return false;

        if (ValidTo.HasValue && now > ValidTo.Value)
            return false;

        return true;
    }

    /// <summary>
    /// Checks if the capability assignment is expired
    /// </summary>
    public bool IsExpired()
    {
        return ValidTo.HasValue && DateTimeOffset.UtcNow > ValidTo.Value;
    }

    /// <summary>
    /// Gets the remaining time until expiration
    /// </summary>
    public TimeSpan? GetTimeUntilExpiration()
    {
        if (!ValidTo.HasValue)
            return null;

        var timeRemaining = ValidTo.Value - DateTimeOffset.UtcNow;
        return timeRemaining > TimeSpan.Zero ? timeRemaining : TimeSpan.Zero;
    }

    /// <summary>
    /// Updates the validity period
    /// </summary>
    public void UpdateValidityPeriod(DateTime? validFrom, DateTime? validTo)
    {
        ValidFrom = validFrom;
        ValidTo = validTo;
    }

    /// <summary>
    /// Updates the notes
    /// </summary>
    public void UpdateNotes(string? notes)
    {
        Notes = notes?.Trim();
    }

    /// <summary>
    /// Activates the capability assignment
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Deactivates the capability assignment
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// Updates who assigned this capability
    /// </summary>
    public void UpdateAssignedBy(string? assignedBy)
    {
        AssignedBy = assignedBy?.Trim();
    }
}