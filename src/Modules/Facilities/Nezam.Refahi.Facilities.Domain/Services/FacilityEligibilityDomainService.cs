using Nezam.Refahi.Facilities.Domain.Entities;
using Nezam.Refahi.Facilities.Domain.Enums;

namespace Nezam.Refahi.Facilities.Domain.Services;

/// <summary>
/// Domain service for facility eligibility validation
/// Single responsibility: validating member eligibility for facility requests based on features and capabilities
/// </summary>
public class FacilityEligibilityDomainService
{
    /// <summary>
    /// بررسی مجوز عضو برای درخواست تسهیلات بر اساس ویژگی‌ها و قابلیت‌ها
    /// هدف: بررسی اینکه آیا عضو دارای حداقل یکی از ویژگی‌ها یا قابلیت‌های مورد نیاز تسهیلات است
    /// نتیجه مورد انتظار: true اگر عضو مجاز باشد، false اگر نباشد
    /// منطق کسب‌وکار: عضو باید حداقل یکی از ویژگی‌ها یا قابلیت‌های مورد نیاز تسهیلات را داشته باشد (OR Logic)
    /// </summary>
    public bool ValidateMemberEligibility(FacilityCycle cycle, IEnumerable<string> memberFeatures, IEnumerable<string> memberCapabilities)
    {
        // اگر تسهیلات هیچ محدودیتی ندارد، همه مجاز هستند
        if (!cycle.Facility.Features.Any() && !cycle.Facility.CapabilityPolicies.Any())
            return true;

        var memberFeaturesList = memberFeatures?.ToList() ?? new List<string>();
        var memberCapabilitiesList = memberCapabilities?.ToList() ?? new List<string>();

        // بررسی ویژگی‌های مورد نیاز تسهیلات
        var requiredFeatures = cycle.Facility.Features
            .Where(f => f.RequirementType == FacilityFeatureRequirementType.Required)
            .Select(f => f.FeatureId)
            .ToList();
        var hasRequiredFeature = !requiredFeatures.Any() || requiredFeatures.Any(f => memberFeaturesList.Contains(f));

        // بررسی قابلیت‌های مورد نیاز تسهیلات (همه قابلیت‌ها اکنون مورد نیاز هستند)
        var requiredCapabilities = cycle.Facility.CapabilityPolicies
            .Select(cp => cp.CapabilityId)
            .ToList();
        var hasRequiredCapability = !requiredCapabilities.Any() || requiredCapabilities.Any(c => memberCapabilitiesList.Contains(c));

        // بررسی ویژگی‌های ممنوع
        var prohibitedFeatures = cycle.Facility.Features
            .Where(f => f.RequirementType == FacilityFeatureRequirementType.Prohibited)
            .Select(f => f.FeatureId)
            .ToList();
        var hasProhibitedFeature = prohibitedFeatures.Any(f => memberFeaturesList.Contains(f));

        // اگر عضو دارای ویژگی ممنوع است، مجاز نیست
        if (hasProhibitedFeature)
            return false;

        // اگر تسهیلات هم ویژگی و هم قابلیت نیاز دارد، عضو باید حداقل یکی از هر کدام را داشته باشد
        if (requiredFeatures.Any() && requiredCapabilities.Any())
        {
            return hasRequiredFeature && hasRequiredCapability;
        }

        // اگر فقط ویژگی یا فقط قابلیت نیاز دارد، داشتن یکی کافی است
        return hasRequiredFeature || hasRequiredCapability;
    }

