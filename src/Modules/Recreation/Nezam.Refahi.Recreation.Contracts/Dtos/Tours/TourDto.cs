namespace Nezam.Refahi.Recreation.Contracts.Dtos;

/// <summary>
/// Lightweight Tour DTO for listings. Primitives + compact aggregates + per-entity summary relations.
/// All photos are listed as summaries; no single "main" is selected by the DTO.
/// </summary>
public class TourDto
{
    // Identity
    public Guid Id { get; set; } = Guid.Empty;
    public string Title { get; set; } = string.Empty;

    // Schedule
    public DateTime TourStart { get; set; } = DateTime.MinValue;
    public DateTime TourEnd { get; set; } = DateTime.MinValue;

    // Status
    public bool IsActive { get; set; } = false;
    public string Status { get; set; } = string.Empty;
    public string CapacityState { get; set; } = string.Empty;

    // Registration window (effective across active capacities)
    public DateTime? RegistrationStart { get; set; } = null;
    public DateTime? RegistrationEnd { get; set; } = null;
    public bool IsRegistrationOpen { get; set; } = false;

    // Capacity aggregates (public numbers exclude special capacities)
    public int MaxCapacity { get; set; } = 0;
    public int RemainingCapacity { get; set; } = 0;
    public int ReservedCapacity { get; set; } = 0;
    public double UtilizationPct { get; set; } = 0; // 0..100
    public bool IsFullyBooked { get; set; } = false;
    public bool IsNearlyFull { get; set; } = false; // ≥80% utilized

    // Relation summaries (light)
    public List<AgencySummaryDto> Agencies { get; set; } = new();
    public List<FeatureSummaryDto> Features { get; set; } = new();
    public List<PhotoSummaryDto> Photos { get; set; } = new();

    // Pricing summaries (per participant type)
    public decimal? LowestPriceRials { get; set; } = null;
    public decimal? HighestPriceRials { get; set; } = null;
    public bool HasDiscount { get; set; } = false;
    public List<PricingDetailDto> Pricing { get; set; } = new();

    // Current user quick flags (set at query layer)
}