namespace Nezam.Refahi.Recreation.Contracts.Dtos;

/// <summary>
/// Tour capacity data transfer object with detailed capacity information
/// </summary>
public class TourCapacityDto
{
    public Guid Id { get; set; }
    public Guid TourId { get; set; }
    public int MaxParticipants { get; set; }
    public int RemainingParticipants { get; set; }
    public int AllocatedParticipants { get; set; }
    public double UtilizationPercentage { get; set; }
    public int MinParticipantsPerReservation { get; set; }
    public int MaxParticipantsPerReservation { get; set; }
    
    public DateTime RegistrationStart { get; set; }
    public DateTime RegistrationEnd { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
    public bool IsRegistrationOpen { get; set; }
    public bool IsEffectiveFor { get; set; }
    public bool IsFullyBooked { get; set; }
    public bool IsNearlyFull { get; set; } // When > 80% booked
    
    // Availability status for UI
    public string AvailabilityStatus { get; set; } = string.Empty; // Available, Nearly Full, Full, Closed
    public string AvailabilityMessage { get; set; } = string.Empty; // Human readable message
}