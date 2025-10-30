namespace Nezam.Refahi.Finance.Domain.Enums;

/// <summary>
/// وضعیت کد تخفیف
/// </summary>
public enum DiscountCodeStatus
{
    /// <summary>
    /// فعال - قابل استفاده
    /// </summary>
    Active = 1,

    /// <summary>
    /// غیرفعال - موقتاً غیرقابل استفاده
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// منقضی شده - تاریخ انقضا گذشته
    /// </summary>
    Expired = 3,

    /// <summary>
    /// تمام شده - تعداد استفاده به حد مجاز رسیده
    /// </summary>
    Exhausted = 4,

    /// <summary>
    /// لغو شده - به صورت دستی لغو شده
    /// </summary>
    Cancelled = 5
}
