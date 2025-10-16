using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Facilities.Domain.ValueObjects;

/// <summary>
/// سیاست مشارکت - تعیین کننده نحوه مشارکت در دوره
/// مسئولیت: تکرارپذیری، ظرفیت، انحصار، لیست سفید/سیاه
/// </summary>
public sealed class ParticipationPolicy : ValueObject
{
    public bool IsRepeatable { get; private set; }
    public string? ExclusiveSetId { get; private set; }
    public int MaxActiveAcrossCycles { get; private set; }
    public List<string> WhitelistFeatures { get; private set; } = new();
    public List<string> BlacklistFeatures { get; private set; } = new();
    public List<string> WhitelistCapabilities { get; private set; } = new();
    public List<string> BlacklistCapabilities { get; private set; } = new();
    
    // Admission strategy
    public string AdmissionStrategy { get; private set; } = "FIFO"; // FIFO, Score, Lottery
    public int? WaitlistCapacity { get; private set; }
    
    // Policy versioning
    public string PolicyVersion { get; private set; } = "1.0";
    public DateTime EffectiveFrom { get; private set; }
    public DateTime? EffectiveTo { get; private set; }

    private ParticipationPolicy() { }

    public ParticipationPolicy(
        bool isRepeatable,
        string? exclusiveSetId,
        int maxActiveAcrossCycles,
        List<string> whitelistFeatures,
        List<string> blacklistFeatures,
        List<string> whitelistCapabilities,
        List<string> blacklistCapabilities,
        string admissionStrategy = "FIFO",
        int? waitlistCapacity = null,
        string policyVersion = "1.0",
        DateTime? effectiveFrom = null)
    {
        IsRepeatable = isRepeatable;
        ExclusiveSetId = exclusiveSetId;
        MaxActiveAcrossCycles = maxActiveAcrossCycles;
        WhitelistFeatures = whitelistFeatures ?? new List<string>();
        BlacklistFeatures = blacklistFeatures ?? new List<string>();
        WhitelistCapabilities = whitelistCapabilities ?? new List<string>();
        BlacklistCapabilities = blacklistCapabilities ?? new List<string>();
        AdmissionStrategy = admissionStrategy;
        WaitlistCapacity = waitlistCapacity;
        PolicyVersion = policyVersion;
        EffectiveFrom = effectiveFrom ?? DateTime.UtcNow;
    }

    /// <summary>
    /// ایجاد نسخه جدید سیاست
    /// </summary>
    public ParticipationPolicy CreateNewVersion(
        bool? isRepeatable = null,
        string? exclusiveSetId = null,
        int? maxActiveAcrossCycles = null,
        List<string>? whitelistFeatures = null,
        List<string>? blacklistFeatures = null,
        List<string>? whitelistCapabilities = null,
        List<string>? blacklistCapabilities = null,
        string? admissionStrategy = null,
        int? waitlistCapacity = null)
    {
        var newVersion = int.Parse(PolicyVersion.Split('.')[0]) + 1;
        var newPolicyVersion = $"{newVersion}.0";

        return new ParticipationPolicy(
            isRepeatable ?? IsRepeatable,
            exclusiveSetId ?? ExclusiveSetId,
            maxActiveAcrossCycles ?? MaxActiveAcrossCycles,
            whitelistFeatures ?? WhitelistFeatures,
            blacklistFeatures ?? BlacklistFeatures,
            whitelistCapabilities ?? WhitelistCapabilities,
            blacklistCapabilities ?? BlacklistCapabilities,
            admissionStrategy ?? AdmissionStrategy,
            waitlistCapacity ?? WaitlistCapacity,
            newPolicyVersion,
            DateTime.UtcNow);
    }

    /// <summary>
    /// بررسی اعتبار سیاست در زمان مشخص
    /// </summary>
    public bool IsEffectiveAt(DateTime date)
    {
        return date >= EffectiveFrom && (EffectiveTo == null || date <= EffectiveTo);
    }

    /// <summary>
    /// ایجاد snapshot برای audit
    /// </summary>
    public Dictionary<string, object> CreateSnapshot()
    {
        return new Dictionary<string, object>
        {
            ["PolicyVersion"] = PolicyVersion,
            ["EffectiveFrom"] = EffectiveFrom,
            ["EffectiveTo"] = EffectiveTo ?? DateTime.MaxValue,
            ["IsRepeatable"] = IsRepeatable,
            ["ExclusiveSetId"] = ExclusiveSetId ?? string.Empty,
            ["MaxActiveAcrossCycles"] = MaxActiveAcrossCycles,
            ["WhitelistFeatures"] = WhitelistFeatures,
            ["BlacklistFeatures"] = BlacklistFeatures,
            ["WhitelistCapabilities"] = WhitelistCapabilities,
            ["BlacklistCapabilities"] = BlacklistCapabilities,
            ["AdmissionStrategy"] = AdmissionStrategy,
            ["WaitlistCapacity"] = WaitlistCapacity ?? 0
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return PolicyVersion;
        yield return EffectiveFrom;
        yield return IsRepeatable;
        yield return ExclusiveSetId ?? string.Empty;
        yield return MaxActiveAcrossCycles;
        yield return AdmissionStrategy;
        yield return WaitlistCapacity ?? 0;
    }
}
