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
        List<FacilityCapability> facilityCapabilityPolicies,
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
            // All capabilities are now treated as required
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
            FinalAmount = 0 // No adjusters anymore, so amount remains unchanged
        };

        // Combine all violations
        combinedResolution.AllViolations.AddRange(featureResolution.Violations.Select(v => v.Message));
        combinedResolution.AllViolations.AddRange(capabilityResolution.Violations.Select(v => v.Message));

        // No adjusters to combine since capabilities no longer have modifiers

        // Set resolution details
        combinedResolution.ResolutionDetails["FeatureEligible"] = featureResolution.IsEligible;
        combinedResolution.ResolutionDetails["CapabilityEligible"] = capabilityResolution.IsEligible;
        combinedResolution.ResolutionDetails["TotalViolations"] = combinedResolution.AllViolations.Count;
        combinedResolution.ResolutionDetails["AppliedAdjusters"] = 0; // No adjusters anymore

        return combinedResolution;
    }

    public async Task<decimal> ApplyAdjustersAsync(
        decimal baseAmount,
        List<CapabilityAdjuster> adjusters,
        CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;

        // Since capabilities no longer have modifiers/adjusters, 
        // simply return the base amount unchanged
        return baseAmount;
    }
}