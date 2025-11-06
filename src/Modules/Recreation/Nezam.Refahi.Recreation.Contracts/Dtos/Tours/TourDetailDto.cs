namespace Nezam.Refahi.Recreation.Contracts.Dtos;

/// <summary>
/// Detailed Tour DTO (inherits from TourDto). Contains full relations. No "best price" fields.
/// </summary>
public class TourDetailDto : TourDto
{
    // Long-form content
    public string? Description { get; set; } = string.Empty;
    public string? LongDescription { get; set; } = string.Empty;
    public string? Summary { get; set; } = string.Empty;

    // Policies / limits
    public int? MinAge { get; set; } = null;
    public int? MaxAge { get; set; } = null;
    public int? MaxGuestsPerReservation { get; set; } = null;

    // Member requirements (with names from BasicDefinitions)
    public List<RequiredCapabilityDto> RequiredCapabilities { get; set; } = new();
    public List<RequiredFeatureDto> RequiredFeatures { get; set; } = new();

    // Rich relations
    public List<CapacityDetailDto> Capacities { get; set; } = new();
    public new List<FeatureDetailDto> Features { get; set; } = new();     // hides base Feature summaries
    public new List<PhotoDetailDto> Photos { get; set; } = new();         // hides base Photo summaries
    public new List<AgencyDetailDto> Agencies { get; set; } = new();      // hides base Agency summaries
    public List<RestrictedTourSummaryDto> RestrictedTours { get; set; } = new();

    // Basic analytics (nullable to avoid forced loading)
    public int? TotalReservations { get; set; } = null;
    public int? ConfirmedReservations { get; set; } = null;
    public int? PendingReservations { get; set; } = null;
    public int? CancelledReservations { get; set; } = null;

    // User-specific reservation extras (filled by application layer)
    public string? UserReservationTrackingCode { get; set; } = string.Empty;
    public DateTime? UserReservationDate { get; set; } = null;
    public DateTime? UserReservationExpiryDate { get; set; } = null;
}