    /// <summary>
    /// بررسی مجوز عضو برای درخواست تسهیلات بر اساس ویژگی‌ها و قابلیت‌ها
    /// هدف: بررسی اینکه آیا عضو دارای حداقل یکی از ویژگی‌ها یا قابلیت‌های مورد نیاز تسهیلات است
    /// نتیجه مورد انتظار: نتیجه اعتبارسنجی با جزئیات خطا
    /// منطق کسب‌وکار: عضو باید حداقل یکی از ویژگی‌ها یا قابلیت‌های مورد نیاز تسهیلات را داشته باشد (OR Logic)
    /// </summary>
    public EligibilityValidationResult ValidateMemberEligibilityWithDetails(FacilityCycle cycle, IEnumerable<string> memberFeatures, IEnumerable<string> memberCapabilities)
    {
        var errors = new List<string>();

        // اگر تسهیلات هیچ محدودیتی ندارد، همه مجاز هستند
        if (!cycle.Facility.Features.Any() && !cycle.Facility.CapabilityPolicies.Any())
        {
            return new EligibilityValidationResult(true, null, null);
        }

        var memberFeaturesList = memberFeatures?.ToList() ?? new List<string>();
        var memberCapabilitiesList = memberCapabilities?.ToList() ?? new List<string>();

        // بررسی ویژگی‌های مورد نیاز تسهیلات
        var requiredFeatures = cycle.Facility.Features
            .Where(f => f.RequirementType == FacilityFeatureRequirementType.Required)
            .Select(f => f.FeatureId)
            .ToList();
        var hasRequiredFeature = !requiredFeatures.Any() || requiredFeatures.Any(f => memberFeaturesList.Contains(f));

        // بررسی قابلیت‌های مورد نیاز تسهیلات (همه قابلیت‌ها اکنون مورد نیاز هستند)
        var requiredCapabilities = cycle.Facility.CapabilityPolicies
            .Select(cp => cp.CapabilityId)
            .ToList();
        var hasRequiredCapability = !requiredCapabilities.Any() || requiredCapabilities.Any(c => memberCapabilitiesList.Contains(c));

        // بررسی ویژگی‌های ممنوع
        var prohibitedFeatures = cycle.Facility.Features
            .Where(f => f.RequirementType == FacilityFeatureRequirementType.Prohibited)
            .Select(f => f.FeatureId)
            .ToList();
        var hasProhibitedFeature = prohibitedFeatures.Any(f => memberFeaturesList.Contains(f));

        // اگر عضو دارای ویژگی ممنوع است
        if (hasProhibitedFeature)
        {
            var prohibitedMemberFeatures = prohibitedFeatures.Intersect(memberFeaturesList).ToList();
            errors.Add($"شما دارای ویژگی‌های ممنوع هستید که مانع درخواست تسهیلات می‌شود: {string.Join(", ", prohibitedMemberFeatures)}");
        }

        // اگر تسهیلات هم ویژگی و هم قابلیت نیاز دارد
        if (requiredFeatures.Any() && requiredCapabilities.Any())
        {
            if (!hasRequiredFeature && !hasRequiredCapability)
            {
                errors.Add($"شما باید حداقل یکی از ویژگی‌های مورد نیاز ({string.Join(", ", requiredFeatures)}) یا یکی از قابلیت‌های مورد نیاز ({string.Join(", ", requiredCapabilities)}) را داشته باشید");
            }
            else if (!hasRequiredFeature)
            {
                errors.Add($"شما باید حداقل یکی از ویژگی‌های مورد نیاز را داشته باشید: {string.Join(", ", requiredFeatures)}");
            }
            else if (!hasRequiredCapability)
            {
                errors.Add($"شما باید حداقل یکی از قابلیت‌های مورد نیاز را داشته باشید: {string.Join(", ", requiredCapabilities)}");
            }
        }
        // اگر فقط ویژگی نیاز دارد
        else if (requiredFeatures.Any() && !hasRequiredFeature)
        {
            errors.Add($"شما باید حداقل یکی از ویژگی‌های مورد نیاز را داشته باشید: {string.Join(", ", requiredFeatures)}");
        }
        // اگر فقط قابلیت نیاز دارد
        else if (requiredCapabilities.Any() && !hasRequiredCapability)
        {
            errors.Add($"شما باید حداقل یکی از قابلیت‌های مورد نیاز را داشته باشید: {string.Join(", ", requiredCapabilities)}");
        }

        return new EligibilityValidationResult(errors.Count == 0, errors.FirstOrDefault(), errors);
    }
}

/// <summary>
/// نتیجه اعتبارسنجی مجوز عضو برای تسهیلات
/// </summary>
public record EligibilityValidationResult(bool IsEligible, string? ErrorMessage, List<string>? ValidationErrors);
