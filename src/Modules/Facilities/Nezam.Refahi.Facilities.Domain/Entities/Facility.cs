using MCA.SharedKernel.Domain.AggregateRoots;
using Nezam.Refahi.Facilities.Domain.Enums;
using Nezam.Refahi.Facilities.Domain.Events;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Facilities.Domain.Entities;

/// <summary>
/// تسهیلات - تعریف نوع تسهیلات و سیاست‌های کلی
/// در دنیای واقعی: وام، کمک‌هزینه، کارت، بن رفاهی و سایر تسهیلات سازمان
/// </summary>
public sealed class Facility : FullAggregateRoot<Guid>
{
    public string Name { get; private set; } = null!;
    public string Code { get; private set; } = null!;
    public FacilityType Type { get; private set; }
    public FacilityStatus Status { get; private set; }
    public string? Description { get; private set; }
    
    // Bank information
    public string? BankName { get; private set; }
    public string? BankCode { get; private set; }
    public string? BankAccountNumber { get; private set; }
    
    // Basic metadata
    public Dictionary<string, string> Metadata { get; private set; } = new();

    // Navigation properties
    private readonly List<FacilityCycle> _cycles = new();
    public IReadOnlyCollection<FacilityCycle> Cycles => _cycles.AsReadOnly();

    private readonly List<FacilityFeature> _features = new();
    public IReadOnlyCollection<FacilityFeature> Features => _features.AsReadOnly();

    private readonly List<FacilityCapabilityPolicy> _capabilityPolicies = new();
    public IReadOnlyCollection<FacilityCapabilityPolicy> CapabilityPolicies => _capabilityPolicies.AsReadOnly();

    // Private constructor for EF Core
    private Facility() : base() { }

    /// <summary>
    /// ایجاد تسهیلات جدید
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار یک تسهیلات جدید با اطلاعات ابتدایی ایجاد می‌کند که شامل نام، کد، نوع،
    /// مبلغ پیش‌فرض و سایر تنظیمات اولیه است.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که سازمان نیاز به تعریف نوع جدیدی از تسهیلات دارد، این رفتار برای
    /// ایجاد تعریف اولیه تسهیلات استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - نام و کد تسهیلات اجباری و یکتا باشد.
    /// - نوع تسهیلات باید مشخص باشد.
    /// - مبلغ پیش‌فرض اختیاری است.
    /// - وضعیت اولیه همیشه Draft خواهد بود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: نام و کد معتبر ارائه شود.
    /// - باید: نوع تسهیلات مشخص باشد.
    /// - نباید: کد تکراری استفاده شود.
    /// - نباید: مبلغ منفی تعیین شود.
    /// </remarks>
    public Facility(
        string name,
        string code,
        FacilityType type,
        string? description = null,
        string? bankName = null,
        string? bankCode = null,
        string? bankAccountNumber = null,
        Dictionary<string, string>? metadata = null)
        : base(Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code cannot be empty", nameof(code));

        Name = name.Trim();
        Code = code.Trim();
        Type = type;
        Status = FacilityStatus.Draft;
        Description = description?.Trim();
        
        // Bank information
        BankName = bankName?.Trim();
        BankCode = bankCode?.Trim();
        BankAccountNumber = bankAccountNumber?.Trim();
        
        Metadata = metadata ?? new Dictionary<string, string>();

        // Raise domain event
        AddDomainEvent(new FacilityCreatedEvent(
            Id,
            Name,
            Code,
            Type,
            null, // No default amount anymore
            CreatedAt));
    }

    /// <summary>
    /// فعال کردن تسهیلات
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار تسهیلات را از حالت پیش‌نویس به حالت فعال تغییر می‌دهد
    /// و آن را آماده برای ایجاد دوره‌ها و دریافت درخواست‌ها می‌نماید.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که تعریف تسهیلات تکمیل شده و آماده ارائه به اعضا است،
    /// این رفتار برای فعال کردن تسهیلات استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - فقط تسهیلات پیش‌نویس قابل فعال کردن هستند.
    /// - وضعیت به Active تغییر می‌یابد.
    /// - زمان فعال‌سازی ثبت می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: فقط از حالت Draft قابل تغییر باشد.
    /// - باید: زمان فعال‌سازی دقیق ثبت شود.
    /// - نباید: تسهیلات غیر Draft فعال شوند.
    /// - نباید: بدون تکمیل اطلاعات فعال شود.
    /// </remarks>
    public void Activate()
    {
        if (Status != FacilityStatus.Draft)
            throw new InvalidOperationException("Can only activate draft facilities");

        var previousStatus = Status;
        Status = FacilityStatus.Active;

        // Raise domain event
        AddDomainEvent(new FacilityStatusChangedEvent(
            Id,
            Name,
            Code,
            previousStatus,
            Status,
            "Facility activated"));
    }

