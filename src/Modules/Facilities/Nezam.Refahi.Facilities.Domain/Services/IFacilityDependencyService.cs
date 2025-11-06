using Nezam.Refahi.Facilities.Domain.Entities;

namespace Nezam.Refahi.Facilities.Domain.Services;

/// <summary>
/// Domain Service for managing and validating facility cycle dependencies
/// Handles business rules related to dependencies between facility cycles
/// 
/// This is a stateless domain service for dependency validation logic
/// </summary>
public interface IFacilityDependencyService
{
    /// <summary>
    /// بررسی اینکه آیا وابستگی‌های دوره دایره‌ای هستند
    /// این متد نیاز به دسترسی به FacilityCycle دارد که باید از Repository لود شود
    /// </summary>
    /// <param name="cycle">دوره تسهیلات (باید از Repository لود شده باشد)</param>
    /// <param name="requiredFacilityId">شناسه تسهیلات مورد نیاز</param>
    /// <param name="facilityId">شناسه تسهیلات دوره (برای بررسی دایره‌ای)</param>
    /// <returns>true اگر وابستگی دایره‌ای وجود دارد</returns>
    bool HasCircularDependency(
        FacilityCycle cycle,
        Guid requiredFacilityId,
        Guid facilityId);

    /// <summary>
    /// بررسی اینکه آیا وابستگی معتبر است
    /// </summary>
    /// <param name="facilityId">شناسه تسهیلات دوره</param>
    /// <param name="requiredFacilityId">شناسه تسهیلات مورد نیاز</param>
    /// <returns>true اگر معتبر باشد</returns>
    bool IsValidDependency(Guid facilityId, Guid requiredFacilityId);

    /// <summary>
    /// بررسی اینکه آیا وابستگی‌های دوره برآورده شده‌اند
    /// </summary>
    /// <param name="dependencies">وابستگی‌های دوره</param>
    /// <param name="completedFacilityIds">لیست تسهیلات تکمیل شده توسط عضو</param>
    /// <returns>true اگر همه وابستگی‌ها برآورده شده‌اند</returns>
    bool AreDependenciesSatisfied(
        IEnumerable<FacilityCycleDependency> dependencies,
        IEnumerable<Guid> completedFacilityIds);
}

