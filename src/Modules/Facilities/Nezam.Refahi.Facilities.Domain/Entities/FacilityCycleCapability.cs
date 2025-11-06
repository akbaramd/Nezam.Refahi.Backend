using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Facilities.Domain.Entities;

/// <summary>
/// نگاشت بین دوره تسهیلات و قابلیت‌ها
/// فقط ID قابلیت را نگه می‌دارد
/// </summary>
public sealed class FacilityCycleCapability : Entity<Guid>
{
    public Guid FacilityCycleId { get; private set; }
    public string CapabilityId { get; private set; } = null!;

    // Navigation properties
    public FacilityCycle FacilityCycle { get; private set; } = null!;

    // Private constructor for EF Core
    private FacilityCycleCapability() : base() { }

    /// <summary>
    /// ایجاد نگاشت قابلیت دوره
    /// </summary>
    public FacilityCycleCapability(
        Guid facilityCycleId,
        string capabilityId)
        : base(Guid.NewGuid())
    {
        if (facilityCycleId == Guid.Empty)
            throw new ArgumentException("Facility cycle ID cannot be empty", nameof(facilityCycleId));
        if (string.IsNullOrWhiteSpace(capabilityId))
            throw new ArgumentException("Capability ID cannot be empty", nameof(capabilityId));

        FacilityCycleId = facilityCycleId;
        CapabilityId = capabilityId.Trim();
    }
}