    /// <summary>
    /// تعلیق موقت تسهیلات
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار تسهیلات را به حالت تعلیق تغییر می‌دهد و دریافت درخواست‌های جدید
    /// را متوقف می‌نماید اما درخواست‌های موجود را حفظ می‌کند.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که نیاز به توقف موقت ارائه تسهیلات وجود دارد، این رفتار
    /// برای تعلیق تسهیلات استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - فقط تسهیلات فعال قابل تعلیق هستند.
    /// - وضعیت به Suspended تغییر می‌یابد.
    /// - دلیل تعلیق ثبت می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: فقط از حالت Active قابل تغییر باشد.
    /// - باید: دلیل تعلیق ثبت شود.
    /// - نباید: تسهیلات غیر فعال تعلیق شوند.
    /// - نباید: بدون دلیل تعلیق انجام شود.
    /// </remarks>
    public void Suspend(string? reason = null)
    {
        if (Status != FacilityStatus.Active)
            throw new InvalidOperationException("Can only suspend active facilities");

        var previousStatus = Status;
        Status = FacilityStatus.Suspended;

        if (!string.IsNullOrWhiteSpace(reason))
        {
            Metadata["SuspensionReason"] = reason;
        }

        // Raise domain event
        AddDomainEvent(new FacilityStatusChangedEvent(
            Id,
            Name,
            Code,
            previousStatus,
            Status,
            reason ?? "Facility suspended"));
    }

    /// <summary>
    /// بستن دائمی تسهیلات
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار تسهیلات را به حالت بسته تغییر می‌دهد و تمام فعالیت‌های مرتبط
    /// با آن را متوقف می‌نماید.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که تسهیلات دیگر ارائه نمی‌شود یا نیاز به حذف کامل آن وجود دارد،
    /// این رفتار برای بستن تسهیلات استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - تسهیلات فعال یا تعلیق شده قابل بستن هستند.
    /// - وضعیت به Closed تغییر می‌یابد.
    /// - دلیل بستن ثبت می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: دلیل بستن ثبت شود.
    /// - باید: وضعیت به Closed تغییر کند.
    /// - نباید: بدون دلیل بسته شود.
    /// - نباید: تسهیلات Draft بسته شوند.
    /// </remarks>
    public void Close(string? reason = null)
    {
        if (Status == FacilityStatus.Draft)
            throw new InvalidOperationException("Cannot close draft facilities");

        var previousStatus = Status;
        Status = FacilityStatus.Closed;

        if (!string.IsNullOrWhiteSpace(reason))
        {
            Metadata["ClosureReason"] = reason;
        }

        // Raise domain event
        AddDomainEvent(new FacilityStatusChangedEvent(
            Id,
            Name,
            Code,
            previousStatus,
            Status,
            reason ?? "Facility closed"));
    }

    /// <summary>
    /// اضافه کردن ویژگی به تسهیلات
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار یک ویژگی را به تسهیلات اضافه می‌کند که می‌تواند الزام، ممنوعیت
    /// یا تعدیل شرایط باشد.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که نیاز به تعریف قوانین خاص بر اساس ویژگی‌های عضو وجود دارد،
    /// این رفتار برای اضافه کردن ویژگی استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - شناسه ویژگی اجباری است.
    /// - نوع الزام باید مشخص باشد.
    /// - ویژگی تکراری اضافه نمی‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: شناسه ویژگی معتبر باشد.
    /// - باید: نوع الزام مشخص باشد.
    /// - نباید: ویژگی تکراری اضافه شود.
    /// - نباید: بدون شناسه ویژگی اضافه شود.
    /// </remarks>
    public void AddFeature(
        string featureId,
        FacilityFeatureRequirementType requirementType,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(featureId))
            throw new ArgumentException("Feature ID cannot be empty", nameof(featureId));

        if (_features.Any(f => f.FeatureId == featureId))
            throw new InvalidOperationException("Feature already exists for this facility");

        var facilityFeature = new FacilityFeature(Id, featureId, requirementType, notes);
        _features.Add(facilityFeature);

