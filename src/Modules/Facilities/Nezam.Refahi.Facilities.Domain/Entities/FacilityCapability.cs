using MCA.SharedKernel.Domain;
using Nezam.Refahi.Facilities.Domain.Enums;

namespace Nezam.Refahi.Facilities.Domain.Entities;

/// <summary>
/// نگاشت بین تسهیلات و قابلیت‌ها با قواعد دامنه‌ای
/// در دنیای واقعی: تعریف سیاست‌های خاص تسهیلات بر اساس قابلیت‌های عضو
/// </summary>
public sealed class FacilityCapabilityPolicy : Entity<Guid>
{
    public Guid FacilityId { get; private set; }
    public string CapabilityId { get; private set; } = null!;
    public CapabilityPolicyType PolicyType { get; private set; }
    public decimal? ModifierValue { get; private set; }
    public string? Notes { get; private set; }
    public DateTime AssignedAt { get; private set; }

    // Navigation properties
    public Facility Facility { get; private set; } = null!;

    // Private constructor for EF Core
    private FacilityCapabilityPolicy() : base() { }

    /// <summary>
    /// ایجاد سیاست قابلیت تسهیلات
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار یک سیاست قابلیت برای تسهیلات ایجاد می‌کند که نوع سیاست
    /// (الزام، ممنوعیت، تعدیل مبلغ، تعدیل سهمیه، تعدیل اولویت) را مشخص می‌نماید.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که نیاز به تعریف قوانین خاص تسهیلات بر اساس قابلیت‌های عضو وجود دارد،
    /// این رفتار برای ایجاد سیاست استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - شناسه تسهیلات و قابلیت اجباری است.
    /// - نوع سیاست باید مشخص باشد.
    /// - مقدار تعدیل برای انواع تعدیل اجباری است.
    /// - زمان اختصاص ثبت می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: شناسه‌های معتبر ارائه شود.
    /// - باید: نوع سیاست مشخص باشد.
    /// - نباید: سیاست تکراری ایجاد شود.
    /// - نباید: بدون مقدار تعدیل برای انواع تعدیل ایجاد شود.
    /// </remarks>
    public FacilityCapabilityPolicy(
        Guid facilityId,
        string capabilityId,
        CapabilityPolicyType policyType,
        decimal? modifierValue = null,
        string? notes = null)
        : base(Guid.NewGuid())
    {
        if (facilityId == Guid.Empty)
            throw new ArgumentException("Facility ID cannot be empty", nameof(facilityId));
        if (string.IsNullOrWhiteSpace(capabilityId))
            throw new ArgumentException("Capability ID cannot be empty", nameof(capabilityId));

        // Validate modifier value for modifier types
        if ((policyType == CapabilityPolicyType.AmountModifier || 
             policyType == CapabilityPolicyType.QuotaModifier || 
             policyType == CapabilityPolicyType.PriorityModifier) && 
            !modifierValue.HasValue)
        {
            throw new ArgumentException("Modifier value is required for modifier policy types", nameof(modifierValue));
        }

        FacilityId = facilityId;
        CapabilityId = capabilityId.Trim();
        PolicyType = policyType;
        ModifierValue = modifierValue;
        Notes = notes?.Trim();
        AssignedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// به‌روزرسانی یادداشت‌های سیاست
    /// </summary>
    public void UpdateNotes(string? notes)
    {
        Notes = notes?.Trim();
    }

    /// <summary>
    /// تغییر نوع سیاست
    /// </summary>
    public void ChangePolicyType(CapabilityPolicyType newType, decimal? newModifierValue = null)
    {
        // Validate modifier value for modifier types
        if ((newType == CapabilityPolicyType.AmountModifier || 
             newType == CapabilityPolicyType.QuotaModifier || 
             newType == CapabilityPolicyType.PriorityModifier) && 
            !newModifierValue.HasValue)
        {
            throw new ArgumentException("Modifier value is required for modifier policy types", nameof(newModifierValue));
        }

        PolicyType = newType;
        ModifierValue = newModifierValue;
    }

    /// <summary>
    /// به‌روزرسانی مقدار تعدیل
    /// </summary>
    public void UpdateModifierValue(decimal? newValue)
    {
        if ((PolicyType == CapabilityPolicyType.AmountModifier || 
             PolicyType == CapabilityPolicyType.QuotaModifier || 
             PolicyType == CapabilityPolicyType.PriorityModifier) && 
            !newValue.HasValue)
        {
            throw new ArgumentException("Modifier value is required for modifier policy types");
        }

        ModifierValue = newValue;
    }

    /// <summary>
    /// بررسی اینکه آیا سیاست تعدیل است
    /// </summary>
    public bool IsModifierPolicy()
    {
        return PolicyType == CapabilityPolicyType.AmountModifier ||
               PolicyType == CapabilityPolicyType.QuotaModifier ||
               PolicyType == CapabilityPolicyType.PriorityModifier;
    }

    /// <summary>
    /// بررسی اینکه آیا سیاست الزام است
    /// </summary>
    public bool IsRequiredPolicy()
    {
        return PolicyType == CapabilityPolicyType.Required;
    }

    /// <summary>
    /// بررسی اینکه آیا سیاست ممنوعیت است
    /// </summary>
    public bool IsProhibitedPolicy()
    {
        return PolicyType == CapabilityPolicyType.Prohibited;
    }
}
