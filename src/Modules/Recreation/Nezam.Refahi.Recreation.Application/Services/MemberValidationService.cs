using Microsoft.Extensions.Logging;
using Nezam.Refahi.Membership.Contracts.Dtos;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Recreation.Application.Services;

/// <summary>
/// Service for validating members and their capabilities/features for recreation activities
/// </summary>
public class MemberValidationService
{
    private readonly IMemberInfoService _memberInfoService;
    private readonly ILogger<MemberValidationService> _logger;

    public MemberValidationService(
        IMemberInfoService memberInfoService,
        ILogger<MemberValidationService> logger)
    {
        _memberInfoService = memberInfoService ?? throw new ArgumentNullException(nameof(memberInfoService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets member information by national code
    /// </summary>
    /// <param name="nationalCode">The member's national code</param>
    /// <returns>Member information with capabilities and features if found, null otherwise</returns>
    public async Task<MemberInfoDto?> GetMemberInfoAsync(string nationalCode)
    {
        try
        {
            _logger.LogDebug("Getting member info for national code: {NationalCode}", nationalCode);
            var nationalId = new NationalId(nationalCode);
            return await _memberInfoService.GetMemberInfoAsync(nationalId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting member info for national code: {NationalCode}", nationalCode);
            return null;
        }
    }

    /// <summary>
    /// Gets member information by national ID
    /// </summary>
    /// <param name="nationalId">The member's national ID</param>
    /// <returns>Member information with capabilities and features if found, null otherwise</returns>
    public async Task<MemberInfoDto?> GetMemberInfoAsync(NationalId nationalId)
    {
        try
        {
            _logger.LogDebug("Getting member info for national ID: {NationalCode}", nationalId.Value);
            return await _memberInfoService.GetMemberInfoAsync(nationalId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting member info for national ID: {NationalCode}", nationalId.Value);
            return null;
        }
    }

    /// <summary>
    /// Gets member information by external user ID (for authenticated users)
    /// </summary>
    /// <param name="externalUserId">The external user ID</param>
    /// <returns>Member information with capabilities and features if found, null otherwise</returns>
    public async Task<MemberInfoDto?> GetMemberInfoByExternalIdAsync(string externalUserId)
    {
        try
        {
            _logger.LogDebug("Getting member info for external user ID: {ExternalUserId}", externalUserId);
            return await _memberInfoService.GetMemberInfoByExternalIdAsync(externalUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting member info for external user ID: {ExternalUserId}", externalUserId);
            return null;
        }
    }

    /// <summary>
    /// Checks if a member exists
    /// </summary>
    /// <param name="nationalCode">The member's national code</param>
    /// <returns>True if member exists, false otherwise</returns>
    public async Task<bool> IsMemberExistsAsync(string nationalCode)
    {
        try
        {
            _logger.LogDebug("Checking if member exists for national code: {NationalCode}", nationalCode);
            var nationalId = new NationalId(nationalCode);
            return await _memberInfoService.IsMemberExistsAsync(nationalId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if member exists for national code: {NationalCode}", nationalCode);
            return false;
        }
    }

    /// <summary>
    /// Checks if a member has an active membership
    /// </summary>
    /// <param name="nationalCode">The member's national code</param>
    /// <returns>True if member has active membership, false otherwise</returns>
    public async Task<bool> HasActiveMembershipAsync(string nationalCode)
    {
        try
        {
            _logger.LogDebug("Checking active membership for national code: {NationalCode}", nationalCode);
            var nationalId = new NationalId(nationalCode);
            return await _memberInfoService.HasActiveMembershipAsync(nationalId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking active membership for national code: {NationalCode}", nationalCode);
            return false;
        }
    }

    /// <summary>
    /// Checks if a member has a specific capability
    /// </summary>
    /// <param name="nationalCode">The member's national code</param>
    /// <param name="capabilityKey">The capability key to check</param>
    /// <returns>True if member has the capability, false otherwise</returns>
    public async Task<bool> HasCapabilityAsync(string nationalCode, string capabilityKey)
    {
        try
        {
            _logger.LogDebug("Checking capability {CapabilityKey} for member {NationalCode}", capabilityKey, nationalCode);
            var nationalId = new NationalId(nationalCode);
            return await _memberInfoService.HasCapabilityAsync(nationalId, capabilityKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking capability {CapabilityKey} for member {NationalCode}", capabilityKey, nationalCode);
            return false;
        }
    }

    /// <summary>
    /// Checks if a member has a specific feature
    /// </summary>
    /// <param name="nationalCode">The member's national code</param>
    /// <param name="featureKey">The feature key to check</param>
    /// <returns>True if member has the feature, false otherwise</returns>
    public async Task<bool> HasFeatureAsync(string nationalCode, string featureKey)
    {
        try
        {
            _logger.LogDebug("Checking feature {FeatureKey} for member {NationalCode}", featureKey, nationalCode);
            var nationalId = new NationalId(nationalCode);
            return await _memberInfoService.HasFeatureAsync(nationalId, featureKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking feature {FeatureKey} for member {NationalCode}", featureKey, nationalCode);
            return false;
        }
    }

    /// <summary>
    /// Gets member capabilities
    /// </summary>
    /// <param name="nationalCode">The member's national code</param>
    /// <returns>List of member capability keys</returns>
    public async Task<IEnumerable<string>> GetMemberCapabilitiesAsync(string nationalCode)
    {
        try
        {
            _logger.LogDebug("Getting capabilities for member {NationalCode}", nationalCode);
            var nationalId = new NationalId(nationalCode);
            return await _memberInfoService.GetMemberCapabilitiesAsync(nationalId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting capabilities for member {NationalCode}", nationalCode);
            return new List<string>();
        }
    }

    /// <summary>
    /// Gets member features
    /// </summary>
    /// <param name="nationalCode">The member's national code</param>
    /// <returns>List of member feature keys</returns>
    public async Task<IEnumerable<string>> GetMemberFeaturesAsync(string nationalCode)
    {
        try
        {
            _logger.LogDebug("Getting features for member {NationalCode}", nationalCode);
            var nationalId = new NationalId(nationalCode);
            return await _memberInfoService.GetMemberFeaturesAsync(nationalId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting features for member {NationalCode}", nationalCode);
            return new List<string>();
        }
    }

    /// <summary>
    /// Validates if a member can participate in a specific activity
    /// </summary>
    /// <param name="nationalCode">The member's national code</param>
    /// <param name="requiredCapabilities">Required capabilities for the activity</param>
    /// <param name="requiredFeatures">Required features for the activity</param>
    /// <returns>Validation result with member info if valid</returns>
    public async Task<MemberValidationResult> ValidateMemberForActivityAsync(
        string nationalCode, 
        IEnumerable<string>? requiredCapabilities = null, 
        IEnumerable<string>? requiredFeatures = null)
    {
        try
        {
            _logger.LogDebug("Validating member {NationalCode} for activity", nationalCode);

            var memberInfo = await GetMemberInfoAsync(nationalCode);
            if (memberInfo == null)
            {
                return MemberValidationResult.Failed($"Member with national code {nationalCode} not found");
            }

            if (!memberInfo.HasActiveMembership)
            {
                return MemberValidationResult.Failed($"Member {memberInfo.FullName} does not have an active membership");
            }

            // Check required capabilities
            if (requiredCapabilities != null && requiredCapabilities.Any())
            {
                var missingCapabilities = requiredCapabilities
                    .Where(cap => !memberInfo.Capabilities.Contains(cap))
                    .ToList();

                if (missingCapabilities.Any())
                {
                    return MemberValidationResult.Failed(
                        $"Member {memberInfo.FullName} is missing required capabilities: {string.Join(", ", missingCapabilities)}");
                }
            }

            // Check required features
            if (requiredFeatures != null && requiredFeatures.Any())
            {
                var missingFeatures = requiredFeatures
                    .Where(feature => !memberInfo.Features.Contains(feature))
                    .ToList();

                if (missingFeatures.Any())
                {
                    return MemberValidationResult.Failed(
                        $"Member {memberInfo.FullName} is missing required features: {string.Join(", ", missingFeatures)}");
                }
            }

            return MemberValidationResult.Success(memberInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating member {NationalCode} for activity", nationalCode);
            return MemberValidationResult.Failed($"Error validating member: {ex.Message}");
        }
    }
}

/// <summary>
/// Result of member validation
/// </summary>
public class MemberValidationResult
{
    public bool IsValid { get; private set; }
    public string? ErrorMessage { get; private set; }
    public MemberInfoDto? MemberInfo { get; private set; }

    private MemberValidationResult(bool isValid, string? errorMessage = null, MemberInfoDto? memberInfo = null)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
        MemberInfo = memberInfo;
    }

    public static MemberValidationResult Success(MemberInfoDto memberInfo)
    {
        return new MemberValidationResult(true, null, memberInfo);
    }

    public static MemberValidationResult Failed(string errorMessage)
    {
        return new MemberValidationResult(false, errorMessage);
    }
}
