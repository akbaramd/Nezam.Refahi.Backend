using MCA.SharedKernel.Domain;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Recreation.Domain.Entities;

/// <summary>
/// Represents a frozen snapshot of pricing at the time of reservation creation.
/// Captures all pricing rule details including capabilities and features that determined the price.
/// </summary>
public sealed class ReservationPriceSnapshot : Entity<Guid>
{
    public Guid ReservationId { get; private set; }
    public ParticipantType ParticipantType { get; private set; }
    
    /// <summary>
    /// Reference to the TourPricing that was used to calculate this price.
    /// Nullable for backward compatibility with old snapshots.
    /// </summary>
    public Guid? TourPricingId { get; private set; }
    
    public Money BasePrice { get; private set; } = null!;
    public Money? DiscountAmount { get; private set; }
    public Money FinalPrice { get; private set; } = null!;
    public string? DiscountCode { get; private set; }
    public string? DiscountDescription { get; private set; }
    public DateTime SnapshotDate { get; private set; }
    
    /// <summary>
    /// JSON serialized pricing rule details including capabilities, features, and other metadata.
    /// Maintains backward compatibility and provides flexible storage.
    /// </summary>
    public string? PricingRules { get; private set; }
    
    /// <summary>
    /// Required capability IDs that were used to select this pricing (JSON array).
    /// Null if pricing had no capability requirements.
    /// </summary>
    public string? RequiredCapabilityIds { get; private set; }
    
    /// <summary>
    /// Required feature IDs that were used to select this pricing (JSON array).
    /// Null if pricing had no feature requirements.
    /// </summary>
    public string? RequiredFeatureIds { get; private set; }
    
    /// <summary>
    /// Indicates if this was the default pricing for the participant type.
    /// </summary>
    public bool WasDefaultPricing { get; private set; }
    
    /// <summary>
    /// Discount percentage applied from the pricing rule (0-100).
    /// </summary>
    public decimal? AppliedDiscountPercentage { get; private set; }
    
    /// <summary>
    /// Indicates if this pricing was marked as early bird.
    /// </summary>
    public bool WasEarlyBird { get; private set; }
    
    /// <summary>
    /// Indicates if this pricing was marked as last minute.
    /// </summary>
    public bool WasLastMinute { get; private set; }
    
    // Multi-tenancy support
    public string? TenantId { get; private set; }

    // Navigation property
    public TourReservation Reservation { get; private set; } = null!;

    // Private constructor for EF Core
    private ReservationPriceSnapshot() : base() { }

    /// <summary>
    /// Creates a new price snapshot (legacy constructor for backward compatibility).
    /// </summary>
    public ReservationPriceSnapshot(
        Guid reservationId,
        ParticipantType participantType,
        Money basePrice,
        Money finalPrice,
        Money? discountAmount = null,
        string? discountCode = null,
        string? discountDescription = null,
        string? pricingRules = null,
        string? tenantId = null)
        : base(Guid.NewGuid())
    {
        if (reservationId == Guid.Empty)
            throw new ArgumentException("Reservation ID cannot be empty", nameof(reservationId));

        ReservationId = reservationId;
        ParticipantType = participantType;
        BasePrice = basePrice ?? throw new ArgumentNullException(nameof(basePrice));
        FinalPrice = finalPrice ?? throw new ArgumentNullException(nameof(finalPrice));
        DiscountAmount = discountAmount;
        DiscountCode = discountCode?.Trim();
        DiscountDescription = discountDescription?.Trim();
        PricingRules = pricingRules?.Trim();
        TenantId = tenantId;
        SnapshotDate = DateTime.UtcNow;
        TourPricingId = null;
        WasDefaultPricing = false;
        AppliedDiscountPercentage = null;
        WasEarlyBird = false;
        WasLastMinute = false;
    }

