namespace Nezam.Refahi.Finance.Domain.Enums;

/// <summary>
/// نوع تخفیف - درصدی یا مقداری
/// </summary>
public enum DiscountType
{
    /// <summary>
    /// تخفیف درصدی - بر اساس درصد از مبلغ کل
    /// </summary>
    Percentage = 1,

    /// <summary>
    /// تخفیف مقداری - مبلغ ثابت از مبلغ کل
    /// </summary>
    FixedAmount = 2
}
