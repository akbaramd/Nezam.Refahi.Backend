using Nezam.Refahi.Facilities.Domain.Entities;
using Nezam.Refahi.Facilities.Domain.Services;

namespace Nezam.Refahi.Facilities.Infrastructure.Services;

public class FeatureCapabilityConflictResolver : IFeatureCapabilityConflictResolver
{
    public async Task<FeatureConflictResolution> ResolveFeatureConflictsAsync(
        List<FacilityFeature> facilityFeatures,
        List<string> memberFeatures,
        CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;

        var resolution = new FeatureConflictResolution
        {
            IsEligible = true,
            ResolutionStrategy = "Strict"
        };

        foreach (var facilityFeature in facilityFeatures)
        {
            switch (facilityFeature.RequirementType)
            {
                case Domain.Enums.FacilityFeatureRequirementType.Required:
                    if (!memberFeatures.Contains(facilityFeature.FeatureId))
                    {
                        resolution.IsEligible = false;
                        resolution.MissingFeatures.Add(facilityFeature.FeatureId);
                        resolution.Violations.Add(new FeatureViolation
                        {
                            FeatureId = facilityFeature.FeatureId,
                            ViolationType = "Required",
                            Message = $"Required feature '{facilityFeature.FeatureId}' is missing",
                            Severity = "Error"
                        });
                    }
                    else
                    {
                        resolution.RequiredFeatures.Add(facilityFeature.FeatureId);
                    }
                    break;

                case Domain.Enums.FacilityFeatureRequirementType.Prohibited:
                    if (memberFeatures.Contains(facilityFeature.FeatureId))
                    {
                        resolution.IsEligible = false;
                        resolution.BlacklistedFeatures.Add(facilityFeature.FeatureId);
                        resolution.Violations.Add(new FeatureViolation
                        {
                            FeatureId = facilityFeature.FeatureId,
                            ViolationType = "Prohibited",
                            Message = $"Feature '{facilityFeature.FeatureId}' is prohibited",
                            Severity = "Error"
                        });
                    }
                    break;

                case Domain.Enums.FacilityFeatureRequirementType.Modifier:
                    // Modifier features don't affect eligibility directly
                    break;
            }
        }

        return resolution;
    }

    public async Task<CapabilityConflictResolution> ResolveCapabilityConflictsAsync(
        List<FacilityCapabilityPolicy> facilityCapabilityPolicies,
        List<string> memberCapabilities,
        CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;

        var resolution = new CapabilityConflictResolution
        {
            IsEligible = true,
            ResolutionStrategy = "Strict"
        };

        foreach (var capabilityPolicy in facilityCapabilityPolicies)
        {
            switch (capabilityPolicy.PolicyType)
            {
                case Domain.Enums.CapabilityPolicyType.Required:
                    if (!memberCapabilities.Contains(capabilityPolicy.CapabilityId))
                    {
                        resolution.IsEligible = false;
                        resolution.MissingCapabilities.Add(capabilityPolicy.CapabilityId);
                        resolution.Violations.Add(new CapabilityViolation
                        {
                            CapabilityId = capabilityPolicy.CapabilityId,
                            ViolationType = "Required",
                            Message = $"Required capability '{capabilityPolicy.CapabilityId}' is missing",
                            Severity = "Error"
                        });
                    }
                    else
                    {
                        resolution.RequiredCapabilities.Add(capabilityPolicy.CapabilityId);
                    }
                    break;

                case Domain.Enums.CapabilityPolicyType.Prohibited:
                    if (memberCapabilities.Contains(capabilityPolicy.CapabilityId))
                    {
                        resolution.IsEligible = false;
                        resolution.BlacklistedCapabilities.Add(capabilityPolicy.CapabilityId);
                        resolution.Violations.Add(new CapabilityViolation
                        {
                            CapabilityId = capabilityPolicy.CapabilityId,
                            ViolationType = "Prohibited",
                            Message = $"Capability '{capabilityPolicy.CapabilityId}' is prohibited",
                            Severity = "Error"
                        });
                    }
                    break;

                case Domain.Enums.CapabilityPolicyType.AmountModifier:
                case Domain.Enums.CapabilityPolicyType.QuotaModifier:
                case Domain.Enums.CapabilityPolicyType.PriorityModifier:
                    if (memberCapabilities.Contains(capabilityPolicy.CapabilityId))
                    {
                        var adjuster = new CapabilityAdjuster
                        {
                            CapabilityId = capabilityPolicy.CapabilityId,
                            AdjusterType = capabilityPolicy.PolicyType.ToString(),
                            Value = capabilityPolicy.ModifierValue ?? 1.0m,
                            Priority = 0
                        };
                        resolution.AppliedAdjusters.Add(adjuster);
                    }
                    break;
            }
        }

        return resolution;
    }

    public async Task<CombinedResolution> CombineResolutionsAsync(
        FeatureConflictResolution featureResolution,
        CapabilityConflictResolution capabilityResolution,
        CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;

        var combinedResolution = new CombinedResolution
        {
            IsEligible = featureResolution.IsEligible && capabilityResolution.IsEligible,
            ResolutionStrategy = "Strict",
            FinalAmount = 0 // This would be calculated based on adjusters
        };

        // Combine all violations
        combinedResolution.AllViolations.AddRange(featureResolution.Violations.Select(v => v.Message));
        combinedResolution.AllViolations.AddRange(capabilityResolution.Violations.Select(v => v.Message));

        // Combine adjusters
        combinedResolution.FinalAdjusters.AddRange(capabilityResolution.AppliedAdjusters);

        // Set resolution details
        combinedResolution.ResolutionDetails["FeatureEligible"] = featureResolution.IsEligible;
        combinedResolution.ResolutionDetails["CapabilityEligible"] = capabilityResolution.IsEligible;
        combinedResolution.ResolutionDetails["TotalViolations"] = combinedResolution.AllViolations.Count;
        combinedResolution.ResolutionDetails["AppliedAdjusters"] = combinedResolution.FinalAdjusters.Count;

        return combinedResolution;
    }

    public async Task<decimal> ApplyAdjustersAsync(
        decimal baseAmount,
        List<CapabilityAdjuster> adjusters,
        CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;

        var adjustedAmount = baseAmount;

        // Sort adjusters by priority (higher priority first)
        var sortedAdjusters = adjusters.OrderByDescending(a => a.Priority).ToList();

        foreach (var adjuster in sortedAdjusters)
        {
            switch (adjuster.AdjusterType.ToLower())
            {
                case "amountmodifier":
                    adjustedAmount *= adjuster.Value;
                    break;
                case "quotamodifier":
                    // Quota modifiers don't affect amount directly
                    break;
                case "prioritymodifier":
                    // Priority modifiers don't affect amount directly
                    break;
                case "multiply":
                    adjustedAmount *= adjuster.Value;
                    break;
                case "add":
                    adjustedAmount += adjuster.Value;
                    break;
                case "subtract":
                    adjustedAmount -= adjuster.Value;
                    break;
                case "set":
                    adjustedAmount = adjuster.Value;
                    break;
            }
        }

        return adjustedAmount;
    }
}