using Nezam.Refahi.Membership.Contracts.Dtos;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Membership.Contracts.Services;

/// <summary>
/// Service interface for accessing membership data from other bounded contexts
/// This follows DDD principles by providing a clean service contract
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
    /// Gets basic member information (limited fields for cross-context access)
    /// </summary>
    /// <param name="nationalCode">The member's national code</param>
    /// <returns>Basic member information</returns>
    Task<BasicMemberInfoDto?> GetBasicMemberInfoAsync(NationalId nationalCode);
}