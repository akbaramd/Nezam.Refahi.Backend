using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Domain.Services;

/// <summary>
/// Domain service for discount code validation and application
/// Single responsibility: validating discount codes and applying discounts to bills
/// Stateless service that operates on domain entities without repository dependencies
/// </summary>
public class DiscountCodeDomainService
{
    /// <summary>
    /// اعتبارسنجی کد تخفیف
    /// هدف: بررسی اینکه آیا کد تخفیف معتبر و قابل استفاده است
    /// نتیجه مورد انتظار: نتیجه اعتبارسنجی با جزئیات خطا
    /// منطق کسب‌وکار: کد تخفیف باید فعال، معتبر و قابل استفاده باشد
    /// </summary>
    public DiscountValidationResult ValidateDiscountCode(
        DiscountCode discountCode, 
        Money billAmount, 
        Guid externalUserId,
        bool hasUserUsedCode = false)
    {
        var errors = new List<string>();

        // بررسی وضعیت کد تخفیف
        if (!discountCode.IsValid())
        {
            if (discountCode.IsExpired())
                errors.Add("کد تخفیف منقضی شده است");
            else if (discountCode.IsExhausted())
                errors.Add("تعداد استفاده از کد تخفیف به حد مجاز رسیده است");
            else if (discountCode.Status != DiscountCodeStatus.Active)
                errors.Add("کد تخفیف غیرفعال است");
        }

        // بررسی حداقل مبلغ فاکتور
        if (discountCode.MinimumBillAmount?.AmountRials > 0 && 
            billAmount.AmountRials < discountCode.MinimumBillAmount?.AmountRials)
        {
            errors.Add($"مبلغ فاکتور باید حداقل {discountCode.MinimumBillAmount?.AmountRials:N0} ریال باشد");
        }

        // بررسی استفاده یکباره توسط کاربر
        if (discountCode.IsSingleUse && hasUserUsedCode)
        {
            errors.Add("این کد تخفیف قبلاً توسط شما استفاده شده است");
        }

        var isValid = errors.Count == 0;
        Money? discountAmount = null;

        if (isValid)
        {
            // محاسبه مبلغ تخفیف
            discountAmount = CalculateDiscountAmount(discountCode, billAmount);
        }

        return new DiscountValidationResult(
            isValid, 
            errors.FirstOrDefault(), 
            discountAmount, 
            discountCode);
    }

    /// <summary>
    /// محاسبه مبلغ تخفیف بدون اعمال
    /// هدف: محاسبه مبلغ تخفیف بدون ثبت استفاده
    /// نتیجه مورد انتظار: مبلغ تخفیف محاسبه شده
    /// منطق کسب‌وکار: فقط محاسبه تخفیف بدون تغییر وضعیت کد
    /// </summary>
    public Money CalculateDiscountAmount(DiscountCode discountCode, Money billAmount)
    {
        Money discountAmount;

        if (discountCode.Type == DiscountType.Percentage)
        {
            var percentageAmount = billAmount.AmountRials * (discountCode.DiscountValue / 100);
            discountAmount = Money.FromRials(percentageAmount);
        }
        else // FixedAmount
        {
            discountAmount = Money.FromRials(discountCode.DiscountValue);
        }

        // اعمال حداکثر مبلغ تخفیف
        if (discountCode.MaximumDiscountAmount?.AmountRials > 0 && 
            discountAmount.AmountRials > discountCode.MaximumDiscountAmount?.AmountRials)
        {
            discountAmount = discountCode.MaximumDiscountAmount;
        }

        // اطمینان از عدم تجاوز از مبلغ فاکتور
        if (discountAmount.AmountRials > billAmount.AmountRials)
        {
            discountAmount = billAmount;
        }

        return discountAmount;
    }

    /// <summary>
    /// بررسی انقضای کدهای تخفیف
    /// هدف: بررسی و به‌روزرسانی وضعیت کدهای منقضی شده
    /// نتیجه مورد انتظار: تعداد کدهای به‌روزرسانی شده
    /// منطق کسب‌وکار: کدهای منقضی شده به وضعیت منقضی تغییر می‌یابند
    /// </summary>
    public int ProcessExpiredDiscountCodes(IEnumerable<DiscountCode> discountCodes)
    {
        var updatedCount = 0;

        foreach (var code in discountCodes)
        {
            if (code.Status == DiscountCodeStatus.Active && code.IsExpired())
            {
                code.Deactivate("کد تخفیف منقضی شده است");
                updatedCount++;
            }
        }

        return updatedCount;
    }

