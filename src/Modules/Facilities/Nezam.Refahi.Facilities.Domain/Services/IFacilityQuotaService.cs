using Nezam.Refahi.Facilities.Domain.Enums;

namespace Nezam.Refahi.Facilities.Domain.Services;

/// <summary>
/// Domain Service for quota management and calculations
/// Handles business rules related to quota management across aggregates
/// </summary>
public interface IFacilityQuotaService
{
    /// <summary>
    /// بررسی اینکه آیا دوره می‌تواند درخواست جدید بپذیرد
    /// </summary>
    /// <param name="quota">سهمیه کل دوره</param>
    /// <param name="usedQuota">سهمیه استفاده شده</param>
    /// <param name="status">وضعیت دوره</param>
    /// <param name="startDate">تاریخ شروع</param>
    /// <param name="endDate">تاریخ پایان</param>
    /// <returns>true اگر می‌تواند درخواست بپذیرد</returns>
    bool CanAcceptNewRequest(
        int quota,
        int usedQuota,
        FacilityCycleStatus status,
        DateTime startDate,
        DateTime endDate);

    /// <summary>
    /// محاسبه سهمیه باقیمانده
    /// </summary>
    int CalculateRemainingQuota(int quota, int usedQuota);

    /// <summary>
    /// بررسی اینکه آیا افزایش سهمیه استفاده شده معتبر است
    /// </summary>
    bool CanIncrementQuota(int quota, int usedQuota);
}

