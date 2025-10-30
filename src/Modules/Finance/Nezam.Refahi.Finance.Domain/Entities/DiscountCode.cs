using MCA.SharedKernel.Domain.AggregateRoots;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Finance.Domain.Events;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Domain.Entities;

/// <summary>
/// کد تخفیف - نماینده کدهای تخفیف قابل استفاده در سیستم
/// در دنیای واقعی: کوپن‌ها، کدهای تخفیف، کدهای ویژه و سایر ابزارهای کاهش قیمت
/// </summary>
public sealed class DiscountCode : FullAggregateRoot<Guid>
{
    public string Code { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public DiscountType Type { get; private set; }
    public decimal DiscountValue { get; private set; }
    public Money? MaximumDiscountAmount { get; private set; }
    public Money? MinimumBillAmount { get; private set; }
    public DiscountCodeStatus Status { get; private set; }
    public DateTime ValidFrom { get; private set; }
    public DateTime ValidTo { get; private set; }
    public int? UsageLimit { get; private set; }
    public int UsedCount { get; private set; }
    public bool IsSingleUse { get; private set; }
    public bool IsActive { get; private set; }
    public Guid? CreatedByExternalUserId { get; private set; }
    public string? CreatedByUserFullName { get; private set; }
    public Dictionary<string, string> Metadata { get; private set; } = new();

    // Navigation properties
    private readonly List<DiscountCodeUsage> _usages = new();
    public IReadOnlyCollection<DiscountCodeUsage> Usages => _usages.AsReadOnly();

    // Private constructor for EF Core
    private DiscountCode() : base(Guid.NewGuid()) { } // EF Core uses this

    /// <summary>
    /// ایجاد کد تخفیف جدید
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار یک کد تخفیف جدید با تمام پارامترهای لازم ایجاد می‌کند که شامل
    /// نوع تخفیف، مقدار، محدودیت‌ها و قوانین استفاده است.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که کسب‌وکار نیاز به ایجاد کوپن یا کد تخفیف برای مشتریان دارد،
    /// این رفتار برای ایجاد کد تخفیف استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - کد تخفیف باید یکتا و غیرتکراری باشد.
    /// - مقدار تخفیف باید مثبت باشد.
    /// - تاریخ شروع باید قبل از تاریخ پایان باشد.
    /// - در صورت تعیین حد مجاز، باید مثبت باشد.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: کد یکتا و معتبر ارائه شود.
    /// - باید: مقدار تخفیف مثبت باشد.
    /// - نباید: کد تکراری ایجاد شود.
    /// - نباید: تاریخ‌های نامعتبر تعیین شود.
    /// </remarks>
    public DiscountCode(
        string code,
        string title,
        DiscountType type,
        decimal discountValue,
        DateTime validFrom,
        DateTime validTo,
        string? description = null,
        Money? maximumDiscountAmount = null,
        Money? minimumBillAmount = null,
        int? usageLimit = null,
        bool isSingleUse = false,
        Guid? createdByExternalUserId = null,
        string? createdByUserFullName = null,
        Dictionary<string, string>? metadata = null)
        : base(Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code cannot be empty", nameof(code));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
        if (discountValue <= 0)
            throw new ArgumentException("Discount value must be positive", nameof(discountValue));
        if (validFrom >= validTo)
            throw new ArgumentException("Valid from date must be before valid to date");
        if (usageLimit.HasValue && usageLimit.Value <= 0)
            throw new ArgumentException("Usage limit must be positive", nameof(usageLimit));

        // Validate discount value based on type
        if (type == DiscountType.Percentage && discountValue > 100)
            throw new ArgumentException("Percentage discount cannot exceed 100%", nameof(discountValue));

        Code = code.Trim().ToUpperInvariant();
        Title = title.Trim();
        Description = description?.Trim();
        Type = type;
        DiscountValue = discountValue;
        MaximumDiscountAmount = maximumDiscountAmount;
        MinimumBillAmount = minimumBillAmount;
        Status = DiscountCodeStatus.Active;
        ValidFrom = validFrom;
        ValidTo = validTo;
        UsageLimit = usageLimit;
        UsedCount = 0;
        IsSingleUse = isSingleUse;
        IsActive = true;
        CreatedByExternalUserId = createdByExternalUserId;
        CreatedByUserFullName = createdByUserFullName?.Trim();
        Metadata = metadata ?? new Dictionary<string, string>();

        // Raise domain event
        AddDomainEvent(new DiscountCodeCreatedEvent(
            Id,
            Code,
            Title,
            Type,
            DiscountValue,
            ValidFrom,
            ValidTo,
            UsageLimit,
            IsSingleUse,
            CreatedByExternalUserId,
            CreatedByUserFullName));
    }

    /// <summary>
    /// اعمال کد تخفیف بر روی مبلغ فاکتور
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار کد تخفیف را بر روی مبلغ فاکتور اعمال کرده و مبلغ تخفیف محاسبه شده
    /// را برمی‌گرداند. همچنین استفاده از کد را ثبت می‌کند.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که مشتری کد تخفیف را در فرآیند پرداخت وارد می‌کند،
    /// این رفتار برای محاسبه و اعمال تخفیف استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - کد تخفیف باید فعال و معتبر باشد.
    /// - تاریخ استفاده باید در بازه معتبر باشد.
    /// - تعداد استفاده نباید از حد مجاز تجاوز کند.
    /// - مبلغ فاکتور باید حداقل مبلغ مورد نیاز را داشته باشد.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: کد معتبر و قابل استفاده باشد.
    /// - باید: مبلغ فاکتور شرایط را برآورده کند.
    /// - نباید: کد منقضی یا تمام شده استفاده شود.
    /// - نباید: مبلغ فاکتور کمتر از حداقل باشد.
    /// </remarks>
    public (Money FinalDiscountAmount, DiscountCodeUsage Usage) ApplyDiscount(Money billAmount, Guid billId, Guid externalUserId, string? userFullName = null)
    {
        ValidateDiscountEligibility(billAmount);

        var discountAmount = CalculateDiscountAmount(billAmount);
        var finalDiscountAmount = ApplyMaximumDiscountLimit(discountAmount);

        // Record usage
        var usage = new DiscountCodeUsage(
            Id,
            billId,
            externalUserId,
            userFullName,
            billAmount,
            finalDiscountAmount);

        _usages.Add(usage);
        UsedCount++;

        // Update status if needed
        UpdateStatusAfterUsage();

        // Raise domain event
        AddDomainEvent(new DiscountCodeUsedEvent(
            Id,
            Code,
            billId,
            externalUserId,
            userFullName,
            billAmount,
            finalDiscountAmount,
            UsedCount));

        return (finalDiscountAmount, usage);
    }

    /// <summary>
    /// غیرفعال کردن کد تخفیف
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار کد تخفیف را غیرفعال می‌کند و آن را از استفاده بیشتر محروم می‌نماید.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که نیاز به توقف استفاده از کد تخفیف وجود دارد،
    /// این رفتار برای غیرفعال کردن کد استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - فقط کدهای فعال قابل غیرفعال کردن هستند.
    /// - وضعیت به غیرفعال تغییر می‌یابد.
    /// - دلیل غیرفعال‌سازی ثبت می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: فقط کدهای فعال غیرفعال شوند.
    /// - باید: دلیل غیرفعال‌سازی ثبت شود.
    /// - نباید: کدهای غیرفعال مجدداً غیرفعال شوند.
    /// - نباید: بدون دلیل غیرفعال شود.
    /// </remarks>
    public void Deactivate(string? reason = null)
    {
        if (Status != DiscountCodeStatus.Active)
            throw new InvalidOperationException("Can only deactivate active discount codes");

        var previousStatus = Status;
        Status = DiscountCodeStatus.Inactive;
        IsActive = false;

        if (!string.IsNullOrWhiteSpace(reason))
        {
            Metadata["DeactivationReason"] = reason;
        }

        // Raise domain event
        AddDomainEvent(new DiscountCodeStatusChangedEvent(
            Id,
            Code,
            previousStatus,
            Status,
            reason ?? "Discount code deactivated"));
    }

    /// <summary>
    /// فعال کردن مجدد کد تخفیف
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار کد تخفیف غیرفعال را مجدداً فعال می‌کند.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که نیاز به فعال کردن مجدد کد تخفیف وجود دارد،
    /// این رفتار برای فعال کردن کد استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - فقط کدهای غیرفعال قابل فعال کردن هستند.
    /// - کد نباید منقضی شده باشد.
    /// - وضعیت به فعال تغییر می‌یابد.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: فقط کدهای غیرفعال فعال شوند.
    /// - باید: کد منقضی نشده باشد.
    /// - نباید: کدهای فعال مجدداً فعال شوند.
    /// - نباید: کدهای منقضی فعال شوند.
    /// </remarks>
    public void Activate()
    {
        if (Status != DiscountCodeStatus.Inactive)
            throw new InvalidOperationException("Can only activate inactive discount codes");

        if (DateTime.UtcNow > ValidTo)
            throw new InvalidOperationException("Cannot activate expired discount code");

        var previousStatus = Status;
        Status = DiscountCodeStatus.Active;
        IsActive = true;

        // Raise domain event
        AddDomainEvent(new DiscountCodeStatusChangedEvent(
            Id,
            Code,
            previousStatus,
            Status,
            "Discount code activated"));
    }

    /// <summary>
    /// لغو کد تخفیف
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار کد تخفیف را لغو می‌کند و آن را به صورت دائمی غیرقابل استفاده می‌نماید.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که نیاز به لغو کامل کد تخفیف وجود دارد،
    /// این رفتار برای لغو کد استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - کدهای فعال یا غیرفعال قابل لغو هستند.
    /// - وضعیت به لغو شده تغییر می‌یابد.
    /// - دلیل لغو ثبت می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: دلیل لغو ثبت شود.
    /// - باید: وضعیت به لغو شده تغییر کند.
    /// - نباید: بدون دلیل لغو شود.
    /// - نباید: کدهای قبلاً لغو شده مجدداً لغو شوند.
    /// </remarks>
    public void Cancel(string? reason = null)
    {
        if (Status == DiscountCodeStatus.Cancelled)
            return; // Already cancelled

        var previousStatus = Status;
        Status = DiscountCodeStatus.Cancelled;
        IsActive = false;

        if (!string.IsNullOrWhiteSpace(reason))
        {
            Metadata["CancellationReason"] = reason;
        }

        // Raise domain event
        AddDomainEvent(new DiscountCodeStatusChangedEvent(
            Id,
            Code,
            previousStatus,
            Status,
            reason ?? "Discount code cancelled"));
    }

    /// <summary>
    /// بررسی معتبر بودن کد تخفیف
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار بررسی می‌کند که آیا کد تخفیف در زمان جاری معتبر و قابل استفاده است یا خیر.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که سیستم نیاز به بررسی اعتبار کد تخفیف دارد،
    /// این رفتار برای اعتبارسنجی استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - کد باید فعال باشد.
    /// - تاریخ جاری باید در بازه معتبر باشد.
    /// - تعداد استفاده نباید از حد مجاز تجاوز کند.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: تمام شرایط اعتبار بررسی شود.
    /// - باید: وضعیت کد در نظر گرفته شود.
    /// - نباید: کدهای منقضی معتبر تلقی شوند.
    /// - نباید: کدهای تمام شده معتبر تلقی شوند.
    /// </remarks>
    public bool IsValid()
    {
        if (!IsActive || Status != DiscountCodeStatus.Active)
            return false;

        var now = DateTime.UtcNow;
        if (now < ValidFrom || now > ValidTo)
            return false;

        if (UsageLimit.HasValue && UsedCount >= UsageLimit.Value)
            return false;

        return true;
    }

    /// <summary>
    /// بررسی منقضی بودن کد تخفیف
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار بررسی می‌کند که آیا تاریخ انقضای کد تخفیف گذشته است یا خیر.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که سیستم نیاز به بررسی انقضای کد تخفیف دارد،
    /// این رفتار برای بررسی انقضا استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - مقایسه با زمان جاری بر اساس UTC انجام می‌شود.
    /// - اگر تاریخ انقضا گذشته باشد، کد منقضی است.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: مقایسه زمان دقیق انجام شود.
    /// - باید: زمان UTC استفاده شود.
    /// - نباید: زمان محلی استفاده شود.
    /// - نباید: کدهای معتبر منقضی تلقی شوند.
    /// </remarks>
    public bool IsExpired()
    {
        return DateTime.UtcNow > ValidTo;
    }

    /// <summary>
    /// بررسی تمام شدن کد تخفیف
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار بررسی می‌کند که آیا تعداد استفاده از کد تخفیف به حد مجاز رسیده است یا خیر.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که سیستم نیاز به بررسی تمام شدن کد تخفیف دارد،
    /// این رفتار برای بررسی تمام شدن استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - اگر حد مجاز تعیین نشده باشد، کد تمام نمی‌شود.
    /// - اگر تعداد استفاده از حد مجاز بیشتر یا مساوی باشد، کد تمام شده است.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: مقایسه دقیق تعداد استفاده انجام شود.
    /// - باید: حد مجاز در نظر گرفته شود.
    /// - نباید: کدهای بدون حد مجاز تمام شده تلقی شوند.
    /// - نباید: کدهای با استفاده کمتر تمام شده تلقی شوند.
    /// </remarks>
    public bool IsExhausted()
    {
        return UsageLimit.HasValue && UsedCount >= UsageLimit.Value;
    }

    /// <summary>
    /// دریافت تعداد استفاده باقیمانده
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار تعداد استفاده باقیمانده از کد تخفیف را محاسبه و برمی‌گرداند.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که سیستم نیاز به نمایش تعداد استفاده باقیمانده دارد،
    /// این رفتار برای محاسبه باقیمانده استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - اگر حد مجاز تعیین نشده باشد، null برمی‌گرداند.
    /// - باقیمانده = حد مجاز - تعداد استفاده فعلی.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: محاسبه دقیق باقیمانده انجام شود.
    /// - باید: حد مجاز در نظر گرفته شود.
    /// - نباید: مقدار منفی برگردانده شود.
    /// - نباید: کدهای بدون حد مجاز مقدار برگردانند.
    /// </remarks>
    public int? GetRemainingUsageCount()
    {
        if (!UsageLimit.HasValue)
            return null;

        return Math.Max(0, UsageLimit.Value - UsedCount);
    }

    // Private methods
    private void ValidateDiscountEligibility(Money billAmount)
    {
        if (!IsValid())
            throw new InvalidOperationException("Discount code is not valid");

        if (MinimumBillAmount != null && billAmount.AmountRials < MinimumBillAmount.AmountRials)
            throw new InvalidOperationException($"Bill amount must be at least {MinimumBillAmount.AmountRials} Rials");

        if (IsSingleUse && UsedCount > 0)
            throw new InvalidOperationException("Single-use discount code has already been used");
    }

    private Money CalculateDiscountAmount(Money billAmount)
    {
        if (Type == DiscountType.Percentage)
        {
            var percentageAmount = billAmount.AmountRials * (DiscountValue / 100);
            return Money.FromRials(percentageAmount);
        }
        else // FixedAmount
        {
            return Money.FromRials(DiscountValue);
        }
    }

    private Money ApplyMaximumDiscountLimit(Money calculatedDiscount)
    {
        if (MaximumDiscountAmount != null && calculatedDiscount.AmountRials > MaximumDiscountAmount.AmountRials)
        {
            return MaximumDiscountAmount;
        }

        return calculatedDiscount;
    }

    private void UpdateStatusAfterUsage()
    {
        if (IsSingleUse && UsedCount >= 1)
        {
            Status = DiscountCodeStatus.Exhausted;
            IsActive = false;
        }
        else if (UsageLimit.HasValue && UsedCount >= UsageLimit.Value)
        {
            Status = DiscountCodeStatus.Exhausted;
            IsActive = false;
        }
        else if (IsExpired())
        {
            Status = DiscountCodeStatus.Expired;
            IsActive = false;
        }
    }
}
