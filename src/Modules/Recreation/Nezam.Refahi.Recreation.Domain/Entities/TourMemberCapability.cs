using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Recreation.Domain.Entities;

/// <summary>
/// Middle table entity linking Tour with required member capabilities
/// Defines which member capabilities are required for tour participation
/// </summary>
public sealed class TourMemberCapability : Entity<Guid>
{
    public Guid TourId { get; private set; }
    public string CapabilityId { get; private set; } = string.Empty;

    // Navigation properties
    public Tour Tour { get; private set; } = null!;

    // Private constructor for EF Core
    private TourMemberCapability() : base() { }

    /// <summary>
    /// Creates a new tour member capability requirement
    /// </summary>
    public TourMemberCapability(Guid tourId, string capabilityId)
        : base(Guid.NewGuid())
    {
        if (tourId == Guid.Empty)
            throw new ArgumentException("Tour ID cannot be empty", nameof(tourId));
        if (string.IsNullOrWhiteSpace(capabilityId))
            throw new ArgumentException("Capability ID cannot be empty", nameof(capabilityId));

        TourId = tourId;
        CapabilityId = capabilityId.Trim();
    }

}