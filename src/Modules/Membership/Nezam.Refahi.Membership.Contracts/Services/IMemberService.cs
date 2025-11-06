using Nezam.Refahi.Membership.Contracts.Dtos;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Membership.Contracts.Services;

/// <summary>
/// Service interface for accessing member data from other bounded contexts
/// This follows DDD principles by providing a clean service contract for cross-context communication
/// </summary>
public interface IMemberService
{
    /// <summary>
    /// Gets a member by their national code
    /// </summary>
    /// <param name="nationalCode">The member's national code</param>
    /// <returns>Member data if found, null otherwise</returns>
    Task<MemberDto?> GetMemberByNationalCodeAsync(NationalId nationalCode);

    /// <summary>
    /// Gets detailed member information by their national code (includes relational data)
    /// </summary>
    /// <param name="nationalCode">The member's national code</param>
    /// <returns>Detailed member data with capabilities, features, roles, and agencies if found, null otherwise</returns>
    Task<MemberDetailDto?> GetMemberDetailByNationalCodeAsync(NationalId nationalCode);

    /// <summary>
    /// Gets multiple members by their national codes
    /// </summary>
    /// <param name="nationalCodes">List of national codes</param>
    /// <returns>Collection of found members</returns>
    Task<IEnumerable<MemberDto>> GetMembersByNationalCodesAsync(IEnumerable<NationalId> nationalCodes);

    /// <summary>
    /// Gets a member by their membership number
    /// </summary>
    /// <param name="membershipNumber">The membership number</param>
    /// <returns>Member data if found, null otherwise</returns>
    Task<MemberDto?> GetMemberByMembershipNumberAsync(string membershipNumber);

    /// <summary>
    /// Gets a member by their phone number
    /// </summary>
    /// <param name="phoneNumber">The phone number</param>
    /// <returns>Member data if found, null otherwise</returns>
    Task<MemberDto?> GetMemberByPhoneNumberAsync(string phoneNumber);

    /// <summary>
    /// Gets a member by their email address
    /// </summary>
    /// <param name="email">The email address</param>
    /// <returns>Member data if found, null otherwise</returns>
    Task<MemberDto?> GetMemberByEmailAsync(string email);

    /// <summary>
    /// Gets a member by their external user ID
    /// </summary>
    /// <param name="externalId">The external user ID</param>
    /// <returns>Member data if found, null otherwise</returns>
    Task<MemberDto?> GetMemberByExternalIdAsync(string externalId);

    /// <summary>
    /// Checks if a member exists with the given national code
    /// </summary>
    /// <param name="nationalCode">The national code to check</param>
    /// <returns>True if member exists, false otherwise</returns>
    Task<bool> IsMemberExistsByNationalCodeAsync(NationalId nationalCode);

    /// <summary>
    /// Checks if a member has an active membership
    /// </summary>
    /// <param name="nationalCode">The member's national code</param>
    /// <returns>True if member has active membership, false otherwise</returns>
    Task<bool> HasActiveMembershipAsync(NationalId nationalCode);

    /// <summary>
    /// Gets member capabilities by national code
    /// </summary>
    /// <param name="nationalCode">The member's national code</param>
    /// <returns>List of member capability keys</returns>
    Task<IEnumerable<string>> GetMemberCapabilitiesAsync(NationalId nationalCode);

    /// <summary>
    /// Gets member features by national code
    /// </summary>
    /// <param name="nationalCode">The member's national code</param>
    /// <returns>List of member feature keys</returns>
    Task<IEnumerable<string>> GetMemberFeaturesAsync(NationalId nationalCode);

    /// <summary>
    /// Checks if a member has specific capability
    /// </summary>
    /// <param name="nationalCode">The member's national code</param>
    /// <param name="capabilityId">The capability ID to check</param>
    /// <returns>True if member has the capability, false otherwise</returns>
    Task<bool> HasCapabilityAsync(NationalId nationalCode, string capabilityId);

    /// <summary>
    /// Checks if a member has specific feature
    /// </summary>
    /// <param name="nationalCode">The member's national code</param>
    /// <param name="featureId">The feature ID to check</param>
    /// <returns>True if member has the feature, false otherwise</returns>
    Task<bool> HasFeatureAsync(NationalId nationalCode, string featureId);

    /// <summary>
    /// Validates eligibility of a single member against tour requirements
    /// </summary>
    /// <param name="nationalCode">The member's national code</param>
    /// <param name="requiredCapabilities">Required capability IDs (member must have at least one of them - OR logic with features)</param>
    /// <param name="requiredFeatures">Required feature IDs (member must have at least one of them - OR logic with capabilities)</param>
    /// <param name="requiredAgencies">Required agency IDs (member must be associated with at least one of them, checked separately, null means no agency requirement)</param>
    /// <returns>Eligibility result with detailed validation information</returns>
    /// <remarks>
    /// Validation logic:
    /// - Member must have at least ONE of the required capabilities OR at least ONE of the required features (OR logic)
    /// - Agencies are checked separately - member must have at least one of the required agencies (if any are specified)
    /// - Both conditions must pass for the member to be eligible
    /// </remarks>
    Task<MemberEligibilityResult> ValidateMemberEligibilityAsync(
        NationalId nationalCode,
        IEnumerable<string>? requiredCapabilities = null,
        IEnumerable<string>? requiredFeatures = null,
        IEnumerable<Guid>? requiredAgencies = null);

    /// <summary>
    /// Validates eligibility of multiple members against tour requirements
    /// </summary>
    /// <param name="nationalCodes">List of member national codes to validate</param>
    /// <param name="requiredCapabilities">Required capability IDs (member must have all of them)</param>
    /// <param name="requiredFeatures">Required feature IDs (member must have all of them)</param>
    /// <param name="requiredAgencies">Required agency IDs (member must be associated with at least one of them, null means no agency requirement)</param>
    /// <returns>Dictionary of eligibility results keyed by national code</returns>
    Task<Dictionary<string, MemberEligibilityResult>> ValidateMembersEligibilityAsync(
        IEnumerable<NationalId> nationalCodes,
        IEnumerable<string>? requiredCapabilities = null,
        IEnumerable<string>? requiredFeatures = null,
        IEnumerable<Guid>? requiredAgencies = null);

    /// <summary>
    /// Checks if a member is associated with any of the specified agencies
    /// </summary>
    /// <param name="nationalCode">The member's national code</param>
    /// <param name="agencyIds">List of agency IDs to check</param>
    /// <returns>True if member is associated with at least one of the agencies, false otherwise</returns>
    Task<bool> HasAnyAgencyAsync(NationalId nationalCode, IEnumerable<Guid> agencyIds);

    /// <summary>
    /// Checks if a member is VIP (has VIP capability or feature)
    /// </summary>
    /// <param name="nationalCode">The member's national code</param>
    /// <returns>True if member is VIP, false otherwise</returns>
    Task<bool> IsVipAsync(NationalId nationalCode);
}
