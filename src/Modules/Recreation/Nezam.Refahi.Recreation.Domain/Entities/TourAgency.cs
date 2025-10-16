using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Recreation.Domain.Entities;

/// <summary>
/// Junction entity representing the many-to-many relationship between Tour and Agency
/// Tracks agency access assignments for tours with validity periods and access levels
/// </summary>
public sealed class TourAgency : Entity<Guid>
{
    public Guid TourId { get; private set; }
    public Guid AgencyId { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Cached agency information for performance
    public string AgencyCode { get; private set; } = string.Empty;
    public string AgencyName { get; private set; } = string.Empty;

    public DateTime? ValidFrom { get; private set; }
    public DateTime? ValidTo { get; private set; }
    public string? AssignedBy { get; private set; }      // Who assigned this tour access
    public DateTime AssignedAt { get; private set; }     // When was it assigned
    public string? Notes { get; private set; }           // Additional context
    public string? AccessLevel { get; private set; }      // Access level (e.g., "Full", "ReadOnly", "Limited")
    public int? MaxReservations { get; private set; }    // Maximum number of reservations allowed
    public int? MaxParticipants { get; private set; }    // Maximum participants per reservation

    // Navigation properties
    public Tour Tour { get; private set; } = null!;
    // Agency relation will be handled through BasicDefinitions context

    // Private constructor for EF Core
    private TourAgency() : base() { }

    public TourAgency(Guid tourId, Guid agencyId,
        string agencyCode, string agencyName,
        DateTime? validFrom = null, DateTime? validTo = null,
        string? assignedBy = null, string? notes = null, 
        string? accessLevel = null, int? maxReservations = null, 
        int? maxParticipants = null)
        : base(Guid.NewGuid())
    {
        if (tourId == Guid.Empty)
            throw new ArgumentException("Tour ID cannot be empty", nameof(tourId));
        if (agencyId == Guid.Empty)
            throw new ArgumentException("Representative Office ID cannot be empty", nameof(agencyId));
        if (string.IsNullOrWhiteSpace(agencyCode))
            throw new ArgumentException("Agency Code cannot be empty", nameof(agencyCode));
        if (string.IsNullOrWhiteSpace(agencyName))
            throw new ArgumentException("Agency Name cannot be empty", nameof(agencyName));

        TourId = tourId;
        AgencyId = agencyId;
        AgencyCode = agencyCode.Trim();
        AgencyName = agencyName.Trim();
        ValidFrom = validFrom;
        ValidTo = validTo;
        AssignedBy = assignedBy?.Trim();
        AssignedAt = DateTime.UtcNow;
        Notes = notes?.Trim();
        AccessLevel = accessLevel?.Trim();
        MaxReservations = maxReservations;
        MaxParticipants = maxParticipants;
    }

    /// <summary>
    /// Checks if the tour access assignment is currently valid (active and within validity period)
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
    /// Checks if the tour access assignment is expired
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
    /// Updates the maximum reservations allowed
    /// </summary>
    public void UpdateMaxReservations(int? maxReservations)
    {
        MaxReservations = maxReservations;
    }

    /// <summary>
    /// Updates the maximum participants per reservation
    /// </summary>
    public void UpdateMaxParticipants(int? maxParticipants)
    {
        MaxParticipants = maxParticipants;
    }

    /// <summary>
    /// Updates the cached agency information when agency details change
    /// </summary>
    public void UpdateAgencyInformation(string agencyCode, string agencyName)
    {
        if (string.IsNullOrWhiteSpace(agencyCode))
            throw new ArgumentException("Agency Code cannot be empty", nameof(agencyCode));
        if (string.IsNullOrWhiteSpace(agencyName))
            throw new ArgumentException("Agency Name cannot be empty", nameof(agencyName));

        AgencyCode = agencyCode.Trim();
        AgencyName = agencyName.Trim();
    }

    /// <summary>
    /// Activates the tour access assignment
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Deactivates the tour access assignment
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// Updates who assigned this tour access
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

    /// <summary>
    /// Checks if the agency can make reservations for this tour
    /// </summary>
    public bool CanMakeReservations()
    {
        return IsValid() && (AccessLevel == null || HasFullAccess() || HasLimitedAccess());
    }

    /// <summary>
    /// Checks if the agency can only view this tour (read-only access)
    /// </summary>
    public bool CanOnlyView()
    {
        return IsValid() && HasReadOnlyAccess();
    }
}
