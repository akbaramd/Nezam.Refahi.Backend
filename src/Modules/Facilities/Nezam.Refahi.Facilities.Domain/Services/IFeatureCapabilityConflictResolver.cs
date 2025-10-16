using Nezam.Refahi.Facilities.Domain.Entities;
using Nezam.Refahi.Facilities.Domain.ValueObjects;

namespace Nezam.Refahi.Facilities.Domain.Services;

/// <summary>
/// سرویس حل تعارضات Feature/Capability
/// </summary>
public interface IFeatureCapabilityConflictResolver
{
    /// <summary>
    /// حل تعارضات بین Features
    /// </summary>
    Task<FeatureConflictResolution> ResolveFeatureConflictsAsync(
        List<FacilityFeature> facilityFeatures,
        List<string> memberFeatures,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// حل تعارضات بین Capabilities
    /// </summary>
    Task<CapabilityConflictResolution> ResolveCapabilityConflictsAsync(
        List<FacilityCapabilityPolicy> facilityCapabilityPolicies,
        List<string> memberCapabilities,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ترکیب نتایج Features و Capabilities
    /// </summary>
    Task<CombinedResolution> CombineResolutionsAsync(
        FeatureConflictResolution featureResolution,
        CapabilityConflictResolution capabilityResolution,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// اعمال Adjusters بر مبلغ
    /// </summary>
    Task<decimal> ApplyAdjustersAsync(
        decimal baseAmount,
        List<CapabilityAdjuster> adjusters,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// نتیجه حل تعارض Features
/// </summary>
public class FeatureConflictResolution
{
    public bool IsEligible { get; set; }
    public List<string> RequiredFeatures { get; set; } = new();
    public List<string> MissingFeatures { get; set; } = new();
    public List<string> BlacklistedFeatures { get; set; } = new();
    public List<string> ConflictingFeatures { get; set; } = new();
    public List<FeatureViolation> Violations { get; set; } = new();
    public string ResolutionStrategy { get; set; } = "Strict"; // Strict, Lenient, Custom
}

/// <summary>
/// نتیجه حل تعارض Capabilities
/// </summary>
public class CapabilityConflictResolution
{
    public bool IsEligible { get; set; }
    public List<string> RequiredCapabilities { get; set; } = new();
    public List<string> MissingCapabilities { get; set; } = new();
    public List<string> BlacklistedCapabilities { get; set; } = new();
    public List<string> ConflictingCapabilities { get; set; } = new();
    public List<CapabilityAdjuster> AppliedAdjusters { get; set; } = new();
    public List<CapabilityViolation> Violations { get; set; } = new();
    public string ResolutionStrategy { get; set; } = "Strict"; // Strict, Lenient, Custom
}

/// <summary>
/// نتیجه ترکیبی
/// </summary>
public class CombinedResolution
{
    public bool IsEligible { get; set; }
    public List<string> AllViolations { get; set; } = new();
    public List<CapabilityAdjuster> FinalAdjusters { get; set; } = new();
    public decimal FinalAmount { get; set; }
    public string ResolutionStrategy { get; set; } = "Strict";
    public Dictionary<string, object> ResolutionDetails { get; set; } = new();
}

/// <summary>
/// تعدیل‌کننده قابلیت
/// </summary>
public class CapabilityAdjuster
{
    public string CapabilityId { get; set; } = null!;
    public string AdjusterType { get; set; } = null!; // Multiply, Add, Subtract, Set
    public decimal Value { get; set; }
    public string? Condition { get; set; }
    public int Priority { get; set; } = 0; // بالاتر = اولویت بیشتر
}

/// <summary>
/// نقض ویژگی
/// </summary>
public class FeatureViolation
{
    public string FeatureId { get; set; } = null!;
    public string ViolationType { get; set; } = null!; // Required, Blacklisted, Conflict
    public string Message { get; set; } = null!;
    public string Severity { get; set; } = "Error"; // Error, Warning, Info
}

/// <summary>
/// نقض قابلیت
/// </summary>
public class CapabilityViolation
{
    public string CapabilityId { get; set; } = null!;
    public string ViolationType { get; set; } = null!; // Required, Blacklisted, Conflict
    public string Message { get; set; } = null!;
    public string Severity { get; set; } = "Error"; // Error, Warning, Info
}
