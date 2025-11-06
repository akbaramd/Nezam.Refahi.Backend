namespace Nezam.Refahi.Recreation.Contracts.Dtos;

/// <summary>
/// DTO for reservation pricing result
/// Contains all information needed for reservation pricing including capabilities/features requirements
/// </summary>
public class ReservationPricingDto
{
    /// <summary>
    /// Pricing ID
    /// </summary>
    public Guid PricingId { get; set; }

    /// <summary>
    /// Tour ID
    /// </summary>
    public Guid TourId { get; set; }

    /// <summary>
    /// Participant type (Member or Guest)
    /// </summary>
    public string ParticipantType { get; set; } = string.Empty;

    /// <summary>
    /// Base price in Rials (before discounts)
    /// </summary>
    public decimal BasePriceRials { get; set; }

    /// <summary>
    /// Effective price in Rials (after discounts)
    /// </summary>
    public decimal EffectivePriceRials { get; set; }

    /// <summary>
    /// Discount percentage (0-100)
    /// </summary>
    public decimal? DiscountPercentage { get; set; }

    /// <summary>
    /// Discount amount in Rials
    /// </summary>
    public decimal? DiscountAmountRials { get; set; }

    /// <summary>
    /// Validity period start date
    /// </summary>
    public DateTime? ValidFrom { get; set; }

    /// <summary>
    /// Validity period end date
    /// </summary>
    public DateTime? ValidTo { get; set; }

    /// <summary>
    /// Indicates if pricing is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Indicates if this is default pricing (no specific capabilities/features required)
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// Indicates if pricing has capability or feature requirements
    /// </summary>
    public bool HasRequirements { get; set; }

    /// <summary>
    /// List of required capability IDs (if HasRequirements is true)
    /// </summary>
    public List<string> RequiredCapabilityIds { get; set; } = new();

    /// <summary>
    /// List of required feature IDs (if HasRequirements is true)
    /// </summary>
    public List<string> RequiredFeatureIds { get; set; } = new();

    /// <summary>
    /// Marketing flags
    /// </summary>
    public bool IsEarlyBird { get; set; }

    /// <summary>
    /// Marketing flags
    /// </summary>
    public bool IsLastMinute { get; set; }

    /// <summary>
    /// Pricing description
    /// </summary>
    public string? Description { get; set; }
}

