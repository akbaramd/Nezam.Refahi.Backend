using Nezam.Refahi.Identity.Contracts.Dtos;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Identity.Contracts.Pool;

/// <summary>
/// Anti-Corruption Layer contract for integrating user data from external contexts
/// This interface defines what Identity context needs from external user sources
/// Implementation can be swapped (direct service, HTTP client, message broker) without affecting Identity domain
/// </summary>
public interface IUserIntegrationPool
{
    /// <summary>
    /// Gets external user information for account creation or validation
    /// Primary use case: During OTP send to fetch user data from external sources
    /// </summary>
    /// <param name="nationalCode">User's national code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>External user information or null if not found</returns>
    Task<ExternalUserInfo?> GetExternalUserInfoAsync(
      NationalId nationalCode, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates if a user exists in external systems and can create an account
    /// Primary use case: During user registration to verify eligibility
    /// </summary>
    /// <param name="nationalCode">User's national code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User validation information or null if not found</returns>
    Task<UserValidationInfo?> ValidateUserEligibilityAsync(
        NationalId nationalCode, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user has active status in external systems
    /// Primary use case: During authentication to verify user status
    /// </summary>
    /// <param name="nationalCode">User's national code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if user has active status</returns>
    Task<bool> HasActiveStatusAsync(
      NationalId nationalCode, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates user existence by different lookup methods
    /// Primary use case: Preventing duplicate accounts and user validation
    /// </summary>
    /// <param name="searchKey">Search value (national code, phone, email, etc.)</param>
    /// <param name="lookupType">Type of lookup to perform</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Lookup result with user information</returns>
    Task<UserLookupResult> ValidateUserExistenceAsync(
      NationalId searchKey, 
        UserLookupType lookupType, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Batch validation of multiple users for bulk operations
    /// Primary use case: Role assignments, bulk user operations
    /// </summary>
    /// <param name="nationalCodes">Collection of national codes to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of validation results</returns>
    Task<IEnumerable<UserValidationInfo>> BatchValidateUsersAsync(
        IEnumerable<NationalId> nationalCodes, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Health check for the external user integration
    /// Primary use case: Monitoring and ensuring external service availability
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if external service is available and responsive</returns>
    Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default);
}