using MCA.SharedKernel.Domain;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Domain.Entities;

/// <summary>
/// سابقه استفاده از کد تخفیف - ثبت هر بار استفاده از کد تخفیف
/// در دنیای واقعی: تاریخچه استفاده از کوپن‌ها و کدهای تخفیف برای گزارش‌گیری و تحلیل
/// </summary>
public sealed class DiscountCodeUsage : Entity<Guid>
{
    public Guid DiscountCodeId { get; private set; }
    public Guid BillId { get; private set; }
    public Guid ExternalUserId { get; private set; }
    public string? UserFullName { get; private set; }
    public Money BillAmount { get; private set; } = null!;
    public Money DiscountAmount { get; private set; } = null!;
    public DateTime UsedAt { get; private set; }
    public string? Notes { get; private set; }
    public Dictionary<string, string> Metadata { get; private set; } = new();

    // Navigation properties
    public DiscountCode DiscountCode { get; private set; } = null!;

    // Private constructor for EF Core
    private DiscountCodeUsage() : base(Guid.NewGuid()) { } // EF Core uses this

    /// <summary>
    /// ایجاد سابقه استفاده از کد تخفیف
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار سابقه استفاده از کد تخفیف را ثبت می‌کند که شامل
    /// اطلاعات فاکتور، کاربر، مبالغ و زمان استفاده است.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که کاربر کد تخفیف را استفاده می‌کند،
    /// این رفتار برای ثبت سابقه استفاده استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - شناسه کد تخفیف و فاکتور اجباری است.
    /// - شناسه کاربر اجباری است.
    /// - مبالغ باید مثبت باشند.
    /// - زمان استفاده به صورت خودکار ثبت می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: شناسه‌های معتبر ارائه شود.
    /// - باید: مبالغ مثبت باشند.
    /// - نباید: بدون شناسه کد تخفیف ایجاد شود.
    /// - نباید: مبالغ منفی ثبت شود.
    /// </remarks>
    public DiscountCodeUsage(
        Guid discountCodeId,
        Guid billId,
        Guid externalUserId,
        string? userFullName,
        Money billAmount,
        Money discountAmount,
        string? notes = null,
        Dictionary<string, string>? metadata = null)
        : base(Guid.NewGuid())
    {
        if (discountCodeId == Guid.Empty)
            throw new ArgumentException("Discount code ID cannot be empty", nameof(discountCodeId));
        if (billId == Guid.Empty)
            throw new ArgumentException("Bill ID cannot be empty", nameof(billId));
        if (externalUserId == Guid.Empty)
            throw new ArgumentException("External user ID cannot be empty", nameof(externalUserId));
        if (billAmount.AmountRials < 0)
            throw new ArgumentException("Bill amount cannot be negative", nameof(billAmount));
        if (discountAmount.AmountRials < 0)
            throw new ArgumentException("Discount amount cannot be negative", nameof(discountAmount));

        DiscountCodeId = discountCodeId;
        BillId = billId;
        ExternalUserId = externalUserId;
        UserFullName = userFullName?.Trim();
        BillAmount = billAmount;
        DiscountAmount = discountAmount;
        UsedAt = DateTime.UtcNow;
        Notes = notes?.Trim();
        Metadata = metadata ?? new Dictionary<string, string>();
    }

    /// <summary>
    /// به‌روزرسانی یادداشت‌های استفاده
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار یادداشت‌های مرتبط با استفاده از کد تخفیف را به‌روزرسانی می‌کند.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که نیاز به اضافه کردن یا تغییر یادداشت‌های استفاده وجود دارد،
    /// این رفتار برای به‌روزرسانی یادداشت‌ها استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - یادداشت‌ها می‌توانند خالی باشند.
    /// - یادداشت‌ها باید trim شوند.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: یادداشت‌ها trim شوند.
    /// - نباید: یادداشت‌های خالی رد شوند.
    /// </remarks>
    public void UpdateNotes(string? notes)
    {
        Notes = notes?.Trim();
    }

    /// <summary>
    /// اضافه کردن متادیتا
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار متادیتای جدید را به سابقه استفاده اضافه می‌کند.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که نیاز به ذخیره اطلاعات اضافی مرتبط با استفاده وجود دارد،
    /// این رفتار برای اضافه کردن متادیتا استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - کلید و مقدار نباید خالی باشند.
    /// - کلیدهای تکراری جایگزین می‌شوند.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: کلید و مقدار معتبر باشند.
    /// - نباید: کلید یا مقدار خالی باشند.
    /// </remarks>
    public void AddMetadata(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be empty", nameof(key));
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be empty", nameof(value));

        Metadata[key.Trim()] = value.Trim();
    }

    /// <summary>
    /// حذف متادیتا
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار متادیتای مشخص شده را از سابقه استفاده حذف می‌کند.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که نیاز به حذف اطلاعات اضافی وجود دارد،
    /// این رفتار برای حذف متادیتا استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - کلید نباید خالی باشد.
    /// - اگر کلید وجود نداشته باشد، خطا رخ نمی‌دهد.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: کلید معتبر باشد.
    /// - نباید: کلید خالی باشد.
    /// </remarks>
    public void RemoveMetadata(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be empty", nameof(key));

        Metadata.Remove(key.Trim());
    }

    /// <summary>
    /// محاسبه درصد تخفیف اعمال شده
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار درصد تخفیف اعمال شده نسبت به مبلغ فاکتور را محاسبه می‌کند.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که نیاز به تحلیل میزان تخفیف اعمال شده وجود دارد،
    /// این رفتار برای محاسبه درصد تخفیف استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - مبلغ فاکتور نباید صفر باشد.
    /// - درصد تخفیف بین 0 تا 100 خواهد بود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: مبلغ فاکتور مثبت باشد.
    /// - باید: محاسبه دقیق انجام شود.
    /// - نباید: تقسیم بر صفر رخ دهد.
    /// - نباید: درصد منفی برگردانده شود.
    /// </remarks>
    public decimal GetDiscountPercentage()
    {
        if (BillAmount.AmountRials == 0)
            return 0;

        return (DiscountAmount.AmountRials / BillAmount.AmountRials) * 100;
    }

    /// <summary>
    /// محاسبه مبلغ نهایی پس از تخفیف
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار مبلغ نهایی فاکتور پس از اعمال تخفیف را محاسبه می‌کند.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که نیاز به نمایش مبلغ نهایی پس از تخفیف وجود دارد،
    /// این رفتار برای محاسبه مبلغ نهایی استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - مبلغ نهایی = مبلغ فاکتور - مبلغ تخفیف.
    /// - مبلغ نهایی نمی‌تواند منفی باشد.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: محاسبه دقیق انجام شود.
    /// - باید: مبلغ نهایی مثبت باشد.
    /// - نباید: مبلغ منفی برگردانده شود.
    /// </remarks>
    public Money GetFinalAmount()
    {
        var finalAmount = BillAmount.AmountRials - DiscountAmount.AmountRials;
        return Money.FromRials(Math.Max(0, finalAmount));
    }
}
