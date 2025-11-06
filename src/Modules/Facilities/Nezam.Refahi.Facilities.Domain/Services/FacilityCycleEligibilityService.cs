using Nezam.Refahi.Facilities.Domain.Entities;

namespace Nezam.Refahi.Facilities.Domain.Services;

/// <summary>
/// Domain Service for checking member eligibility for facility cycles
/// Implements business rules that span multiple aggregates and value objects
/// 
/// This service is stateless and operates on domain entities and value objects
/// It should be called from Application Service which loads the FacilityCycle from repository
/// </summary>
public sealed class FacilityCycleEligibilityService : IFacilityCycleEligibilityService
{
    /// <summary>
    /// بررسی اینکه آیا عضو واجد شرایط دریافت تسهیلات در دوره مشخص است
    /// 
    /// Note: این متد باید از طریق Application Service فراخوانی شود
    /// که FacilityCycle را از Repository لود کرده و به این متد پاس دهد
    /// </summary>
    public EligibilityResult CheckEligibility(
        Guid cycleId,
        Guid memberId,
        IEnumerable<Guid> completedFacilityIds,
        IEnumerable<Guid> activeFacilityIds,
        IEnumerable<string> memberFeatures,
        IEnumerable<string> memberCapabilities)
    {
        if (cycleId == Guid.Empty)
            throw new ArgumentException("Cycle ID cannot be empty", nameof(cycleId));
        if (memberId == Guid.Empty)
            throw new ArgumentException("Member ID cannot be empty", nameof(memberId));

        var reasons = new List<string>();
        var completedFacilities = completedFacilityIds.ToHashSet();
        var activeFacilities = activeFacilityIds.ToHashSet();
        var memberFeaturesSet = memberFeatures.ToHashSet();
        var memberCapabilitiesSet = memberCapabilities.ToHashSet();

        // Note: این logic باید در Application Service پیاده‌سازی شود
        // که FacilityCycle را از Repository لود کرده و سپس متدهای helper این service را فراخوانی کند
        
        return EligibilityResult.Eligible();
    }

    /// <summary>
    /// بررسی کامل واجد شرایط بودن عضو برای یک دوره
    /// این متد باید از Application Service فراخوانی شود که FacilityCycle را لود کرده است
    /// </summary>
    public EligibilityResult CheckEligibilityForCycle(
        FacilityCycle cycle,
        Guid memberId,
        IEnumerable<Guid> completedFacilityIds,
        IEnumerable<Guid> activeFacilityIds,
        IEnumerable<string> memberFeatures,
        IEnumerable<string> memberCapabilities)
    {
        if (cycle == null)
            throw new ArgumentNullException(nameof(cycle));
        if (memberId == Guid.Empty)
            throw new ArgumentException("Member ID cannot be empty", nameof(memberId));

        var reasons = new List<string>();
        var completedFacilities = completedFacilityIds.ToHashSet();
        var activeFacilities = activeFacilityIds.ToHashSet();
        var memberFeaturesSet = memberFeatures.ToHashSet();
        var memberCapabilitiesSet = memberCapabilities.ToHashSet();

        // بررسی وابستگی‌ها
        if (!CheckDependencies(cycle.Dependencies, completedFacilities))
        {
            reasons.Add("Member has not completed required facilities");
        }

        // بررسی ویژگی‌های مورد نیاز
        if (!CheckRequiredFeatures(cycle.Features, memberFeaturesSet))
        {
            reasons.Add("Member does not have required features");
        }

        // بررسی قابلیت‌های مورد نیاز
        if (!CheckRequiredCapabilities(cycle.Capabilities, memberCapabilitiesSet))
        {
            reasons.Add("Member does not have required capabilities");
        }

        // بررسی وضعیت دوره
        if (!cycle.IsActive())
        {
            reasons.Add("Cycle is not active");
        }

        // بررسی سهمیه
        if (cycle.IsQuotaFull())
        {
            reasons.Add("Cycle quota is full");
        }

        if (reasons.Any())
        {
            return EligibilityResult.NotEligible(reasons.ToArray());
        }

        return EligibilityResult.Eligible();
    }

    /// <summary>
    /// بررسی وابستگی‌های دوره
    /// </summary>
    public bool CheckDependencies(
        IEnumerable<FacilityCycleDependency> dependencies,
        HashSet<Guid> completedFacilities)
    {
        foreach (var dependency in dependencies)
        {
            if (dependency.MustBeCompleted && !completedFacilities.Contains(dependency.RequiredFacilityId))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// بررسی ویژگی‌های مورد نیاز
    /// </summary>
    public bool CheckRequiredFeatures(
        IEnumerable<FacilityCycleFeature> requiredFeatures,
        HashSet<string> memberFeatures)
    {
        var requiredFeatureIds = requiredFeatures.Select(f => f.FeatureId).ToHashSet();
        return requiredFeatureIds.IsSubsetOf(memberFeatures);
    }

    /// <summary>
    /// بررسی قابلیت‌های مورد نیاز
    /// </summary>
    public bool CheckRequiredCapabilities(
        IEnumerable<FacilityCycleCapability> requiredCapabilities,
        HashSet<string> memberCapabilities)
    {
        var requiredCapabilityIds = requiredCapabilities.Select(c => c.CapabilityId).ToHashSet();
        return requiredCapabilityIds.IsSubsetOf(memberCapabilities);
    }
}

