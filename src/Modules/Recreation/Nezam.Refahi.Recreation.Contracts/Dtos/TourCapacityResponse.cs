using Nezam.Refahi.Recreation.Domain.Enums;

namespace Nezam.Refahi.Recreation.Contracts.Dtos;

/// <summary>
/// Response for tour capacity endpoint
/// </summary>
public class TourCapacityResponse
{
    public Guid TourId { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<TourCapacityDetailDto> Capacities { get; set; } = new();
    public int MaxCapacity { get; set; }
    public int RemainingCapacity { get; set; }
    public int ReservedCapacity { get; set; }
    public double UtilizationPercentage { get; set; }
    public bool IsFullyBooked { get; set; }
    public bool IsNearlyFull { get; set; }
    public string CapacityStatus { get; set; } = string.Empty;
    public string CapacityMessage { get; set; } = string.Empty;
    public bool IsRegistrationOpen { get; set; }
    public DateTime? RegistrationEnd { get; set; }
}

/// <summary>
/// Response for tour pricing endpoint
/// </summary>
public class TourPricingResponse
{
    public Guid TourId { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<TourPricingDetailDto> Pricing { get; set; } = new();
    public decimal? LowestPrice { get; set; }
    public decimal? HighestPrice { get; set; }
    public bool HasDiscount { get; set; }
    public decimal? MaxDiscountPercentage { get; set; }
}

/// <summary>
/// Response for tour reservation eligibility endpoint
/// </summary>
public class TourReservationEligibilityResponse
{
    public Guid TourId { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool CanReserve { get; set; }
    public List<string> BlockReasons { get; set; } = new();
    public ReservationStatus? UserReservationStatus { get; set; }
    public Guid? UserReservationId { get; set; }
    public string? UserReservationTrackingCode { get; set; }
    public bool IsRegistrationOpen { get; set; }
    public DateTime? RegistrationEnd { get; set; }
    public int RemainingCapacity { get; set; }
    public bool IsFullyBooked { get; set; }
    public List<RestrictedTourDetailDto> RestrictedTours { get; set; } = new();
}
