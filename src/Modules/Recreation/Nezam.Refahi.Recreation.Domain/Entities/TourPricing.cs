using MCA.SharedKernel.Domain;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Recreation.Domain.Entities;

/// <summary>
/// Pricing rule per participant type for a tour.
/// Uses decimal for price arithmetic; exposes Money for persistence and invariants.
/// </summary>
public sealed class TourPricing : Entity<Guid>
{
    // Required
    public Guid TourId { get; private set; }
    public ParticipantType ParticipantType { get; private set; }

    /// <summary>Base price before any discount.</summary>
    public Money BasePrice { get; private set; } = null!;

    // Optional metadata
    public string? Description { get; private set; }

    // Validity window (optional)
    public DateTime? ValidFrom { get; private set; }
    public DateTime? ValidTo   { get; private set; }

    // State
    public bool IsActive { get; private set; }
    
    /// <summary>
    /// Indicates if this is the default pricing for the participant type.
    /// Only one default pricing per participant type should exist.
    /// </summary>
    public bool IsDefault { get; private set; }

    // Optional quantity gates
    public int? MinQuantity { get; private set; }
    public int? MaxQuantity { get; private set; }

    /// <summary>Percentage [0..100]. Null or 0 means no discount.</summary>
    public decimal? DiscountPercentage { get; private set; }

    /// <summary>Flags for marketing/UX ribbons.</summary>
    public bool IsEarlyBird { get; private set; }
    public bool IsLastMinute { get; private set; }

    // Navigation
    public Tour Tour { get; private set; } = null!;
    
    // Many-to-many relationships with Capabilities and Features
    private readonly List<TourPricingCapability> _capabilities = new();
    public IReadOnlyCollection<TourPricingCapability> Capabilities => _capabilities.AsReadOnly();
    
    private readonly List<TourPricingFeature> _features = new();
    public IReadOnlyCollection<TourPricingFeature> Features => _features.AsReadOnly();

    // EF Core
    private TourPricing() : base() { }

    public TourPricing(
        Guid tourId,
        ParticipantType participantType,
        Money basePrice,
        string? description = null,
        DateTime? validFrom = null,
        DateTime? validTo = null,
        int? minQuantity = null,
        int? maxQuantity = null,
        decimal? discountPercentage = null,
        bool isEarlyBird = false,
        bool isLastMinute = false,
        bool isDefault = false)
        : base(Guid.NewGuid())
    {
        if (tourId == Guid.Empty)
            throw new ArgumentException("Tour ID cannot be empty.", nameof(tourId));

        ValidateDates(validFrom, validTo);
        ValidateQuantities(minQuantity, maxQuantity);
        ValidateDiscount(discountPercentage);

        TourId = tourId;
        ParticipantType = participantType;
        BasePrice = basePrice ?? throw new ArgumentNullException(nameof(basePrice));
        Description = description?.Trim();
        ValidFrom = validFrom;
        ValidTo = validTo;
        IsActive = true;
        IsDefault = isDefault;
        MinQuantity = minQuantity;
        MaxQuantity = maxQuantity;
        DiscountPercentage = discountPercentage;
        IsEarlyBird = isEarlyBird;
        IsLastMinute = isLastMinute;
    }

    // -----------------
    // Mutations (setters)
    // -----------------

    public void UpdatePrice(Money newBasePrice, string? description = null)
    {
        BasePrice = newBasePrice ?? throw new ArgumentNullException(nameof(newBasePrice));
        if (description is not null) Description = description.Trim();
    }

    public void SetValidityPeriod(DateTime? validFrom, DateTime? validTo)
    {
        ValidateDates(validFrom, validTo);
        ValidFrom = validFrom;
        ValidTo = validTo;
    }

    public void SetQuantityConstraints(int? minQuantity, int? maxQuantity)
    {
        ValidateQuantities(minQuantity, maxQuantity);
        MinQuantity = minQuantity;
        MaxQuantity = maxQuantity;
    }

    public void SetDiscount(decimal? discountPercentage)
    {
        ValidateDiscount(discountPercentage);
        DiscountPercentage = discountPercentage;
    }