        // Raise domain event
        AddDomainEvent(new FacilityFeatureAddedEvent(
            Id,
            Name,
            Code,
            featureId,
            requirementType,
            notes));
    }

    /// <summary>
    /// حذف ویژگی از تسهیلات
    /// </summary>
    public void RemoveFeature(string featureId)
    {
        if (string.IsNullOrWhiteSpace(featureId))
            throw new ArgumentException("Feature ID cannot be empty", nameof(featureId));

        var feature = _features.FirstOrDefault(f => f.FeatureId == featureId);
        if (feature == null)
            throw new InvalidOperationException("Feature not found for this facility");

        _features.Remove(feature);

        // Raise domain event
        AddDomainEvent(new FacilityFeatureRemovedEvent(
            Id,
            Name,
            Code,
            featureId));
    }

    /// <summary>
    /// اضافه کردن سیاست قابلیت به تسهیلات
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار یک سیاست قابلیت را به تسهیلات اضافه می‌کند که می‌تواند الزام، ممنوعیت
    /// یا تعدیل مبلغ، سهمیه یا اولویت باشد.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که نیاز به تعریف قوانین خاص بر اساس قابلیت‌های عضو وجود دارد،
    /// این رفتار برای اضافه کردن سیاست قابلیت استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - شناسه قابلیت اجباری است.
    /// - نوع سیاست باید مشخص باشد.
    /// - سیاست تکراری اضافه نمی‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: شناسه قابلیت معتبر باشد.
    /// - باید: نوع سیاست مشخص باشد.
    /// - نباید: سیاست تکراری اضافه شود.
    /// - نباید: بدون شناسه قابلیت اضافه شود.
    /// </remarks>
    public void AddCapabilityPolicy(
        string capabilityId,
        CapabilityPolicyType policyType,
        decimal? modifierValue = null,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(capabilityId))
            throw new ArgumentException("Capability ID cannot be empty", nameof(capabilityId));

        if (_capabilityPolicies.Any(cp => cp.CapabilityId == capabilityId))
            throw new InvalidOperationException("Capability policy already exists for this facility");

        var capabilityPolicy = new FacilityCapabilityPolicy(Id, capabilityId, policyType, modifierValue, notes);
        _capabilityPolicies.Add(capabilityPolicy);

        // Raise domain event
        AddDomainEvent(new FacilityCapabilityPolicyAddedEvent(
            Id,
            Name,
            Code,
            capabilityId,
            policyType,
            modifierValue,
            notes));
    }

    /// <summary>
    /// حذف سیاست قابلیت از تسهیلات
    /// </summary>
    public void RemoveCapabilityPolicy(string capabilityId)
    {
        if (string.IsNullOrWhiteSpace(capabilityId))
            throw new ArgumentException("Capability ID cannot be empty", nameof(capabilityId));

        var policy = _capabilityPolicies.FirstOrDefault(cp => cp.CapabilityId == capabilityId);
        if (policy == null)
            throw new InvalidOperationException("Capability policy not found for this facility");

        _capabilityPolicies.Remove(policy);

        // Raise domain event
        AddDomainEvent(new FacilityCapabilityPolicyRemovedEvent(
            Id,
            Name,
            Code,
            capabilityId));
    }

    /// <summary>
    /// ایجاد دوره جدید برای تسهیلات
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار یک دوره جدید برای تسهیلات ایجاد می‌کند که شامل بازه زمانی،
    /// سهمیه، استراتژی پذیرش و سایر تنظیمات دوره است.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که نیاز به باز کردن دوره جدید برای دریافت درخواست‌ها وجود دارد،
    /// این رفتار برای ایجاد دوره استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - تسهیلات باید فعال باشد.
    /// - تاریخ شروع و پایان باید معتبر باشد.
    /// - سهمیه باید مثبت باشد.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: تسهیلات فعال باشد.
    /// - باید: تاریخ‌ها معتبر باشند.
    /// - نباید: دوره با تاریخ نامعتبر ایجاد شود.
    /// - نباید: سهمیه منفی تعیین شود.
    /// </remarks>
    public FacilityCycle CreateCycle(
        string name,
        DateTime startDate,
        DateTime endDate,
        int quota,
        string? description = null,
        Dictionary<string, string>? cycleMetadata = null)
    {
        if (Status != FacilityStatus.Active)
            throw new InvalidOperationException("Can only create cycles for active facilities");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Cycle name cannot be empty", nameof(name));

        if (startDate >= endDate)
            throw new ArgumentException("Start date must be before end date");

        if (quota <= 0)
            throw new ArgumentException("Quota must be positive", nameof(quota));

        var cycle = new FacilityCycle(
            Id,
            name,
            startDate,
            endDate,
            quota,
            description: description,
            metadata: cycleMetadata);

        _cycles.Add(cycle);

        // Raise domain event
        AddDomainEvent(new FacilityCycleCreatedEvent(
            cycle.Id,
            Id,
            Name,
            Code,
            name,
            startDate,
            endDate,
            quota));

        return cycle;
    }

    /// <summary>
    /// بررسی اینکه آیا تسهیلات فعال است
    /// </summary>
    public bool IsActive()
    {
        return Status == FacilityStatus.Active;
    }

    /// <summary>
    /// دریافت دوره فعال
    /// </summary>
    public FacilityCycle? GetActiveCycle()
    {
        var now = DateTime.UtcNow;
        return _cycles.FirstOrDefault(c => 
            c.Status == FacilityCycleStatus.Active && 
            c.StartDate <= now && 
            c.EndDate >= now);
    }

    /// <summary>
    /// دریافت تمام دوره‌های فعال
    /// </summary>
    public IEnumerable<FacilityCycle> GetActiveCycles()
    {
        var now = DateTime.UtcNow;
        return _cycles.Where(c => 
            c.Status == FacilityCycleStatus.Active && 
            c.StartDate <= now && 
            c.EndDate >= now);
    }
}
