using Nezam.Refahi.Membership.Contracts.Dtos;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Membership.Contracts.Services;

/// <summary>
/// Service interface for accessing member information from other bounded contexts
/// This service provides member info with capabilities and features for cross-context communication
/// </summary>
public interface IMemberInfoService
{
    /// <summary>
    /// Gets member information including capabilities and features by national code
    /// </summary>
    /// <param name="nationalCode">The member's national code</param>
    /// <returns>Member information with capabilities and features if found, null otherwise</returns>
    Task<MemberInfoDto?> GetMemberInfoAsync(NationalId nationalCode);

    /// <summary>
    /// Gets multiple members information including capabilities and features by national codes
    /// </summary>
    /// <param name="nationalCodes">List of national codes</param>
    /// <returns>Collection of member information with capabilities and features</returns>
    Task<IEnumerable<MemberInfoDto>> GetMembersInfoAsync(IEnumerable<NationalId> nationalCodes);

    /// <summary>
    /// Gets member information by membership number
    /// </summary>
    /// <param name="membershipNumber">The membership number</param>
    /// <returns>Member information with capabilities and features if found, null otherwise</returns>
    Task<MemberInfoDto?> GetMemberInfoByMembershipNumberAsync(string membershipNumber);

    /// <summary>
    /// Gets member information by phone number
    /// </summary>
    /// <param name="phoneNumber">The phone number</param>
    /// <returns>Member information with capabilities and features if found, null otherwise</returns>
    Task<MemberInfoDto?> GetMemberInfoByPhoneNumberAsync(string phoneNumber);

    /// <summary>
    /// Gets member information by email address
    /// </summary>
    /// <param name="email">The email address</param>
    /// <returns>Member information with capabilities and features if found, null otherwise</returns>
    Task<MemberInfoDto?> GetMemberInfoByEmailAsync(string email);

    /// <summary>
    /// Gets member information by external ID
    /// </summary>
    /// <param name="externalId">The external ID</param>
    /// <returns>Member information with capabilities and features if found, null otherwise</returns>
    Task<MemberInfoDto?> GetMemberInfoByExternalIdAsync(string externalId);

    /// <summary>
    /// Checks if a member has a specific capability
    /// </summary>
    /// <param name="nationalCode">The member's national code</param>
    /// <param name="capabilityKey">The capability key to check</param>
    /// <returns>True if member has the capability, false otherwise</returns>
    Task<bool> HasCapabilityAsync(NationalId nationalCode, string capabilityKey);

    /// <summary>
    /// Checks if a member has a specific feature
    /// </summary>
    /// <param name="nationalCode">The member's national code</param>
    /// <param name="featureKey">The feature key to check</param>
    /// <returns>True if member has the feature, false otherwise</returns>
    Task<bool> HasFeatureAsync(NationalId nationalCode, string featureKey);

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
    /// Checks if a member exists with the given national code
    /// </summary>
    /// <param name="nationalCode">The national code to check</param>
    /// <returns>True if member exists, false otherwise</returns>
    Task<bool> IsMemberExistsAsync(NationalId nationalCode);

    /// <summary>
    /// Checks if a member has an active membership
    /// </summary>
    /// <param name="nationalCode">The member's national code</param>
    /// <returns>True if member has active membership, false otherwise</returns>
    Task<bool> HasActiveMembershipAsync(NationalId nationalCode);
}