    /// <summary>
    /// دریافت آمار استفاده از کد تخفیف
    /// هدف: تحلیل استفاده از کد تخفیف
    /// نتیجه مورد انتظار: آمار کامل استفاده
    /// منطق کسب‌وکار: تحلیل استفاده برای گزارش‌گیری و بهینه‌سازی
    /// </summary>
    public DiscountCodeStatistics CalculateDiscountCodeStatistics(
        DiscountCode discountCode, 
        IEnumerable<DiscountCodeUsage> usages)
    {
        var usageList = usages.ToList();

        var totalDiscountAmount = usageList.Sum(u => u.DiscountAmount.AmountRials);
        var totalBillAmount = usageList.Sum(u => u.BillAmount.AmountRials);
        var averageDiscountPercentage = usageList.Any() ? usageList.Average(u => u.GetDiscountPercentage()) : 0;

        var uniqueUsers = usageList.Select(u => u.ExternalUserId).Distinct().Count();

        return new DiscountCodeStatistics(
            discountCode.Id,
            discountCode.Code,
            discountCode.Status,
            usageList.Count,
            uniqueUsers,
            totalDiscountAmount,
            totalBillAmount,
            averageDiscountPercentage,
            discountCode.UsedCount,
            discountCode.UsageLimit,
            discountCode.GetRemainingUsageCount());
    }

    /// <summary>
    /// اعتبارسنجی کد تخفیف برای فاکتور
    /// هدف: بررسی اینکه آیا کد تخفیف قابل اعمال بر روی فاکتور است
    /// نتیجه مورد انتظار: نتیجه اعتبارسنجی با جزئیات خطا
    /// منطق کسب‌وکار: کد تخفیف باید با شرایط فاکتور سازگار باشد
    /// </summary>
    public DiscountValidationResult ValidateDiscountForBill(
        DiscountCode discountCode,
        Bill bill,
        bool hasUserUsedCode = false)
    {
        var errors = new List<string>();

        // بررسی وضعیت فاکتور
        if (bill.Status != BillStatus.Draft && bill.Status != BillStatus.Issued)
        {
            errors.Add("کد تخفیف فقط برای فاکتورهای پیش‌نویس یا صادر شده قابل اعمال است");
        }

        if (bill.PaidAmount.AmountRials > 0)
        {
            errors.Add("کد تخفیف برای فاکتورهای پرداخت شده قابل اعمال نیست");
        }

        if (bill.DiscountCodeId.HasValue)
        {
            errors.Add("کد تخفیف قبلاً بر روی این فاکتور اعمال شده است");
        }

        // اعتبارسنجی کد تخفیف
        var validationResult = ValidateDiscountCode(discountCode, bill.TotalAmount, bill.ExternalUserId, hasUserUsedCode);
        if (!validationResult.IsValid)
        {
            errors.AddRange(validationResult.ErrorMessage?.Split(';') ?? new[] { validationResult.ErrorMessage ?? "کد تخفیف معتبر نیست" });
        }

        var isValid = errors.Count == 0;
        Money? discountAmount = null;

        if (isValid)
        {
            discountAmount = CalculateDiscountAmount(discountCode, bill.TotalAmount);
        }

        return new DiscountValidationResult(
            isValid,
            errors.FirstOrDefault(),
            discountAmount,
            discountCode);
    }
}

/// <summary>
/// نتیجه اعتبارسنجی کد تخفیف
/// </summary>
public record DiscountValidationResult(
    bool IsValid, 
    string? ErrorMessage, 
    Money? DiscountAmount, 
    DiscountCode? DiscountCode);

/// <summary>
/// آمار استفاده از کد تخفیف
/// </summary>
public record DiscountCodeStatistics(
    Guid DiscountCodeId,
    string Code,
    DiscountCodeStatus Status,
    int TotalUsages,
    int UniqueUsers,
    decimal TotalDiscountAmount,
    decimal TotalBillAmount,
    decimal AverageDiscountPercentage,
    int UsedCount,
    int? UsageLimit,
    int? RemainingUsageCount);
