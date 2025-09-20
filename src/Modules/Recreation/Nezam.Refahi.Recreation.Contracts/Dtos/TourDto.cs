using Nezam.Refahi.Recreation.Domain.Enums;

namespace Nezam.Refahi.Recreation.Contracts.Dtos;

/// <summary>
/// Tour data transfer object containing all tour information
/// </summary>
public class TourDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public DateTime TourStart { get; set; }
    public DateTime TourEnd { get; set; }

    // Capacity information (calculated from active capacities)
    public int MaxCapacity { get; set; }
    public int RemainingCapacity { get; set; }
    public int ReservedCapacity { get; set; }
    public double CapacityUtilizationPercentage { get; set; }
    public bool IsFullyBooked { get; set; }
    public bool IsNearlyFull { get; set; } // When > 80% booked


    public bool IsActive { get; set; }

    // Age restrictions
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }

    // Registration period (calculated from active capacities)
    public DateTime? RegistrationStart { get; set; }
    public DateTime? RegistrationEnd { get; set; }
    public bool IsRegistrationOpen { get; set; }

    // Tour capacities collection
    public List<TourCapacityDto> Capacities { get; set; } = new();

    // Member requirements
    public List<string> RequiredCapabilities { get; set; } = new();
    public List<string> RequiredFeatures { get; set; } = new();

    // Tour features
    public List<TourFeatureDto> Features { get; set; } = new();

    // Restricted tours
    public List<RestrictedTourDto> RestrictedTours { get; set; } = new();

    // Pricing information
    public List<TourPricingDto> Pricing { get; set; } = new();

    // Photos
    public List<TourPhotoDto> Photos { get; set; } = new();

    // Participants information (summary)
    public int TotalParticipants { get; set; }
    public int MainParticipants { get; set; }
    public int GuestParticipants { get; set; }

    // User reservation status and ID
    public ReservationStatus? UserReservationStatus { get; set; }
    public Guid? UserReservationId { get; set; }

    // Audit fields
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
}