using MCA.SharedKernel.Domain;
using Nezam.Refahi.Facilities.Domain.Enums;

namespace Nezam.Refahi.Facilities.Domain.Entities;

/// <summary>
/// نگاشت بین تسهیلات و ویژگی‌ها با مشخصات الزام، محدودیت یا تعدیل شرایط
/// در دنیای واقعی: تعریف قوانین خاص تسهیلات بر اساس ویژگی‌های عضو
/// </summary>
public sealed class FacilityFeature : Entity<Guid>
{
    public Guid FacilityId { get; private set; }
    public string FeatureId { get; private set; } = null!;
    public FacilityFeatureRequirementType RequirementType { get; private set; }
    public string? Notes { get; private set; }
    public DateTime AssignedAt { get; private set; }

    // Navigation properties
    public Facility Facility { get; private set; } = null!;

    // Private constructor for EF Core
    private FacilityFeature() : base() { }

    /// <summary>
    /// ایجاد نگاشت ویژگی تسهیلات
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار یک نگاشت بین تسهیلات و ویژگی عضو ایجاد می‌کند که نوع الزام
    /// (الزام، ممنوعیت، تعدیل) را مشخص می‌نماید.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که نیاز به تعریف قوانین خاص تسهیلات بر اساس ویژگی‌های عضو وجود دارد،
    /// این رفتار برای ایجاد نگاشت استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - شناسه تسهیلات و ویژگی اجباری است.
    /// - نوع الزام باید مشخص باشد.
    /// - زمان اختصاص ثبت می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: شناسه‌های معتبر ارائه شود.
    /// - باید: نوع الزام مشخص باشد.
    /// - نباید: نگاشت تکراری ایجاد شود.
    /// - نباید: بدون شناسه‌های ضروری ایجاد شود.
    /// </remarks>
    public FacilityFeature(
        Guid facilityId,
        string featureId,
        FacilityFeatureRequirementType requirementType,
        string? notes = null)
        : base(Guid.NewGuid())
    {
        if (facilityId == Guid.Empty)
            throw new ArgumentException("Facility ID cannot be empty", nameof(facilityId));
        if (string.IsNullOrWhiteSpace(featureId))
            throw new ArgumentException("Feature ID cannot be empty", nameof(featureId));

        FacilityId = facilityId;
        FeatureId = featureId.Trim();
        RequirementType = requirementType;
        Notes = notes?.Trim();
        AssignedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// به‌روزرسانی یادداشت‌های نگاشت
    /// </summary>
    public void UpdateNotes(string? notes)
    {
        Notes = notes?.Trim();
    }

    /// <summary>
    /// تغییر نوع الزام
    /// </summary>
    public void ChangeRequirementType(FacilityFeatureRequirementType newType)
    {
        RequirementType = newType;
    }
}
