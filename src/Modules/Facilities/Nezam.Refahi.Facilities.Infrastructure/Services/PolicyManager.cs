using Nezam.Refahi.Facilities.Domain.Entities;
using Nezam.Refahi.Facilities.Domain.Repositories;
using Nezam.Refahi.Facilities.Domain.Services;
using Nezam.Refahi.Facilities.Domain.ValueObjects;

namespace Nezam.Refahi.Facilities.Infrastructure.Services;

public class PolicyManager : IPolicyManager
{
    private readonly IFacilityRepository _facilityRepository;
    private readonly IFacilityCycleRepository _cycleRepository;

    public PolicyManager(IFacilityRepository facilityRepository, IFacilityCycleRepository cycleRepository)
    {
        _facilityRepository = facilityRepository;
        _cycleRepository = cycleRepository;
    }

    public async Task<CombinedPolicy> CombinePoliciesAsync(
        Guid facilityId,
        Guid? cycleId,
        CancellationToken cancellationToken = default)
    {
        var facility = await _facilityRepository.GetByIdAsync(facilityId, cancellationToken);
        if (facility == null)
            throw new ArgumentException($"Facility with ID {facilityId} not found");

        FacilityCycle? cycle = null;
        if (cycleId.HasValue)
        {
            cycle = await _cycleRepository.GetByIdAsync(cycleId.Value, cancellationToken);
        }

        var eligibilityPolicy = CreateEligibilityPolicy(facility, cycle);
        var participationPolicy = CreateParticipationPolicy(facility, cycle);
        var appliedOverrides = new List<PolicyOverride>();

        // Apply cycle overrides
        if (cycle != null)
        {
            appliedOverrides.AddRange(ApplyCycleOverrides(eligibilityPolicy, participationPolicy, cycle));
        }

        return new CombinedPolicy
        {
            EligibilityPolicy = eligibilityPolicy,
            ParticipationPolicy = participationPolicy,
            AppliedOverrides = appliedOverrides,
            CalculatedAt = DateTime.UtcNow
        };
    }

    public async Task<CombinedPolicy> ApplyCycleOverridesAsync(
        EligibilityPolicy facilityEligibilityPolicy,
        ParticipationPolicy facilityParticipationPolicy,
        Guid cycleId,
        CancellationToken cancellationToken = default)
    {
        var cycle = await _cycleRepository.GetByIdAsync(cycleId, cancellationToken);
        if (cycle == null)
            throw new ArgumentException($"Cycle with ID {cycleId} not found");

        var eligibilityPolicy = facilityEligibilityPolicy;
        var participationPolicy = facilityParticipationPolicy;
        var appliedOverrides = new List<PolicyOverride>();

        // Apply cycle overrides
        appliedOverrides.AddRange(ApplyCycleOverrides(eligibilityPolicy, participationPolicy, cycle));

        return new CombinedPolicy
        {
            EligibilityPolicy = eligibilityPolicy,
            ParticipationPolicy = participationPolicy,
            AppliedOverrides = appliedOverrides,
            CalculatedAt = DateTime.UtcNow
        };
    }

    public async Task<PolicySnapshot> CreatePolicySnapshotAsync(
        Guid facilityId,
        Guid? cycleId,
        CancellationToken cancellationToken = default)
    {
        var facility = await _facilityRepository.GetByIdAsync(facilityId, cancellationToken);
        if (facility == null)
            throw new ArgumentException($"Facility with ID {facilityId} not found");

        FacilityCycle? cycle = null;
        if (cycleId.HasValue)
        {
            cycle = await _cycleRepository.GetByIdAsync(cycleId.Value, cancellationToken);
        }

        var eligibilityPolicy = CreateEligibilityPolicy(facility, cycle);
        var participationPolicy = CreateParticipationPolicy(facility, cycle);
        var appliedOverrides = new List<PolicyOverride>();

        if (cycle != null)
        {
            appliedOverrides.AddRange(ApplyCycleOverrides(eligibilityPolicy, participationPolicy, cycle));
        }

        var snapshot = new PolicySnapshot
        {
            FacilityId = facilityId,
            CycleId = cycleId,
            SnapshotVersion = "1.0",
            CreatedAt = DateTime.UtcNow,
            FacilityEligibilitySnapshot = eligibilityPolicy.CreateSnapshot(),
            FacilityParticipationSnapshot = participationPolicy.CreateSnapshot(),
            CycleOverridesSnapshot = cycle?.Metadata?.ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value) ?? new Dictionary<string, object>(),
            AppliedOverrides = appliedOverrides,
            PolicyHash = ComputePolicyHash(eligibilityPolicy, participationPolicy, appliedOverrides)
        };

