using Nezam.Refahi.Recreation.Application.Services.Contracts;

namespace Nezam.Refahi.Recreation.Application.Services;

/// <summary>
/// Service for mapping internal IDs to user-friendly display names
/// </summary>
public class DisplayNameService : IDisplayNameService
{
    private static readonly Dictionary<string, string> CapabilityMappings = new()
    {
        ["structure_supervisor_grade1"] = "ناظر سازه درجه یک",
        ["architecture_supervisor_grade1"] = "ناظر معماری درجه یک",
        ["designer_grade1"] = "طراح درجه یک",
        ["execute_grade1"] = "مجری درجه یک",
        ["architecture_execute_grade2"] = "مجری معماری درجه دو",
        ["عضویت_فعال"] = "عضویت فعال",
        ["بیمه_معتبر"] = "بیمه معتبر",
        ["مجوز_سفر"] = "مجوز سفر"
    };

    private static readonly Dictionary<string, string> FeatureMappings = new()
    {
        ["structure"] = "سازه",
        ["architecture"] = "معماری",
        ["execute"] = "اجرا",
        ["grade1"] = "درجه یک",
        ["grade2"] = "درجه دو",
        ["grade3"] = "درجه سه",
        ["supervisor"] = "ناظر",
        ["designer_grade1"] = "طراح درجه یک",
        ["علاقه_به_عکاسی"] = "علاقه به عکاسی",
        ["طبیعت‌دوست"] = "طبیعت‌دوست",
        ["علاقه_به_تاریخ"] = "علاقه به تاریخ",
        ["عاشق_معماری"] = "عاشق معماری",
        ["عاشق_شعر"] = "عاشق شعر",
        ["علاقه_مند_فرهنگ"] = "علاقه‌مند فرهنگ"
    };

    /// <summary>
    /// Gets display name for a capability ID
    /// </summary>
    public string GetCapabilityDisplayName(string capabilityId)
    {
        if (string.IsNullOrWhiteSpace(capabilityId))
            return string.Empty;

        return CapabilityMappings.TryGetValue(capabilityId, out var displayName) 
            ? displayName 
            : capabilityId;
    }

    /// <summary>
    /// Gets display name for a feature ID
    /// </summary>
    public string GetFeatureDisplayName(string featureId)
    {
        if (string.IsNullOrWhiteSpace(featureId))
            return string.Empty;

        return FeatureMappings.TryGetValue(featureId, out var displayName) 
            ? displayName 
            : featureId;
    }

    /// <summary>
    /// Gets all available capability mappings
    /// </summary>
    public IDictionary<string, string> GetAllCapabilityMappings()
    {
        return new Dictionary<string, string>(CapabilityMappings);
    }

    /// <summary>
    /// Gets all available feature mappings
    /// </summary>
    public IDictionary<string, string> GetAllFeatureMappings()
    {
        return new Dictionary<string, string>(FeatureMappings);
    }
}
