using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.ValueObjects;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Identity.Domain.Repositories;

/// <summary>
/// Repository interface for User entity operations with OTP authentication support
/// </summary>
public interface IUserRepository : IRepository<User, Guid>
{
    /// <summary>
    /// Gets a user by their national ID
    /// </summary>
    /// <param name="nationalId">The user's national ID</param>
    /// <returns>The user if found, null otherwise</returns>
    Task<User?> GetByNationalIdAsync(NationalId nationalId);
    
    /// <summary>
    /// Gets a user by their phone number
    /// </summary>
    /// <param name="phoneNumber">The user's phone number</param>
    /// <returns>The user if found, null otherwise</returns>
    Task<User?> GetByPhoneNumberAsync(string phoneNumber);

    /// <summary>
    /// Gets a user by their phone number using PhoneNumber value object
    /// </summary>
    /// <param name="phoneNumber">The user's phone number</param>
    /// <returns>The user if found, null otherwise</returns>
    Task<User?> GetByPhoneNumberValueObjectAsync(PhoneNumber phoneNumber);

    /// <summary>
    /// Gets all users with verified phone numbers
    /// </summary>
    /// <returns>Collection of verified users</returns>
    Task<IEnumerable<User>> GetVerifiedUsersAsync();

    /// <summary>
    /// Gets all users with unverified phone numbers
    /// </summary>
    /// <returns>Collection of unverified users</returns>
    Task<IEnumerable<User>> GetUnverifiedUsersAsync();

    /// <summary>
    /// Gets all active users
    /// </summary>
    /// <returns>Collection of active users</returns>
    Task<IEnumerable<User>> GetActiveUsersAsync();

    /// <summary>
    /// Gets all locked users
    /// </summary>
    /// <returns>Collection of locked users</returns>
    Task<IEnumerable<User>> GetLockedUsersAsync();


    /// <summary>
    /// Gets users by IP address
    /// </summary>
    /// <param name="ipAddress">IP address to filter by</param>
    /// <returns>Collection of users from the specified IP</returns>
    Task<IEnumerable<User>> GetByIpAddressAsync(string ipAddress);

    /// <summary>
    /// Gets users that haven't been authenticated recently
    /// </summary>
    /// <param name="beforeDate">Date before which users are considered inactive</param>
    /// <returns>Collection of inactive users</returns>
    Task<IEnumerable<User>> GetInactiveUsersAsync(DateTime beforeDate);

    /// <summary>
    /// Checks if a phone number is already registered
    /// </summary>
    /// <param name="phoneNumber">Phone number to check</param>
    /// <returns>True if phone number is registered, false otherwise</returns>
    Task<bool> IsPhoneNumberRegisteredAsync(PhoneNumber phoneNumber);

    /// <summary>
    /// Gets the count of verified users
    /// </summary>
    /// <returns>Number of verified users</returns>
    Task<int> GetVerifiedUserCountAsync();

    /// <summary>
    /// Gets the count of active users
    /// </summary>
    /// <returns>Number of active users</returns>
    Task<int> GetActiveUserCountAsync();

    /// <summary>
    /// Gets the count of locked users
    /// </summary>
    /// <returns>Number of locked users</returns>
    Task<int> GetLockedUserCountAsync();

    /// <summary>
    /// Gets users that need to be unlocked (lock duration expired)
    /// </summary>
    /// <returns>Collection of users that can be unlocked</returns>
    Task<IEnumerable<User>> GetUsersToUnlockAsync();

    /// <summary>
    /// Gets users by authentication date range
    /// </summary>
    /// <param name="fromDate">Start date for the range</param>
    /// <param name="toDate">End date for the range</param>
    /// <returns>Collection of users authenticated in the specified range</returns>
    Task<IEnumerable<User>> GetByAuthenticationDateRangeAsync(DateTime fromDate, DateTime toDate);

    /// <summary>
    /// Gets users by phone verification date range
    /// </summary>
    /// <param name="fromDate">Start date for the range</param>
    /// <param name="toDate">End date for the range</param>
    /// <returns>Collection of users verified in the specified range</returns>
    Task<IEnumerable<User>> GetByPhoneVerificationDateRangeAsync(DateTime fromDate, DateTime toDate);

    /// <summary>
    /// Gets a user by external user id from source systems
    /// </summary>
    Task<User?> GetByExternalIdAsync(Guid externalUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by email address
    /// </summary>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
