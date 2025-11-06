namespace Nezam.Refahi.Facilities.Domain.Services;

/// <summary>
/// Domain Service for validating facility cycle business rules
/// Implements cross-aggregate validation logic
/// </summary>
public interface IFacilityCycleValidationService
{
    /// <summary>
    /// بررسی اینکه آیا نام دوره در یک تسهیلات یکتا است
    /// </summary>
    /// <param name="facilityId">شناسه تسهیلات</param>
    /// <param name="cycleName">نام دوره</param>
    /// <param name="excludeCycleId">شناسه دوره برای حذف از بررسی (برای به‌روزرسانی)</param>
    /// <returns>true اگر یکتا باشد</returns>
    Task<bool> IsCycleNameUniqueAsync(
        Guid facilityId,
        string cycleName,
        Guid? excludeCycleId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// بررسی اینکه آیا دوره جدید با دوره‌های موجود تداخل زمانی دارد
    /// </summary>
    /// <param name="facilityId">شناسه تسهیلات</param>
    /// <param name="startDate">تاریخ شروع دوره جدید</param>
    /// <param name="endDate">تاریخ پایان دوره جدید</param>
    /// <param name="excludeCycleId">شناسه دوره برای حذف از بررسی (برای به‌روزرسانی)</param>
    /// <returns>لیست دوره‌های تداخل‌دار</returns>
    Task<IEnumerable<Guid>> GetOverlappingCyclesAsync(
        Guid facilityId,
        DateTime startDate,
        DateTime endDate,
        Guid? excludeCycleId = null,
        CancellationToken cancellationToken = default);

}

