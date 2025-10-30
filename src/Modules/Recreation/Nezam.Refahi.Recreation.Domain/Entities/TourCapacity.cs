using MCA.SharedKernel.Domain;
using Nezam.Refahi.Recreation.Domain.Enums;

namespace Nezam.Refahi.Recreation.Domain.Entities;

/// <summary>
/// Represents a capacity configuration for a tour with specific time periods and atomic allocation
/// </summary>
public sealed class TourCapacity : Entity<Guid>
{
    public Guid TourId { get; private set; }
    public int MaxParticipants { get; private set; }
    public int RemainingParticipants { get; private set; }
    public DateTime RegistrationStart { get; private set; }
    public DateTime RegistrationEnd { get; private set; }
    public bool IsActive { get; private set; }
    public string? Description { get; private set; }
    public int MinParticipantsPerReservation { get; private set; } = 1;
    public int MaxParticipantsPerReservation { get; private set; } = 10;
    public CapacityState CapacityState { get; private set; }
    
    // Special capacity for VIP members only
    public bool IsSpecial { get; private set; }
    
    // Multi-tenancy support
    public string? TenantId { get; private set; }
    
    // Concurrency control
    public byte[] RowVersion { get; private set; } = null!;

    // Navigation property
    public Tour Tour { get; private set; } = null!;

    // Private constructor for EF Core
    private TourCapacity() : base() { }

    /// <summary>
    /// Creates a new tour capacity
    /// </summary>
    public TourCapacity(
        Guid tourId,
        int maxParticipants,
        DateTime registrationStart,
        DateTime registrationEnd,
        string? description = null,
        int minParticipantsPerReservation = 1,
        int maxParticipantsPerReservation = 10,
        string? tenantId = null,
        bool isSpecial = false)
        : base(Guid.NewGuid())
    {
        ValidateMaxParticipants(maxParticipants);
        ValidateRegistrationDates(registrationStart, registrationEnd);
        ValidateParticipantLimits(minParticipantsPerReservation, maxParticipantsPerReservation);

        TourId = tourId;
        MaxParticipants = maxParticipants;
        RemainingParticipants = maxParticipants; // Initialize with full capacity
        RegistrationStart = registrationStart;
        RegistrationEnd = registrationEnd;
        Description = description?.Trim();
        MinParticipantsPerReservation = minParticipantsPerReservation;
        MaxParticipantsPerReservation = maxParticipantsPerReservation;
        TenantId = tenantId;
        IsSpecial = isSpecial;
        IsActive = true;
        CapacityState = CalculateCapacityState();
    }

    /// <summary>
    /// Updates the capacity settings
    /// </summary>
    public void UpdateCapacity(int maxParticipants, string? description = null)
    {
        ValidateMaxParticipants(maxParticipants);
        
        // Adjust remaining capacity proportionally
        var currentlyUsed = MaxParticipants - RemainingParticipants;
        MaxParticipants = maxParticipants;
        RemainingParticipants = Math.Max(0, maxParticipants - currentlyUsed);
        Description = description?.Trim();
        CapacityState = CalculateCapacityState();
    }

    /// <summary>
    /// Atomically allocates participants for a reservation
    /// This method should be called within a database transaction
    /// </summary>
    public bool TryAllocateParticipants(int requestedCount)
    {
        if (!IsActive)
            return false;
            
        if (!IsRegistrationOpen(DateTime.UtcNow))
            return false;
            
        if (requestedCount < MinParticipantsPerReservation || requestedCount > MaxParticipantsPerReservation)
            return false;
            
        if (RemainingParticipants < requestedCount)
            return false;

        RemainingParticipants -= requestedCount;
        CapacityState = CalculateCapacityState();
        return true;
    }

    /// <summary>
    /// Releases participants back to capacity (for cancellations/expirations)
    /// </summary>
    public void ReleaseParticipants(int count)
    {
        if (count <= 0) return;
        
        RemainingParticipants = Math.Min(MaxParticipants, RemainingParticipants + count);
        CapacityState = CalculateCapacityState();
    }

    /// <summary>
    /// Gets the number of currently allocated participants
    /// </summary>
    public int AllocatedParticipants => MaxParticipants - RemainingParticipants;

    /// <summary>
    /// Gets the utilization percentage
    /// </summary>
    public double UtilizationPercentage => MaxParticipants > 0 ? (double)AllocatedParticipants / MaxParticipants * 100 : 0;

    /// <summary>
    /// Gets the number of currently allocated participants (excluding special capacities from public calculations)
    /// </summary>
    public int PublicAllocatedParticipants => IsSpecial ? 0 : AllocatedParticipants;

    /// <summary>
    /// Gets the utilization percentage (excluding special capacities from public calculations)
    /// </summary>
    public double PublicUtilizationPercentage => IsSpecial ? 0 : UtilizationPercentage;

    /// <summary>
    /// Gets the remaining participants count (excluding special capacities from public calculations)
    /// </summary>
    public int PublicRemainingParticipants => IsSpecial ? 0 : RemainingParticipants;

