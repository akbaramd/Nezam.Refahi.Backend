using Nezam.Refahi.Facilities.Domain.ValueObjects;

namespace Nezam.Refahi.Facilities.Domain.Services;

/// <summary>
/// مدیریت تقدم سیاست‌ها
/// تقدم: Cycle Override > Facility Policy > Defaults
/// </summary>
public interface IPolicyManager
{
    /// <summary>
    /// ترکیب سیاست‌ها با تقدم صحیح
    /// </summary>
    Task<CombinedPolicy> CombinePoliciesAsync(
        Guid facilityId,
        Guid? cycleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// اعمال Override های دوره بر سیاست تسهیلات
    /// </summary>
    Task<CombinedPolicy> ApplyCycleOverridesAsync(
        EligibilityPolicy facilityEligibilityPolicy,
        ParticipationPolicy facilityParticipationPolicy,
        Guid cycleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ایجاد snapshot کامل سیاست‌ها
    /// </summary>
    Task<PolicySnapshot> CreatePolicySnapshotAsync(
        Guid facilityId,
        Guid? cycleId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// سیاست ترکیبی نهایی
/// </summary>
public class CombinedPolicy
{
    public EligibilityPolicy EligibilityPolicy { get; set; } = null!;
    public ParticipationPolicy ParticipationPolicy { get; set; } = null!;
    public List<PolicyOverride> AppliedOverrides { get; set; } = new();
    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Override اعمال شده
/// </summary>
public class PolicyOverride
{
    public string PolicyType { get; set; } = null!; // Eligibility, Participation
    public string PropertyName { get; set; } = null!;
    public object? OriginalValue { get; set; }
    public object? OverrideValue { get; set; }
    public string Source { get; set; } = null!; // Cycle, Facility, Default
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Snapshot کامل سیاست‌ها برای audit
/// </summary>
public class PolicySnapshot
{
    public Guid FacilityId { get; set; }
    public Guid? CycleId { get; set; }
    public string SnapshotVersion { get; set; } = "1.0";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Dictionary<string, object> FacilityEligibilitySnapshot { get; set; } = new();
    public Dictionary<string, object> FacilityParticipationSnapshot { get; set; } = new();
    public Dictionary<string, object> CycleOverridesSnapshot { get; set; } = new();
    public Dictionary<string, object> FinalPolicySnapshot { get; set; } = new();
    
    public List<PolicyOverride> AppliedOverrides { get; set; } = new();
    public string PolicyHash { get; set; } = null!; // برای یکتایی
}
