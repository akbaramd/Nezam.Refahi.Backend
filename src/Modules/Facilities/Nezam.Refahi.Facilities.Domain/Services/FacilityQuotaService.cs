using Nezam.Refahi.Facilities.Domain.Enums;

namespace Nezam.Refahi.Facilities.Domain.Services;

/// <summary>
/// Domain Service for quota management and calculations
/// Handles business rules related to quota management across aggregates
/// </summary>
public sealed class FacilityQuotaService : IFacilityQuotaService
{
    /// <summary>
    /// بررسی اینکه آیا دوره می‌تواند درخواست جدید بپذیرد
    /// </summary>
    public bool CanAcceptNewRequest(
        int quota,
        int usedQuota,
        FacilityCycleStatus status,
        DateTime startDate,
        DateTime endDate)
    {
        if (quota <= 0)
            return false;

        if (usedQuota >= quota)
            return false;

        if (status != FacilityCycleStatus.Active)
            return false;

        var now = DateTime.UtcNow;
        if (now < startDate || now > endDate)
            return false;

        return true;
    }

    /// <summary>
    /// محاسبه سهمیه باقیمانده
    /// </summary>
    public int CalculateRemainingQuota(int quota, int usedQuota)
    {
        if (quota <= 0)
            return 0;

        return Math.Max(0, quota - usedQuota);
    }

    /// <summary>
    /// بررسی اینکه آیا افزایش سهمیه استفاده شده معتبر است
    /// </summary>
    public bool CanIncrementQuota(int quota, int usedQuota)
    {
        if (quota <= 0)
            return false;

        return usedQuota < quota;
    }
}

