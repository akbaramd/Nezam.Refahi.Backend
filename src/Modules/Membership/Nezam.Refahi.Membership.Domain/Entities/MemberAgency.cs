using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Membership.Domain.Entities;

/// <summary>
/// Junction entity representing the many-to-many relationship between Member and Agency
/// Tracks office access assignments with validity periods and provenance
/// </summary>
public sealed class MemberAgency : Entity<Guid>
{
    public Guid MemberId { get; private set; }
    public Guid AgencyId { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Cached office information for performance
    public string OfficeCode { get; private set; } = string.Empty;
    public string OfficeTitle { get; private set; } = string.Empty;

    public DateTime? ValidFrom { get; private set; }
    public DateTime? ValidTo { get; private set; }
    public string? AssignedBy { get; private set; }      // Who assigned this office access
    public DateTime AssignedAt { get; private set; }     // When was it assigned
    public string? Notes { get; private set; }           // Additional context
    public string? AccessLevel { get; private set; }      // Access level (e.g., "Full", "ReadOnly", "Limited")

    // Navigation properties
    public Member Member { get; private set; } = null!;
    // Agency relation will be handled through BasicDefinitions context

    // Private constructor for EF Core
    private MemberAgency() : base() { }

    public MemberAgency(Guid memberId, Guid agencyId,
        string officeCode, string officeTitle,
        DateTime? validFrom = null, DateTime? validTo = null,
        string? assignedBy = null, string? notes = null, string? accessLevel = null)
        : base(Guid.NewGuid())
    {
        if (memberId == Guid.Empty)
            throw new ArgumentException("Member ID cannot be empty", nameof(memberId));
        if (agencyId == Guid.Empty)
            throw new ArgumentException("Representative Office ID cannot be empty", nameof(agencyId));
        if (string.IsNullOrWhiteSpace(officeCode))
            throw new ArgumentException("Office Code cannot be empty", nameof(officeCode));
        if (string.IsNullOrWhiteSpace(officeTitle))
            throw new ArgumentException("Office Title cannot be empty", nameof(officeTitle));

        MemberId = memberId;
        AgencyId = agencyId;
        OfficeCode = officeCode.Trim();
        OfficeTitle = officeTitle.Trim();
        ValidFrom = validFrom;
        ValidTo = validTo;
        AssignedBy = assignedBy?.Trim();
        AssignedAt = DateTime.UtcNow;
        Notes = notes?.Trim();
        AccessLevel = accessLevel?.Trim();
    }

    /// <summary>
    /// Checks if the office access assignment is currently valid (active and within validity period)
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
    /// Checks if the office access assignment is expired
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
    /// Updates the access level
    /// </summary>
    public void UpdateAccessLevel(string? accessLevel)
    {
        AccessLevel = accessLevel?.Trim();
    }

    /// <summary>
    /// Updates the cached office information when office details change
    /// </summary>
    public void UpdateOfficeInformation(string officeCode, string officeTitle)
    {
        if (string.IsNullOrWhiteSpace(officeCode))
            throw new ArgumentException("Office Code cannot be empty", nameof(officeCode));
        if (string.IsNullOrWhiteSpace(officeTitle))
            throw new ArgumentException("Office Title cannot be empty", nameof(officeTitle));

        OfficeCode = officeCode.Trim();
        OfficeTitle = officeTitle.Trim();
    }

    /// <summary>
    /// Activates the office access assignment
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Deactivates the office access assignment
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// Updates who assigned this office access
    /// </summary>
    public void UpdateAssignedBy(string? assignedBy)
    {
        AssignedBy = assignedBy?.Trim();
    }

    /// <summary>
    /// Checks if the access level is full access
    /// </summary>
    public bool HasFullAccess()
    {
        return string.Equals(AccessLevel, "Full", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the access level is read-only
    /// </summary>
    public bool HasReadOnlyAccess()
    {
        return string.Equals(AccessLevel, "ReadOnly", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the access level is limited
    /// </summary>
    public bool HasLimitedAccess()
    {
        return string.Equals(AccessLevel, "Limited", StringComparison.OrdinalIgnoreCase);
    }
}
