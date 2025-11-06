using Nezam.Refahi.Facilities.Domain.Entities;

namespace Nezam.Refahi.Facilities.Domain.Services;

/// <summary>
/// Domain Service for checking member eligibility for facility cycles
/// This service encapsulates business rules that span multiple aggregates
/// 
/// This is a stateless domain service that operates on domain entities
/// It should be called from Application Service which loads aggregates from repositories
/// </summary>
public interface IFacilityCycleEligibilityService
{
    /// <summary>
    /// بررسی کامل واجد شرایط بودن عضو برای یک دوره
    /// این متد باید از Application Service فراخوانی شود که FacilityCycle را لود کرده است
    /// </summary>
    /// <param name="cycle">دوره تسهیلات (باید از Repository لود شده باشد)</param>
    /// <param name="memberId">شناسه عضو</param>
    /// <param name="completedFacilityIds">لیست تسهیلات تکمیل شده توسط عضو</param>
    /// <param name="activeFacilityIds">لیست تسهیلات فعال عضو</param>
    /// <param name="memberFeatures">ویژگی‌های عضو</param>
    /// <param name="memberCapabilities">قابلیت‌های عضو</param>
    /// <returns>نتیجه بررسی واجد شرایط بودن</returns>
    EligibilityResult CheckEligibilityForCycle(
        FacilityCycle cycle,
        Guid memberId,
        IEnumerable<Guid> completedFacilityIds,
        IEnumerable<Guid> activeFacilityIds,
        IEnumerable<string> memberFeatures,
        IEnumerable<string> memberCapabilities);
}

/// <summary>
/// نتیجه بررسی واجد شرایط بودن
/// </summary>
public sealed class EligibilityResult
{
    public bool IsEligible { get; init; }
    public IReadOnlyList<string> Reasons { get; init; } = Array.Empty<string>();

    public static EligibilityResult Eligible() => new() { IsEligible = true };

    public static EligibilityResult NotEligible(params string[] reasons) => new()
    {
        IsEligible = false,
        Reasons = reasons.ToList()
    };
}