    /// <summary>
    /// Gets the maximum participants count (excluding special capacities from public calculations)
    /// </summary>
    public int PublicMaxParticipants => IsSpecial ? 0 : MaxParticipants;

    /// <summary>
    /// Checks if capacity can accommodate the requested number of participants
    /// </summary>
    public bool CanAccommodate(int requestedCount)
    {
        return IsActive && 
               IsRegistrationOpen(DateTime.UtcNow) && 
               requestedCount >= MinParticipantsPerReservation && 
               requestedCount <= MaxParticipantsPerReservation && 
               RemainingParticipants >= requestedCount;
    }

    /// <summary>
    /// Checks if capacity can accommodate the requested number of participants for a specific member
    /// </summary>
    public bool CanAccommodateForMember(int requestedCount, bool memberIsSpecial)
    {
        return CanMemberReserve(memberIsSpecial) && 
               requestedCount >= MinParticipantsPerReservation && 
               requestedCount <= MaxParticipantsPerReservation && 
               RemainingParticipants >= requestedCount;
    }

    /// <summary>
    /// Updates the registration period
    /// </summary>
    public void UpdateRegistrationPeriod(DateTime registrationStart, DateTime registrationEnd)
    {
        ValidateRegistrationDates(registrationStart, registrationEnd);
        RegistrationStart = registrationStart;
        RegistrationEnd = registrationEnd;
    }

    /// <summary>
    /// Validates that the registration end date doesn't exceed tour start date
    /// </summary>
    public void ValidateAgainstTourStartDate(DateTime tourStartDate)
    {
        if (RegistrationEnd > tourStartDate)
            throw new InvalidOperationException("Registration end date cannot be after tour start date");
    }

    /// <summary>
    /// Checks if registration is currently open for this capacity
    /// </summary>
    public bool IsRegistrationOpen(DateTime currentDate)
    {
        return IsActive && currentDate >= RegistrationStart && currentDate <= RegistrationEnd;
    }

    /// <summary>
    /// Checks if this capacity is effective for a given date
    /// </summary>
    public bool IsEffectiveFor(DateTime date)
    {
        return IsActive && date >= RegistrationStart && date <= RegistrationEnd;
    }

    /// <summary>
    /// Activates the capacity
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Deactivates the capacity
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// Checks if this capacity overlaps with another capacity's registration period
    /// </summary>
    public bool OverlapsWith(TourCapacity other)
    {
        if (other == null || other.TourId != TourId)
            return false;

        return RegistrationStart < other.RegistrationEnd && RegistrationEnd > other.RegistrationStart;
    }

    /// <summary>
    /// Updates the capacity state based on current availability
    /// </summary>
    public void UpdateCapacityState()
    {
        CapacityState = CalculateCapacityState();
    }

    /// <summary>
    /// Marks this capacity as special (VIP only)
    /// </summary>
    public void MarkAsSpecial()
    {
        IsSpecial = true;
    }

    /// <summary>
    /// Removes special status from this capacity
    /// </summary>
    public void RemoveSpecialStatus()
    {
        IsSpecial = false;
    }

    /// <summary>
    /// Checks if this capacity is visible to a member based on their special status
    /// </summary>
    public bool IsVisibleToMember(bool memberIsSpecial)
    {
        // Regular capacities are visible to everyone
        // Special capacities are only visible to special members
        return !IsSpecial || memberIsSpecial;
    }

    /// <summary>
    /// Checks if a member can make reservations for this capacity
    /// </summary>
    public bool CanMemberReserve(bool memberIsSpecial)
    {
        return IsVisibleToMember(memberIsSpecial) && 
               IsActive && 
               IsRegistrationOpen(DateTime.UtcNow);
    }

    /// <summary>
    /// Calculates the capacity state based on current availability
    /// </summary>
    private CapacityState CalculateCapacityState()
    {
        if (RemainingParticipants <= 0)
            return CapacityState.Full;
        
        var percentage = (double)RemainingParticipants / MaxParticipants;
        return percentage switch
        {
            >= 0.5 => CapacityState.HasSpare,
            >= 0.1 => CapacityState.Tight,
            _ => CapacityState.Full
        };
    }

    // Private validation methods
    private static void ValidateMaxParticipants(int maxParticipants)
    {
        if (maxParticipants <= 0)
            throw new ArgumentException("Maximum participants must be greater than 0", nameof(maxParticipants));
    }

    private static void ValidateRegistrationDates(DateTime registrationStart, DateTime registrationEnd)
    {
        if (registrationStart >= registrationEnd)
            throw new ArgumentException("Registration start must be before registration end");
    }

    private static void ValidateParticipantLimits(int minParticipants, int maxParticipants)
    {
        if (minParticipants <= 0)
            throw new ArgumentException("Minimum participants must be greater than 0", nameof(minParticipants));
        if (maxParticipants <= 0)
            throw new ArgumentException("Maximum participants must be greater than 0", nameof(maxParticipants));
        if (minParticipants > maxParticipants)
            throw new ArgumentException("Minimum participants cannot exceed maximum participants");
    }
}