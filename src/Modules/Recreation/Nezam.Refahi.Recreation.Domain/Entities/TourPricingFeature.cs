using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Recreation.Domain.Entities;

/// <summary>
/// Many-to-many relationship between TourPricing and Feature.
/// Represents that a pricing rule applies when member has specific feature.
/// </summary>
public sealed class TourPricingFeature : Entity<Guid>
{
    public Guid TourPricingId { get; private set; }
    public string FeatureId { get; private set; } = null!;

    // Navigation properties
    public TourPricing TourPricing { get; private set; } = null!;

    // EF Core
    private TourPricingFeature() : base() { }

    public TourPricingFeature(Guid tourPricingId, string featureId)
        : base(Guid.NewGuid())
    {
        if (tourPricingId == Guid.Empty)
            throw new ArgumentException("TourPricing ID cannot be empty", nameof(tourPricingId));
        if (string.IsNullOrWhiteSpace(featureId))
            throw new ArgumentException("Feature ID cannot be empty", nameof(featureId));

        TourPricingId = tourPricingId;
        FeatureId = featureId.Trim();
    }
}

