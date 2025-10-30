using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Recreation.Domain.Entities;

/// <summary>
/// Many-to-many relationship between TourPricing and Capability.
/// Represents that a pricing rule applies when member has specific capability.
/// </summary>
public sealed class TourPricingCapability : Entity<Guid>
{
    public Guid TourPricingId { get; private set; }
    public string CapabilityId { get; private set; } = null!;

    // Navigation properties
    public TourPricing TourPricing { get; private set; } = null!;

    // EF Core
    private TourPricingCapability() : base() { }

    public TourPricingCapability(Guid tourPricingId, string capabilityId)
        : base(Guid.NewGuid())
    {
        if (tourPricingId == Guid.Empty)
            throw new ArgumentException("TourPricing ID cannot be empty", nameof(tourPricingId));
        if (string.IsNullOrWhiteSpace(capabilityId))
            throw new ArgumentException("Capability ID cannot be empty", nameof(capabilityId));

        TourPricingId = tourPricingId;
        CapabilityId = capabilityId.Trim();
    }
}

