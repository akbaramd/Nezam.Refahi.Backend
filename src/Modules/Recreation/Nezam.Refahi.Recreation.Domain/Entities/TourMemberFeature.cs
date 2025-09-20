using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Recreation.Domain.Entities;

/// <summary>
/// Middle table entity linking Tour with required member features
/// Defines which member features are required for tour participation
/// </summary>
public sealed class TourMemberFeature : Entity<Guid>
{
    public Guid TourId { get; private set; }
    public string FeatureId { get; private set; } = string.Empty;

    // Navigation properties
    public Tour Tour { get; private set; } = null!;

    // Private constructor for EF Core
    private TourMemberFeature() : base() { }

    /// <summary>
    /// Creates a new tour member feature requirement
    /// </summary>
    public TourMemberFeature(Guid tourId, string featureId)
        : base(Guid.NewGuid())
    {
        if (tourId == Guid.Empty)
            throw new ArgumentException("Tour ID cannot be empty", nameof(tourId));
        if (string.IsNullOrWhiteSpace(featureId))
            throw new ArgumentException("Feature ID cannot be empty", nameof(featureId));

        TourId = tourId;
        FeatureId = featureId.Trim();
    }


}