        return snapshot;
    }

    private EligibilityPolicy CreateEligibilityPolicy(Facility facility, FacilityCycle? cycle)
    {
        var minAmount = cycle?.MinAmount;
        var maxAmount = cycle?.MaxAmount;
        var defaultAmount = cycle?.DefaultAmount;
        var cooldownDays = cycle?.CooldownDays ?? 0;

        return new EligibilityPolicy(
            minAmount,
            maxAmount,
            defaultAmount,
            cooldownDays,
            new List<string>(), // RequiredDocuments
            new Dictionary<string, string>(), // DocumentRequirements
            new Dictionary<string, object>() // DynamicRules
        );
    }

    private ParticipationPolicy CreateParticipationPolicy(Facility facility, FacilityCycle? cycle)
    {
        var isRepeatable = cycle?.IsRepeatable ?? true;
        var exclusiveSetId = cycle?.ExclusiveSetId;
        var maxActiveAcrossCycles = cycle?.MaxActiveAcrossCycles ?? 0;
        var admissionStrategy = cycle?.AdmissionStrategy ?? "FIFO";
        var waitlistCapacity = cycle?.WaitlistCapacity;

        return new ParticipationPolicy(
            isRepeatable,
            exclusiveSetId,
            maxActiveAcrossCycles,
            new List<string>(), // WhitelistFeatures - not available in Facility entity
            new List<string>(), // BlacklistFeatures - not available in Facility entity
            new List<string>(), // WhitelistCapabilities - not available in Facility entity
            new List<string>(), // BlacklistCapabilities - not available in Facility entity
            admissionStrategy,
            waitlistCapacity
        );
    }

    private List<PolicyOverride> ApplyCycleOverrides(EligibilityPolicy eligibilityPolicy, ParticipationPolicy participationPolicy, FacilityCycle cycle)
    {
        var overrides = new List<PolicyOverride>();

        if (cycle.MinAmount != null)
        {
            overrides.Add(new PolicyOverride
            {
                PolicyType = "Eligibility",
                PropertyName = "MinAmount",
                OriginalValue = eligibilityPolicy.MinAmount?.AmountRials,
                OverrideValue = cycle.MinAmount.AmountRials,
                Source = "Cycle",
                AppliedAt = DateTime.UtcNow
            });
        }

        if (cycle.MaxAmount != null)
        {
            overrides.Add(new PolicyOverride
            {
                PolicyType = "Eligibility",
                PropertyName = "MaxAmount",
                OriginalValue = eligibilityPolicy.MaxAmount?.AmountRials,
                OverrideValue = cycle.MaxAmount.AmountRials,
                Source = "Cycle",
                AppliedAt = DateTime.UtcNow
            });
        }

        if (cycle.CooldownDays > 0)
        {
            overrides.Add(new PolicyOverride
            {
                PolicyType = "Eligibility",
                PropertyName = "CooldownDays",
                OriginalValue = eligibilityPolicy.CooldownDays,
                OverrideValue = cycle.CooldownDays,
                Source = "Cycle",
                AppliedAt = DateTime.UtcNow
            });
        }

        if (cycle.IsRepeatable != participationPolicy.IsRepeatable)
        {
            overrides.Add(new PolicyOverride
            {
                PolicyType = "Participation",
                PropertyName = "IsRepeatable",
                OriginalValue = participationPolicy.IsRepeatable,
                OverrideValue = cycle.IsRepeatable,
                Source = "Cycle",
                AppliedAt = DateTime.UtcNow
            });
        }

        if (cycle.ExclusiveSetId != participationPolicy.ExclusiveSetId)
        {
            overrides.Add(new PolicyOverride
            {
                PolicyType = "Participation",
                PropertyName = "ExclusiveSetId",
                OriginalValue = participationPolicy.ExclusiveSetId,
                OverrideValue = cycle.ExclusiveSetId,
                Source = "Cycle",
                AppliedAt = DateTime.UtcNow
            });
        }

        if (cycle.MaxActiveAcrossCycles != participationPolicy.MaxActiveAcrossCycles)
        {
            overrides.Add(new PolicyOverride
            {
                PolicyType = "Participation",
                PropertyName = "MaxActiveAcrossCycles",
                OriginalValue = participationPolicy.MaxActiveAcrossCycles,
                OverrideValue = cycle.MaxActiveAcrossCycles,
                Source = "Cycle",
                AppliedAt = DateTime.UtcNow
            });
        }

        return overrides;
    }

    private string ComputePolicyHash(EligibilityPolicy eligibilityPolicy, ParticipationPolicy participationPolicy, List<PolicyOverride> appliedOverrides)
    {
        var hashInput = $"{eligibilityPolicy.CreateSnapshot()}|{participationPolicy.CreateSnapshot()}|{appliedOverrides.Count}";
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(hashInput));
        return Convert.ToBase64String(bytes)[..16];
    }
}