using MCA.SharedKernel.Domain;
using Nezam.Refahi.Facilities.Domain.Enums;

namespace Nezam.Refahi.Facilities.Domain.Entities;

/// <summary>
/// نگاشت بین تسهیلات و قابلیت‌ها
/// در دنیای واقعی: تعریف قابلیت‌های مورد نیاز برای استفاده از تسهیلات
/// </summary>
public sealed class FacilityCapability : Entity<Guid>
{
    public Guid FacilityId { get; private set; }
    public string CapabilityId { get; private set; } = null!;

    // Navigation properties
    public Facility Facility { get; private set; } = null!;

    // Private constructor for EF Core
    private FacilityCapability() : base() { }

    /// <summary>
    /// ایجاد قابلیت تسهیلات
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار یک قابلیت برای تسهیلات ایجاد می‌کند.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که نیاز به تعریف قابلیت‌های تسهیلات وجود دارد،
    /// این رفتار برای ایجاد قابلیت استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - شناسه تسهیلات و قابلیت اجباری است.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: شناسه‌های معتبر ارائه شود.
    /// - نباید: قابلیت تکراری ایجاد شود.
    /// </remarks>
    public FacilityCapability(
        Guid facilityId,
        string capabilityId)
        : base(Guid.NewGuid())
    {
        if (facilityId == Guid.Empty)
            throw new ArgumentException("Facility ID cannot be empty", nameof(facilityId));
        if (string.IsNullOrWhiteSpace(capabilityId))
            throw new ArgumentException("Capability ID cannot be empty", nameof(capabilityId));

        FacilityId = facilityId;
        CapabilityId = capabilityId.Trim();
    }

}
