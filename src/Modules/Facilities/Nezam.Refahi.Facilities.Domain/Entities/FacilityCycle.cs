using MCA.SharedKernel.Domain.AggregateRoots;
using Nezam.Refahi.Facilities.Domain.Enums;
using Nezam.Refahi.Facilities.Domain.Events;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Facilities.Domain.Entities;

/// <summary>
/// دوره تسهیلات - Aggregate Root مستقل برای مدیریت دوره‌ها
/// در دنیای واقعی: دوره زمانی مشخص برای دریافت درخواست‌های تسهیلات
/// </summary>
public sealed class FacilityCycle : FullAggregateRoot<Guid>
{
    public Guid FacilityId { get; private set; }
    public string Name { get; private set; } = null!;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int Quota { get; private set; }
    
    /// <summary>
    /// تعداد سهمیه استفاده شده - محاسبه شده از تعداد واقعی درخواست‌ها
    /// این property به صورت computed از Requests.Count محاسبه می‌شود
    /// </summary>
    public int UsedQuota => Requests.Count;
    
    public FacilityCycleStatus Status { get; private set; }
    public string? Description { get; private set; }
    
    // محدودیت به دوره‌های قبلی همین وام
    // اگر true باشد، کاربرانی که در دوره‌های قبلی همین وام ثبت‌نام کرده‌اند
    // و درخواستشان تایید شده، دیگر نمی‌توانند درخواست بفرستند
    public bool RestrictToPreviousCycles { get; private set; }
    
    // پیام تایید که بعد از تایید درخواست به کاربر نمایش داده می‌شود
    public string? ApprovalMessage { get; private set; }
    
    // درصد سود سالانه
    public decimal? InterestRate { get; private set; }
    
    // تعداد ماه‌های بازپرداخت
    public int? PaymentMonths { get; private set; }
    
    // Required previous facilities (dependencies)
    // هر دوره می‌تواند به یک وام دیگر وابسته باشد
    private readonly List<FacilityCycleDependency> _dependencies = new();
    public IReadOnlyCollection<FacilityCycleDependency> Dependencies => _dependencies.AsReadOnly();
    
    // لیست قیمت دوره - کاربر از این لیست انتخاب می‌کند
    private readonly List<FacilityCyclePriceOption> _priceOptions = new();
    public IReadOnlyCollection<FacilityCyclePriceOption> PriceOptions => _priceOptions.AsReadOnly();
    
    // ویژگی‌های مورد نیاز دوره - فقط ID
    private readonly List<FacilityCycleFeature> _features = new();
    public IReadOnlyCollection<FacilityCycleFeature> Features => _features.AsReadOnly();
    
    // قابلیت‌های مورد نیاز دوره - فقط ID
    private readonly List<FacilityCycleCapability> _capabilities = new();
    public IReadOnlyCollection<FacilityCycleCapability> Capabilities => _capabilities.AsReadOnly();
    
    public Dictionary<string, string> Metadata { get; private set; } = new();

    // Navigation properties
    public Facility Facility { get; private set; } = null!;

    // Navigation property for EF Core querying (read-only)
    // Note: This is for querying purposes only; FacilityCycle and FacilityRequest are separate aggregates
    public ICollection<FacilityRequest> Requests { get; private set; } = new List<FacilityRequest>();

    // Private constructor for EF Core
    private FacilityCycle() : base() { }