    /// <summary>
    /// Creates a price snapshot from a TourPricing entity.
    /// This is the recommended way to create snapshots with full pricing rule information.
    /// </summary>
    public static ReservationPriceSnapshot CreateFromPricing(
        Guid reservationId,
        ParticipantType participantType,
        TourPricing pricing,
        IEnumerable<string>? memberCapabilityIds = null,
        IEnumerable<string>? memberFeatureIds = null,
        Money? discountAmount = null,
        string? discountCode = null,
        string? discountDescription = null,
        string? tenantId = null)
    {
        if (reservationId == Guid.Empty)
            throw new ArgumentException("Reservation ID cannot be empty", nameof(reservationId));
        if (pricing == null)
            throw new ArgumentNullException(nameof(pricing));

        var effectivePrice = pricing.GetEffectivePrice();
        var finalPrice = discountAmount != null 
            ? effectivePrice.Subtract(discountAmount) 
            : effectivePrice;

        var capabilityIds = pricing.Capabilities.Select(c => c.CapabilityId).ToList();
        var featureIds = pricing.Features.Select(f => f.FeatureId).ToList();

        var snapshot = new ReservationPriceSnapshot(
            reservationId: reservationId,
            participantType: participantType,
            basePrice: pricing.BasePrice,
            finalPrice: finalPrice,
            discountAmount: discountAmount,
            discountCode: discountCode,
            discountDescription: discountDescription,
            tenantId: tenantId)
        {
            TourPricingId = pricing.Id,
            WasDefaultPricing = pricing.IsDefault,
            AppliedDiscountPercentage = pricing.DiscountPercentage,
            WasEarlyBird = pricing.IsEarlyBird,
            WasLastMinute = pricing.IsLastMinute,
            RequiredCapabilityIds = capabilityIds.Any() ? System.Text.Json.JsonSerializer.Serialize(capabilityIds) : null,
            RequiredFeatureIds = featureIds.Any() ? System.Text.Json.JsonSerializer.Serialize(featureIds) : null
        };

        // Build comprehensive pricing rules JSON
        var pricingRules = new
        {
            PricingId = pricing.Id,
            ParticipantType = participantType.ToString(),
            BasePrice = pricing.BasePrice.AmountRials,
            DiscountPercentage = pricing.DiscountPercentage,
            EffectivePrice = effectivePrice.AmountRials,
            FinalPrice = finalPrice.AmountRials,
            IsDefault = pricing.IsDefault,
            IsEarlyBird = pricing.IsEarlyBird,
            IsLastMinute = pricing.IsLastMinute,
            RequiredCapabilities = capabilityIds,
            RequiredFeatures = featureIds,
            MemberCapabilities = memberCapabilityIds?.ToList(),
            MemberFeatures = memberFeatureIds?.ToList(),
            AppliedAt = DateTime.UtcNow,
            ValidFrom = pricing.ValidFrom,
            ValidTo = pricing.ValidTo,
            Description = pricing.Description
        };

        snapshot.PricingRules = System.Text.Json.JsonSerializer.Serialize(pricingRules);
        return snapshot;
    }

    /// <summary>
    /// Updates discount information
    /// </summary>
    public void UpdateDiscount(Money? discountAmount, string? discountCode, string? discountDescription)
    {
        DiscountAmount = discountAmount;
        DiscountCode = discountCode?.Trim();
        DiscountDescription = discountDescription?.Trim();
        
        // Recalculate final price
        FinalPrice = BasePrice.Subtract(discountAmount ?? Money.Zero);
    }

    /// <summary>
    /// Checks if discount is applied
    /// </summary>
    public bool HasDiscount => DiscountAmount != null && DiscountAmount.AmountRials > 0;

    /// <summary>
    /// Gets the effective discount percentage
    /// </summary>
    public decimal DiscountPercentage => BasePrice.AmountRials > 0 && HasDiscount 
        ? (decimal)(DiscountAmount!.AmountRials * 100) / BasePrice.AmountRials 
        : 0;

    /// <summary>
    /// Gets the required capability IDs that were used for this pricing.
    /// </summary>
    public IEnumerable<string> GetRequiredCapabilityIds()
    {
        if (string.IsNullOrWhiteSpace(RequiredCapabilityIds))
            return Enumerable.Empty<string>();

        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<List<string>>(RequiredCapabilityIds) 
                ?? Enumerable.Empty<string>();
        }
        catch
        {
            return Enumerable.Empty<string>();
        }
    }

    /// <summary>
    /// Gets the required feature IDs that were used for this pricing.
    /// </summary>
    public IEnumerable<string> GetRequiredFeatureIds()
    {
        if (string.IsNullOrWhiteSpace(RequiredFeatureIds))
            return Enumerable.Empty<string>();

        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<List<string>>(RequiredFeatureIds) 
                ?? Enumerable.Empty<string>();
        }
        catch
        {
            return Enumerable.Empty<string>();
        }
    }
}
