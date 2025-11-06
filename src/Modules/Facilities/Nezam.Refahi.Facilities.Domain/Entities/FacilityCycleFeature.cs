using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Facilities.Domain.Entities;

/// <summary>
/// نگاشت بین دوره تسهیلات و ویژگی‌ها
/// فقط ID ویژگی را نگه می‌دارد
/// </summary>
public sealed class FacilityCycleFeature : Entity<Guid>
{
    public Guid FacilityCycleId { get; private set; }
    public string FeatureId { get; private set; } = null!;

    // Navigation properties
    public FacilityCycle FacilityCycle { get; private set; } = null!;

    // Private constructor for EF Core
    private FacilityCycleFeature() : base() { }

    /// <summary>
    /// ایجاد نگاشت ویژگی دوره
    /// </summary>
    public FacilityCycleFeature(
        Guid facilityCycleId,
        string featureId)
        : base(Guid.NewGuid())
    {
        if (facilityCycleId == Guid.Empty)
            throw new ArgumentException("Facility cycle ID cannot be empty", nameof(facilityCycleId));
        if (string.IsNullOrWhiteSpace(featureId))
            throw new ArgumentException("Feature ID cannot be empty", nameof(featureId));

        FacilityCycleId = facilityCycleId;
        FeatureId = featureId.Trim();
    }
}