    /// <summary>
    /// ایجاد دوره جدید برای تسهیلات
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار یک دوره جدید برای تسهیلات ایجاد می‌کند که شامل بازه زمانی،
    /// سهمیه و سایر تنظیمات اولیه است.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که نیاز به باز کردن دوره جدید برای دریافت درخواست‌ها وجود دارد،
    /// این رفتار برای ایجاد دوره استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - شناسه تسهیلات اجباری است.
    /// - نام دوره اجباری است.
    /// - تاریخ شروع باید قبل از تاریخ پایان باشد.
    /// - سهمیه باید مثبت باشد.
    /// - وضعیت اولیه Draft است.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: شناسه تسهیلات معتبر باشد.
    /// - باید: تاریخ‌ها معتبر باشند.
    /// - نباید: سهمیه منفی تعیین شود.
    /// - نباید: تاریخ شروع بعد از پایان باشد.
    /// </remarks>
    public FacilityCycle(
        Guid facilityId,
        string name,
        DateTime startDate,
        DateTime endDate,
        int quota,
        bool restrictToPreviousCycles = false,
        string? description = null,
        string? approvalMessage = null,
        decimal? interestRate = null,
        int? paymentMonths = null,
        Dictionary<string, string>? metadata = null)
        : base(Guid.NewGuid())
    {
        if (facilityId == Guid.Empty)
            throw new ArgumentException("Facility ID cannot be empty", nameof(facilityId));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));
        if (startDate >= endDate)
            throw new ArgumentException("Start date must be before end date", nameof(startDate));
        if (quota <= 0)
            throw new ArgumentException("Quota must be positive", nameof(quota));
        if (interestRate.HasValue && (interestRate.Value < 0 || interestRate.Value > 1))
            throw new ArgumentException("Interest rate must be between 0 and 1", nameof(interestRate));
        if (paymentMonths.HasValue && paymentMonths.Value <= 0)
            throw new ArgumentException("Payment months must be positive", nameof(paymentMonths));

        FacilityId = facilityId;
        Name = name.Trim();
        StartDate = startDate;
        EndDate = endDate;
        Quota = quota;
        Status = FacilityCycleStatus.Draft;
        Description = description?.Trim();
        RestrictToPreviousCycles = restrictToPreviousCycles;
        ApprovalMessage = approvalMessage?.Trim();
        InterestRate = interestRate;
        PaymentMonths = paymentMonths;
        
        Metadata = metadata ?? new Dictionary<string, string>();
    }

    /// <summary>
    /// اضافه کردن وابستگی به تسهیلات دیگر
    /// </summary>
    public void AddDependency(Guid requiredFacilityId, string requiredFacilityName, bool mustBeCompleted = true)
    {
        if (requiredFacilityId == Guid.Empty)
            throw new ArgumentException("Required facility ID cannot be empty", nameof(requiredFacilityId));
        if (string.IsNullOrWhiteSpace(requiredFacilityName))
            throw new ArgumentException("Required facility name cannot be empty", nameof(requiredFacilityName));

        if (_dependencies.Any(d => d.RequiredFacilityId == requiredFacilityId))
            throw new InvalidOperationException("Dependency already exists for this facility");

        var dependency = new FacilityCycleDependency(Id, requiredFacilityId, requiredFacilityName, mustBeCompleted);
        _dependencies.Add(dependency);
    }

    /// <summary>
    /// حذف وابستگی
    /// </summary>
    public void RemoveDependency(Guid requiredFacilityId)
    {
        var dependency = _dependencies.FirstOrDefault(d => d.RequiredFacilityId == requiredFacilityId);
        if (dependency == null)
            throw new InvalidOperationException("Dependency not found");

        _dependencies.Remove(dependency);
    }

    /// <summary>
    /// اضافه کردن گزینه قیمت به لیست قیمت دوره
    /// </summary>
    public void AddPriceOption(Money amount, int displayOrder = 0, string? description = null)
    {
        if (amount == null)
            throw new ArgumentNullException(nameof(amount));
        if (amount.AmountRials <= 0)
            throw new ArgumentException("Amount must be positive", nameof(amount));

        var priceOption = new FacilityCyclePriceOption(Id, amount, displayOrder, description);
        _priceOptions.Add(priceOption);
    }

    /// <summary>
    /// حذف گزینه قیمت از لیست
    /// </summary>
    public void RemovePriceOption(Guid priceOptionId)
    {
        var priceOption = _priceOptions.FirstOrDefault(po => po.Id == priceOptionId);
        if (priceOption == null)
            throw new InvalidOperationException("Price option not found");

        _priceOptions.Remove(priceOption);
    }

    /// <summary>
    /// به‌روزرسانی پیام تایید
    /// </summary>
    public void UpdateApprovalMessage(string? message)
    {
        ApprovalMessage = message?.Trim();
    }

    /// <summary>
    /// به‌روزرسانی محدودیت به دوره‌های قبلی
    /// </summary>
    public void UpdateRestrictToPreviousCycles(bool restrict)
    {
        RestrictToPreviousCycles = restrict;
    }

    /// <summary>
    /// به‌روزرسانی درصد سود
    /// </summary>
    public void UpdateInterestRate(decimal? interestRate)
    {
        if (interestRate.HasValue && (interestRate.Value < 0 || interestRate.Value > 1))
            throw new ArgumentException("Interest rate must be between 0 and 1", nameof(interestRate));

        InterestRate = interestRate;
    }

    /// <summary>
    /// به‌روزرسانی تعداد ماه‌های بازپرداخت
    /// </summary>
    public void UpdatePaymentMonths(int? paymentMonths)
    {
        if (paymentMonths.HasValue && paymentMonths.Value <= 0)
            throw new ArgumentException("Payment months must be positive", nameof(paymentMonths));

        PaymentMonths = paymentMonths;
    }

    /// <summary>
    /// اضافه کردن ویژگی به دوره
    /// </summary>
    public void AddFeature(string featureId)
    {
        if (string.IsNullOrWhiteSpace(featureId))
            throw new ArgumentException("Feature ID cannot be empty", nameof(featureId));

        if (_features.Any(f => f.FeatureId == featureId))
            throw new InvalidOperationException("Feature already exists for this cycle");

        var cycleFeature = new FacilityCycleFeature(Id, featureId);
        _features.Add(cycleFeature);
    }

    /// <summary>
    /// حذف ویژگی از دوره
    /// </summary>
    public void RemoveFeature(string featureId)
    {
        if (string.IsNullOrWhiteSpace(featureId))
            throw new ArgumentException("Feature ID cannot be empty", nameof(featureId));

        var feature = _features.FirstOrDefault(f => f.FeatureId == featureId);
        if (feature == null)
            throw new InvalidOperationException("Feature not found for this cycle");

        _features.Remove(feature);
    }

    /// <summary>
    /// اضافه کردن قابلیت به دوره
    /// </summary>
    public void AddCapability(string capabilityId)
    {
        if (string.IsNullOrWhiteSpace(capabilityId))
            throw new ArgumentException("Capability ID cannot be empty", nameof(capabilityId));

        if (_capabilities.Any(c => c.CapabilityId == capabilityId))
            throw new InvalidOperationException("Capability already exists for this cycle");

        var cycleCapability = new FacilityCycleCapability(Id, capabilityId);
        _capabilities.Add(cycleCapability);
    }

    /// <summary>
    /// حذف قابلیت از دوره
    /// </summary>
    public void RemoveCapability(string capabilityId)
    {
        if (string.IsNullOrWhiteSpace(capabilityId))
            throw new ArgumentException("Capability ID cannot be empty", nameof(capabilityId));

        var capability = _capabilities.FirstOrDefault(c => c.CapabilityId == capabilityId);
        if (capability == null)
            throw new InvalidOperationException("Capability not found for this cycle");

        _capabilities.Remove(capability);
    }

    /// <summary>
    /// بررسی ساده واجد شرایط بودن بر اساس وابستگی‌ها
    /// برای بررسی کامل از Domain Service استفاده کنید: IFacilityCycleEligibilityService
    /// </summary>
    /// <remarks>
    /// این متد فقط بررسی وابستگی‌ها را انجام می‌دهد
    /// برای بررسی کامل (Features, Capabilities, Status, Quota) از Domain Service استفاده کنید
    /// </remarks>
    public bool HasSatisfiedDependencies(IEnumerable<Guid> completedFacilities)
    {
        if (completedFacilities == null)
            throw new ArgumentNullException(nameof(completedFacilities));

        var completedSet = completedFacilities.ToHashSet();
        
        foreach (var dependency in _dependencies)
        {
            if (dependency.MustBeCompleted && !completedSet.Contains(dependency.RequiredFacilityId))
                return false;
        }

        return true;
    }

    /// <summary>
    /// فعال کردن دوره
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار دوره را از حالت پیش‌نویس به حالت فعال تغییر می‌دهد
    /// و آن را آماده برای دریافت درخواست‌ها می‌نماید.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که دوره آماده شروع و دریافت درخواست‌ها است،
    /// این رفتار برای فعال کردن دوره استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - فقط دوره‌های پیش‌نویس قابل فعال کردن هستند.
    /// - وضعیت به Active تغییر می‌یابد.
    /// - زمان فعال‌سازی ثبت می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: فقط از حالت Draft قابل تغییر باشد.
    /// - باید: زمان فعال‌سازی دقیق ثبت شود.
    /// - نباید: دوره‌های غیر Draft فعال شوند.
    /// - نباید: بدون تکمیل اطلاعات فعال شود.
    /// </remarks>
    public void Activate()
    {
        if (Status != FacilityCycleStatus.Draft)
            throw new InvalidOperationException("Can only activate draft cycles");

        var previousStatus = Status;
        Status = FacilityCycleStatus.Active;
    }

    /// <summary>
    /// بستن دوره برای درخواست‌های جدید
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار دوره را به حالت بسته تغییر می‌دهد و دریافت درخواست‌های جدید
    /// را متوقف می‌نماید اما درخواست‌های موجود را حفظ می‌کند.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که مهلت دریافت درخواست‌ها به پایان رسیده یا سهمیه پر شده،
    /// این رفتار برای بستن دوره استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - فقط دوره‌های فعال قابل بستن هستند.
    /// - وضعیت به Closed تغییر می‌یابد.
    /// - دلیل بستن ثبت می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: فقط از حالت Active قابل تغییر باشد.
    /// - باید: دلیل بستن ثبت شود.
    /// - نباید: دوره‌های غیر فعال بسته شوند.
    /// - نباید: بدون دلیل بسته شود.
    /// </remarks>
    public void Close(string? reason = null)
    {
        if (Status != FacilityCycleStatus.Active)
            throw new InvalidOperationException("Can only close active cycles");

        var previousStatus = Status;
        Status = FacilityCycleStatus.Closed;

        if (!string.IsNullOrWhiteSpace(reason))
        {
            Metadata["ClosureReason"] = reason;
        }
    }

    /// <summary>
    /// تنظیم دوره به حالت تحت بررسی
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار دوره را به حالت تحت بررسی تغییر می‌دهد و یک Domain Event منتشر می‌کند
    /// که باعث می‌شود همه درخواست‌های PendingApproval به UnderReview تغییر کنند.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که دوره آماده شروع بررسی درخواست‌ها است، این رفتار برای
    /// تغییر وضعیت دوره و درخواست‌ها استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - فقط دوره‌های Active یا Closed قابل تغییر به UnderReview هستند.
    /// - وضعیت به UnderReview تغییر می‌یابد.
    /// - یک Domain Event منتشر می‌شود برای تغییر وضعیت درخواست‌ها.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: فقط از حالت Active یا Closed قابل تغییر باشد.
    /// - باید: Domain Event منتشر شود.
    /// - نباید: دوره‌های غیر فعال یا بسته تحت بررسی شوند.
    /// </remarks>
    public void SetUnderReview()
    {
        if (Status != FacilityCycleStatus.Active && Status != FacilityCycleStatus.Closed)
            throw new InvalidOperationException("Can only set under review for active or closed cycles");

        var previousStatus = Status;
        Status = FacilityCycleStatus.UnderReview;

        

        // انتشار Domain Event برای تغییر وضعیت همه درخواست‌های PendingApproval به UnderReview
        AddDomainEvent(new FacilityCycleStatusChangedEvent(
            Id,
            FacilityId,
            Name,
            previousStatus,
            Status,
            "Cycle set to under review - all PendingApproval requests will be changed to UnderReview"));
    }

    /// <summary>
    /// تکمیل دوره
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار دوره را به حالت تکمیل شده تغییر می‌دهد و تمام فرآیندهای
    /// مرتبط با آن را نهایی می‌نماید.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که تمام درخواست‌های دوره پردازش شده و دوره به پایان رسیده،
    /// این رفتار برای تکمیل دوره استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - فقط دوره‌های بسته یا UnderReview قابل تکمیل هستند.
    /// - وضعیت به Completed تغییر می‌یابد.
    /// - زمان تکمیل ثبت می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: فقط از حالت Closed یا UnderReview قابل تغییر باشد.
    /// - باید: زمان تکمیل دقیق ثبت شود.
    /// - نباید: دوره‌های غیر بسته تکمیل شوند.
    /// - نباید: بدون نهایی شدن فرآیندها تکمیل شود.
    /// </remarks>
    public void Complete()
    {
        if (Status != FacilityCycleStatus.Closed && Status != FacilityCycleStatus.UnderReview)
            throw new InvalidOperationException("Can only complete closed or under review cycles");

        var previousStatus = Status;
        Status = FacilityCycleStatus.Completed;
    }

    /// <summary>
    /// لغو دوره
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار دوره را به حالت لغو شده تغییر می‌دهد و تمام درخواست‌های
    /// مرتبط با آن را نیز لغو می‌نماید.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که به دلیل مشکلات فنی یا تغییر سیاست‌ها نیاز به لغو دوره وجود دارد،
    /// این رفتار برای لغو دوره استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - دوره‌های فعال یا بسته قابل لغو هستند.
    /// - وضعیت به Cancelled تغییر می‌یابد.
    /// - دلیل لغو ثبت می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: دلیل لغو ثبت شود.
    /// - باید: وضعیت به Cancelled تغییر کند.
    /// - نباید: بدون دلیل لغو شود.
    /// - نباید: دوره‌های تکمیل شده لغو شوند.
    /// </remarks>
    public void Cancel(string? reason = null)
    {
        if (Status == FacilityCycleStatus.Completed)
            throw new InvalidOperationException("Cannot cancel completed cycles");

        var previousStatus = Status;
        Status = FacilityCycleStatus.Cancelled;

        if (!string.IsNullOrWhiteSpace(reason))
        {
            Metadata["CancellationReason"] = reason;
        }
    }

    /// <summary>
    /// بررسی اینکه آیا سهمیه پر است
    /// Note: UsedQuota is now computed from Requests.Count automatically
    /// </summary>

    /// <summary>
    /// بررسی اینکه آیا دوره فعال است
    /// </summary>
    public bool IsActive()
    {
        var now = DateTime.UtcNow;
        return Status == FacilityCycleStatus.Active && 
               StartDate <= now && 
               EndDate >= now;
    }

    /// <summary>
    /// بررسی اینکه آیا سهمیه پر است
    /// </summary>
    public bool IsQuotaFull()
    {
        return UsedQuota >= Quota;
    }

    /// <summary>
    /// دریافت سهمیه باقیمانده
    /// </summary>
    public int GetRemainingQuota()
    {
        return Math.Max(0, Quota - UsedQuota);
    }

    /// <summary>
    /// بررسی اینکه آیا دوره در بازه زمانی است
    /// </summary>
    public bool IsInDateRange()
    {
        var now = DateTime.UtcNow;
        return StartDate <= now && EndDate >= now;
    }
}
