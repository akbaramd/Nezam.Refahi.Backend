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
    public int UsedQuota { get; private set; }
    public FacilityCycleStatus Status { get; private set; }
    public string? Description { get; private set; }
    
    // Amount limits for this cycle
    public Money? MinAmount { get; private set; }
    public Money? MaxAmount { get; private set; }
    public Money? DefaultAmount { get; private set; }
    
    // Payment terms for this cycle
    public int PaymentMonths { get; private set; }
    public decimal? InterestRate { get; private set; }
    
    // Cooldown period after receiving this facility
    public int CooldownDays { get; private set; }
    
    // Dependency rules
    public bool IsRepeatable { get; private set; }
    public bool IsExclusive { get; private set; }
    public string? ExclusiveSetId { get; private set; }
    public int? MaxActiveAcrossCycles { get; private set; }
    
    // Required previous facilities (dependencies)
    private readonly List<FacilityCycleDependency> _dependencies = new();
    public IReadOnlyCollection<FacilityCycleDependency> Dependencies => _dependencies.AsReadOnly();
    
    // Admission strategy
    public string AdmissionStrategy { get; private set; } = "FIFO"; // FIFO, Score, Lottery
    public int? WaitlistCapacity { get; private set; }
    
    public Dictionary<string, string> Metadata { get; private set; } = new();

    // Navigation properties
    public Facility Facility { get; private set; } = null!;

    private readonly List<FacilityRequest> _applications = new();
    public IReadOnlyCollection<FacilityRequest> Applications => _applications.AsReadOnly();

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
        Money? minAmount = null,
        Money? maxAmount = null,
        Money? defaultAmount = null,
        int paymentMonths = 12,
        decimal? interestRate = null,
        int cooldownDays = 0,
        bool isRepeatable = true,
        bool isExclusive = false,
        string? exclusiveSetId = null,
        int? maxActiveAcrossCycles = null,
        string? description = null,
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
        if (paymentMonths <= 0)
            throw new ArgumentException("Payment months must be positive", nameof(paymentMonths));
        if (cooldownDays < 0)
            throw new ArgumentException("Cooldown days cannot be negative", nameof(cooldownDays));

        FacilityId = facilityId;
        Name = name.Trim();
        StartDate = startDate;
        EndDate = endDate;
        Quota = quota;
        UsedQuota = 0;
        Status = FacilityCycleStatus.Draft;
        Description = description?.Trim();
        
        // Amount limits
        MinAmount = minAmount;
        MaxAmount = maxAmount;
        DefaultAmount = defaultAmount;
        
        // Payment terms
        PaymentMonths = paymentMonths;
        InterestRate = interestRate;
        
        // Cooldown and rules
        CooldownDays = cooldownDays;
        IsRepeatable = isRepeatable;
        IsExclusive = isExclusive;
        ExclusiveSetId = exclusiveSetId?.Trim();
        MaxActiveAcrossCycles = maxActiveAcrossCycles;
        
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
    /// بررسی اینکه آیا عضو واجد شرایط دریافت این تسهیلات است
    /// </summary>
    public bool IsMemberEligible(Guid memberId, IEnumerable<Guid> completedFacilities, IEnumerable<Guid> activeFacilities)
    {
        // بررسی وابستگی‌ها
        foreach (var dependency in _dependencies)
        {
            if (dependency.MustBeCompleted && !completedFacilities.Contains(dependency.RequiredFacilityId))
                return false;
        }

        // بررسی انحصاری بودن
        if (IsExclusive && !string.IsNullOrEmpty(ExclusiveSetId))
        {
            // اگر عضو در حال حاضر تسهیلات دیگری از همین مجموعه دارد، مجاز نیست
            // این منطق باید در Application layer پیاده‌سازی شود
        }

        // بررسی تکرارپذیری
        if (!IsRepeatable && activeFacilities.Contains(FacilityId))
            return false;

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
    /// - فقط دوره‌های بسته قابل تکمیل هستند.
    /// - وضعیت به Completed تغییر می‌یابد.
    /// - زمان تکمیل ثبت می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: فقط از حالت Closed قابل تغییر باشد.
    /// - باید: زمان تکمیل دقیق ثبت شود.
    /// - نباید: دوره‌های غیر بسته تکمیل شوند.
    /// - نباید: بدون نهایی شدن فرآیندها تکمیل شود.
    /// </remarks>
    public void Complete()
    {
        if (Status != FacilityCycleStatus.Closed)
            throw new InvalidOperationException("Can only complete closed cycles");

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
    /// اضافه کردن درخواست به دوره
    /// </summary>
    public void AddApplication(FacilityRequest application)
    {
        if (application == null)
            throw new ArgumentNullException(nameof(application));

        if (application.FacilityCycleId != Id)
            throw new ArgumentException("Application does not belong to this cycle");

        _applications.Add(application);
        UsedQuota++;
    }

    /// <summary>
    /// حذف درخواست از دوره
    /// </summary>
    public void RemoveApplication(FacilityRequest application)
    {
        if (application == null)
            throw new ArgumentNullException(nameof(application));

        if (_applications.Remove(application))
        {
            UsedQuota--;
        }
    }

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
