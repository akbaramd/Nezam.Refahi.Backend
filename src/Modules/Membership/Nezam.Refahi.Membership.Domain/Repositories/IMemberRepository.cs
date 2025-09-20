using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Membership.Domain.Entities;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Membership.Domain.Repositories;

/// <summary>
/// Repository interface for Member entity operations
/// </summary>
public interface IMemberRepository : IRepository<Member, Guid>
{
    /// <summary>
    /// Gets a member by their national code
    /// </summary>
    /// <param name="nationalCode">The member's national code</param>
    /// <returns>The member if found, null otherwise</returns>
    Task<Member?> GetByNationalCodeAsync(NationalId nationalCode);
    
    /// <summary>
    /// Gets a member by their membership number
    /// </summary>
    /// <param name="membershipNumber">The membership number</param>
    /// <returns>The member if found, null otherwise</returns>
    Task<Member?> GetByMembershipNumberAsync(string membershipNumber);

    /// <summary>
    /// Gets a member by their phone number
    /// </summary>
    /// <param name="phoneNumber">The member's phone number</param>
    /// <returns>The member if found, null otherwise</returns>
    Task<Member?> GetByPhoneNumberAsync(PhoneNumber phoneNumber);

    /// <summary>
    /// Gets a member by their email address
    /// </summary>
    /// <param name="email">The member's email</param>
    /// <returns>The member if found, null otherwise</returns>
    Task<Member?> GetByEmailAsync(string email);

    /// <summary>
    /// Gets active members only
    /// </summary>
    /// <returns>Collection of active members</returns>
    Task<IEnumerable<Member>> GetActiveMembersAsync();

    /// <summary>
    /// Gets members by their full name (first name + last name)
    /// </summary>
    /// <param name="firstName">First name to search</param>
    /// <param name="lastName">Last name to search</param>
    /// <returns>Collection of members matching the name</returns>
    Task<IEnumerable<Member>> GetByNameAsync(string firstName, string lastName);

    /// <summary>
    /// Searches members by name (partial match)
    /// </summary>
    /// <param name="searchTerm">Search term for name</param>
    /// <returns>Collection of members matching the search term</returns>
    Task<IEnumerable<Member>> SearchByNameAsync(string searchTerm);



    /// <summary>
    /// Checks if a national code is already in use
    /// </summary>
    /// <param name="nationalCode">National code to check</param>
    /// <param name="excludeMemberId">Member ID to exclude from check (for updates)</param>
    /// <returns>True if national code exists, false otherwise</returns>
    Task<bool> IsNationalCodeExistsAsync(NationalId nationalCode, Guid? excludeMemberId = null);

    /// <summary>
    /// Checks if a membership number is already in use
    /// </summary>
    /// <param name="membershipNumber">Membership number to check</param>
    /// <param name="excludeMemberId">Member ID to exclude from check (for updates)</param>
    /// <returns>True if membership number exists, false otherwise</returns>
    Task<bool> IsMembershipNumberExistsAsync(string membershipNumber, Guid? excludeMemberId = null);

    /// <summary>
    /// Gets paginated list of members
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Paginated list of members</returns>
    Task<(IEnumerable<Member> Members, int TotalCount)> GetMembersPaginatedAsync(
        int pageNumber, 
        int pageSize);

    /// <summary>
    /// Gets members by their national codes
    /// </summary>
    /// <param name="nationalCodes">Collection of national codes</param>
    /// <returns>Collection of members matching the national codes</returns>
    Task<IEnumerable<Member>> GetByNationalCodesAsync(IEnumerable<NationalId> nationalCodes);

    /// <summary>
    /// Gets members by claim type through their capabilities
    /// </summary>
    /// <param name="claimTypeId">The claim type ID</param>
    /// <returns>Collection of members with the specified claim type through capabilities</returns>
    Task<IEnumerable<Member>> GetMembersByFeatureAsync(string featureId);

    /// <summary>
    /// Gets a member with their capabilities by ID
    /// </summary>
    /// <param name="id">Member ID</param>
    /// <returns>The member with capabilities if found, null otherwise</returns>
    Task<Member?> GetByIdWithCapabilitiesAsync(Guid id);

    /// <summary>
    /// Gets a member with their capabilities by national code
    /// </summary>
    /// <param name="nationalCode">The member's national code</param>
    /// <returns>The member with capabilities if found, null otherwise</returns>
    Task<Member?> GetByNationalCodeWithCapabilitiesAsync(NationalId nationalCode);

    /// <summary>
    /// Gets members by capability ID
    /// </summary>
    /// <param name="capabilityId">The capability ID</param>
    /// <returns>Collection of members with the specified capability</returns>
    Task<IEnumerable<Member>> GetMembersByCapabilityAsync(string capabilityId);

    /// <summary>
    /// Gets members without any capability assigned
    /// </summary>
    /// <returns>Collection of members without capabilities</returns>
    Task<IEnumerable<Member>> GetMembersWithoutCapabilitiesAsync();

    /// <summary>
    /// Gets members with capabilities that will expire within the specified days
    /// </summary>
    /// <param name="days">Number of days to look ahead</param>
    /// <returns>Collection of members with expiring capabilities</returns>
    Task<IEnumerable<Member>> GetMembersWithExpiringCapabilitiesAsync(int days);

    /// <summary>
    /// Gets members having all specified capabilities
    /// </summary>
    /// <param name="capabilityIds">List of capability IDs</param>
    /// <returns>Collection of members with all specified capabilities</returns>
    Task<IEnumerable<Member>> GetMembersWithAllCapabilitiesAsync(IEnumerable<string> capabilityIds);

    /// <summary>
    /// Gets members having any of the specified capabilities
    /// </summary>
    /// <param name="capabilityIds">List of capability IDs</param>
    /// <returns>Collection of members with any of the specified capabilities</returns>
    Task<IEnumerable<Member>> GetMembersWithAnyCapabilitiesAsync(IEnumerable<string> capabilityIds);
}