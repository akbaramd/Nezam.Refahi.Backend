using Nezam.Refahi.Facilities.Domain.Entities;

namespace Nezam.Refahi.Facilities.Domain.Services;

/// <summary>
/// Domain Service for managing and validating facility cycle dependencies
/// Handles business rules related to dependencies between facility cycles
/// 
/// This is a stateless domain service for dependency validation logic
/// </summary>
public sealed class FacilityDependencyService : IFacilityDependencyService
{
    /// <summary>
    /// بررسی اینکه آیا وابستگی‌های دوره دایره‌ای هستند
    /// 
    /// وابستگی دایره‌ای: اگر Cycle A (Facility X) به Facility Y وابسته باشد
    /// و Cycle B (Facility Y) به Facility X وابسته باشد
    /// </summary>
    public bool HasCircularDependency(
        FacilityCycle cycle,
        Guid requiredFacilityId,
        Guid facilityId)
    {
        if (cycle == null)
            throw new ArgumentNullException(nameof(cycle));
        if (requiredFacilityId == Guid.Empty)
            throw new ArgumentException("Required facility ID cannot be empty", nameof(requiredFacilityId));
        if (facilityId == Guid.Empty)
            throw new ArgumentException("Facility ID cannot be empty", nameof(facilityId));

        // بررسی ساده: اگر تسهیلات مورد نیاز همان تسهیلات دوره باشد
        if (requiredFacilityId == facilityId)
        {
            return true;
        }

        // بررسی عمیق‌تر: اگر dependency chain دایره‌ای باشد
        // این نیاز به بررسی بیشتر dependency chain دارد که باید در Application Service انجام شود
        // برای بررسی کامل، باید dependency chain را traverse کنیم
        
        return false;
    }

    /// <summary>
    /// بررسی اینکه آیا وابستگی معتبر است
    /// </summary>
    public bool IsValidDependency(Guid facilityId, Guid requiredFacilityId)
    {
        if (facilityId == Guid.Empty)
            throw new ArgumentException("Facility ID cannot be empty", nameof(facilityId));
        if (requiredFacilityId == Guid.Empty)
            throw new ArgumentException("Required facility ID cannot be empty", nameof(requiredFacilityId));

        // یک تسهیلات نمی‌تواند به خودش وابسته باشد
        if (facilityId == requiredFacilityId)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// بررسی اینکه آیا وابستگی‌های دوره برآورده شده‌اند
    /// </summary>
    public bool AreDependenciesSatisfied(
        IEnumerable<FacilityCycleDependency> dependencies,
        IEnumerable<Guid> completedFacilityIds)
    {
        if (dependencies == null)
            throw new ArgumentNullException(nameof(dependencies));
        if (completedFacilityIds == null)
            throw new ArgumentNullException(nameof(completedFacilityIds));

        var completedSet = completedFacilityIds.ToHashSet();

        foreach (var dependency in dependencies)
        {
            if (dependency.MustBeCompleted && !completedSet.Contains(dependency.RequiredFacilityId))
            {
                return false;
            }
        }

        return true;
    }
}

