using MCA.SharedKernel.Domain;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Facilities.Domain.ValueObjects;

/// <summary>
/// سیاست احراز صلاحیت - تعیین کننده واجدین شرایط
/// مسئولیت: کول‌داون، محدوده مبلغ، قواعد پویا، مدارک
/// </summary>
public sealed class EligibilityPolicy : ValueObject
{
    public Money? MinAmount { get; private set; }
    public Money? MaxAmount { get; private set; }
    public Money? DefaultAmount { get; private set; }
    public int CooldownDays { get; private set; }
    
    // Document requirements
    public List<string> RequiredDocuments { get; private set; } = new();
    public Dictionary<string, string> DocumentRequirements { get; private set; } = new();
    
    // Dynamic rules
    public Dictionary<string, object> DynamicRules { get; private set; } = new();
    
    // Policy versioning
    public string PolicyVersion { get; private set; } = "1.0";
    public DateTime EffectiveFrom { get; private set; }
    public DateTime? EffectiveTo { get; private set; }

    private EligibilityPolicy() { }

    public EligibilityPolicy(
        Money? minAmount,
        Money? maxAmount,
        Money? defaultAmount,
        int cooldownDays,
        List<string> requiredDocuments,
        Dictionary<string, string> documentRequirements,
        Dictionary<string, object> dynamicRules,
        string policyVersion = "1.0",
        DateTime? effectiveFrom = null)
    {
        MinAmount = minAmount;
        MaxAmount = maxAmount;
        DefaultAmount = defaultAmount;
        CooldownDays = cooldownDays;
        RequiredDocuments = requiredDocuments ?? new List<string>();
        DocumentRequirements = documentRequirements ?? new Dictionary<string, string>();
        DynamicRules = dynamicRules ?? new Dictionary<string, object>();
        PolicyVersion = policyVersion;
        EffectiveFrom = effectiveFrom ?? DateTime.UtcNow;
    }

    /// <summary>
    /// ایجاد نسخه جدید سیاست
    /// </summary>
    public EligibilityPolicy CreateNewVersion(
        Money? minAmount = null,
        Money? maxAmount = null,
        Money? defaultAmount = null,
        int? cooldownDays = null,
        List<string>? requiredDocuments = null,
        Dictionary<string, string>? documentRequirements = null,
        Dictionary<string, object>? dynamicRules = null)
    {
        var newVersion = int.Parse(PolicyVersion.Split('.')[0]) + 1;
        var newPolicyVersion = $"{newVersion}.0";

        return new EligibilityPolicy(
            minAmount ?? MinAmount,
            maxAmount ?? MaxAmount,
            defaultAmount ?? DefaultAmount,
            cooldownDays ?? CooldownDays,
            requiredDocuments ?? RequiredDocuments,
            documentRequirements ?? DocumentRequirements,
            dynamicRules ?? DynamicRules,
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
    /// اعمال adjusters بر مبلغ
    /// </summary>
    public Money ApplyAdjusters(Money baseAmount, Dictionary<string, decimal> adjusters)
    {
        var adjustedAmount = baseAmount;
        
        foreach (var adjuster in adjusters)
        {
            switch (adjuster.Key.ToLower())
            {
                case "multiply":
                    adjustedAmount = new Money(adjustedAmount.AmountRials * adjuster.Value);
                    break;
                case "add":
                    adjustedAmount = new Money(adjustedAmount.AmountRials + adjuster.Value);
                    break;
                case "subtract":
                    adjustedAmount = new Money(adjustedAmount.AmountRials - adjuster.Value);
                    break;
            }
        }

        // اعمال محدودیت‌های min/max
        if (MinAmount != null && adjustedAmount.AmountRials < MinAmount.AmountRials)
            adjustedAmount = MinAmount;
        
        if (MaxAmount != null && adjustedAmount.AmountRials > MaxAmount.AmountRials)
            adjustedAmount = MaxAmount;

        return adjustedAmount;
    }

    /// <summary>
    /// بررسی کول‌داون
    /// </summary>
    public bool IsCooldownSatisfied(DateTime lastDisbursementDate)
    {
        if (CooldownDays <= 0) return true;
        
        var cooldownEndDate = lastDisbursementDate.AddDays(CooldownDays);
        return DateTime.UtcNow >= cooldownEndDate;
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
            ["MinAmount"] = MinAmount?.AmountRials ?? 0,
            ["MaxAmount"] = MaxAmount?.AmountRials ?? 0,
            ["DefaultAmount"] = DefaultAmount?.AmountRials ?? 0,
            ["CooldownDays"] = CooldownDays,
            ["RequiredDocuments"] = RequiredDocuments,
            ["DocumentRequirements"] = DocumentRequirements,
            ["DynamicRules"] = DynamicRules
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return PolicyVersion;
        yield return EffectiveFrom;
        yield return MinAmount?.AmountRials ?? 0;
        yield return MaxAmount?.AmountRials ?? 0;
        yield return DefaultAmount?.AmountRials ?? 0;
        yield return CooldownDays;
    }
}