    public void MarkEarlyBird(bool value = true) => IsEarlyBird = value;
    public void MarkLastMinute(bool value = true) => IsLastMinute = value;
    
    public void SetAsDefault(bool value = true) => IsDefault = value;

    public void Activate()   => IsActive = true;
    public void Deactivate() => IsActive = false;

    // -----------------
    // Queries/Checks
    // -----------------

    /// <summary>
    /// True if active, within date window (if any), and within quantity window (if any).
    /// </summary>
    public bool IsValidFor(DateTime onDateUtc, int quantity)
    {
        if (!IsActive) return false;

        if (ValidFrom.HasValue && onDateUtc < ValidFrom.Value) return false;
        if (ValidTo.HasValue   && onDateUtc > ValidTo.Value)   return false;

        if (MinQuantity.HasValue && quantity < MinQuantity.Value) return false;
        if (MaxQuantity.HasValue && quantity > MaxQuantity.Value) return false;

        return true;
    }

    /// <summary>
    /// Checks if this pricing matches the given capabilities and features.
    /// Returns true if pricing has no specific requirements (no capabilities/features) or if all requirements are met.
    /// </summary>
    public bool MatchesCapabilitiesAndFeatures(IEnumerable<string>? memberCapabilities = null, IEnumerable<string>? memberFeatures = null)
    {
        var hasCapabilityRequirements = _capabilities.Any();
        var hasFeatureRequirements = _features.Any();

        // If no requirements, pricing applies to all
        if (!hasCapabilityRequirements && !hasFeatureRequirements)
            return true;

        // If requirements exist, member must satisfy them
        var memberCapabilitySet = memberCapabilities?.ToHashSet(StringComparer.OrdinalIgnoreCase) ?? new HashSet<string>();
        var memberFeatureSet = memberFeatures?.ToHashSet(StringComparer.OrdinalIgnoreCase) ?? new HashSet<string>();

        // Check capabilities: member must have ALL required capabilities
        if (hasCapabilityRequirements)
        {
            var requiredCapabilities = _capabilities.Select(c => c.CapabilityId).ToHashSet(StringComparer.OrdinalIgnoreCase);
            if (!requiredCapabilities.IsSubsetOf(memberCapabilitySet))
                return false;
        }

        // Check features: member must have ALL required features
        if (hasFeatureRequirements)
        {
            var requiredFeatures = _features.Select(f => f.FeatureId).ToHashSet(StringComparer.OrdinalIgnoreCase);
            if (!requiredFeatures.IsSubsetOf(memberFeatureSet))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Checks if this pricing has any capability or feature requirements.
    /// </summary>
    public bool HasRequirements()
    {
        return _capabilities.Any() || _features.Any();
    }

    /// <summary>
    /// Base price minus percentage discount (if any). Rounded away-from-zero to the nearest rial.
    /// </summary>
    public Money GetEffectivePrice()
    {
        // Normalize to decimal for math regardless of Money’s internal representation.
        // If Money.AmountRials is long, Convert.ToDecimal works. If it's already decimal, no loss.
        var baseRials = Convert.ToDecimal(BasePrice.AmountRials);

        if (DiscountPercentage is null || DiscountPercentage.Value <= 0m)
            return BasePrice;

        var pct = DiscountPercentage.Value / 100m;
        var discounted = baseRials * (1m - pct);

        // Round to 0 decimals (rial) consistently
        var rounded = decimal.Round(discounted, 0, MidpointRounding.AwayFromZero);

        // Prefer a decimal factory if your Money supports it; otherwise cast to long safely.
        // Example 1: if you have a factory:
        // return Money.FromRials(rounded);

        // Example 2: if constructor expects long:
        return new Money((long)rounded);
    }

    // -----------------
    // Validation
    // -----------------

    private static void ValidateQuantities(int? minQuantity, int? maxQuantity)
    {
        if (minQuantity is < 0) throw new ArgumentException("Minimum quantity cannot be negative.", nameof(minQuantity));
        if (maxQuantity is < 0) throw new ArgumentException("Maximum quantity cannot be negative.", nameof(maxQuantity));
        if (minQuantity.HasValue && maxQuantity.HasValue && minQuantity.Value > maxQuantity.Value)
            throw new ArgumentException("Minimum quantity cannot be greater than maximum quantity.");
    }

    private static void ValidateDiscount(decimal? discountPercentage)
    {
        if (discountPercentage is null) return;
        if (discountPercentage < 0m || discountPercentage > 100m)
            throw new ArgumentException("Discount percentage must be in the range [0, 100].", nameof(discountPercentage));
    }

    private static void ValidateDates(DateTime? validFrom, DateTime? validTo)
    {
        if (validFrom.HasValue && validTo.HasValue && validFrom.Value >= validTo.Value)
            throw new ArgumentException("ValidFrom must be earlier than ValidTo.");
    }

    // -----------------
    // Capability and Feature management
    // -----------------

    /// <summary>
    /// Adds a capability requirement to this pricing rule.
    /// </summary>
    public void AddCapability(string capabilityId)
    {
        if (string.IsNullOrWhiteSpace(capabilityId))
            throw new ArgumentException("Capability ID cannot be empty", nameof(capabilityId));

        var trimmedId = capabilityId.Trim();
        if (_capabilities.Any(c => c.CapabilityId.Equals(trimmedId, StringComparison.OrdinalIgnoreCase)))
            return; // Already exists

        var pricingCapability = new TourPricingCapability(Id, trimmedId);
        _capabilities.Add(pricingCapability);
    }

    /// <summary>
    /// Removes a capability requirement from this pricing rule.
    /// </summary>
    public void RemoveCapability(string capabilityId)
    {
        if (string.IsNullOrWhiteSpace(capabilityId))
            return;

        var capabilityToRemove = _capabilities.FirstOrDefault(c => 
            c.CapabilityId.Equals(capabilityId.Trim(), StringComparison.OrdinalIgnoreCase));
        
        if (capabilityToRemove != null)
            _capabilities.Remove(capabilityToRemove);
    }

    /// <summary>
    /// Sets multiple capability requirements for this pricing rule (replaces existing).
    /// </summary>
    public void SetCapabilities(IEnumerable<string> capabilityIds)
    {
        if (capabilityIds == null)
            throw new ArgumentNullException(nameof(capabilityIds));

        _capabilities.Clear();
        foreach (var capabilityId in capabilityIds)
        {
            if (!string.IsNullOrWhiteSpace(capabilityId))
                AddCapability(capabilityId);
        }
    }

    /// <summary>
    /// Adds a feature requirement to this pricing rule.
    /// </summary>
    public void AddFeature(string featureId)
    {
        if (string.IsNullOrWhiteSpace(featureId))
            throw new ArgumentException("Feature ID cannot be empty", nameof(featureId));

        var trimmedId = featureId.Trim();
        if (_features.Any(f => f.FeatureId.Equals(trimmedId, StringComparison.OrdinalIgnoreCase)))
            return; // Already exists

        var pricingFeature = new TourPricingFeature(Id, trimmedId);
        _features.Add(pricingFeature);
    }

    /// <summary>
    /// Removes a feature requirement from this pricing rule.
    /// </summary>
    public void RemoveFeature(string featureId)
    {
        if (string.IsNullOrWhiteSpace(featureId))
            return;

        var featureToRemove = _features.FirstOrDefault(f => 
            f.FeatureId.Equals(featureId.Trim(), StringComparison.OrdinalIgnoreCase));
        
        if (featureToRemove != null)
            _features.Remove(featureToRemove);
    }

    /// <summary>
    /// Sets multiple feature requirements for this pricing rule (replaces existing).
    /// </summary>
    public void SetFeatures(IEnumerable<string> featureIds)
    {
        if (featureIds == null)
            throw new ArgumentNullException(nameof(featureIds));

        _features.Clear();
        foreach (var featureId in featureIds)
        {
            if (!string.IsNullOrWhiteSpace(featureId))
                AddFeature(featureId);
        }
    }
}
