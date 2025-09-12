using Nezam.Refahi.Identity.Application.Services.Models;

namespace Nezam.Refahi.Identity.Application.Services.Contracts;

/// <summary>
/// Service for validating user data uniqueness and business rules
/// </summary>
public interface IUserValidationService
{
    /// <summary>
    /// Validates if a national ID is unique in the system
    /// </summary>
    /// <param name="nationalId">National ID to validate</param>
    /// <param name="excludeUserId">User ID to exclude from check (for updates)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result</returns>
    Task<UserValidationResult> ValidateNationalIdUniquenessAsync(
        string nationalId, 
        Guid? excludeUserId = null, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates if a phone number is unique in the system
    /// </summary>
    /// <param name="phoneNumber">Phone number to validate</param>
    /// <param name="excludeUserId">User ID to exclude from check (for updates)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result</returns>
    Task<UserValidationResult> ValidatePhoneNumberUniquenessAsync(
        string phoneNumber, 
        Guid? excludeUserId = null, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates Iranian national ID format and check digit
    /// </summary>
    /// <param name="nationalId">National ID to validate</param>
    /// <returns>Validation result</returns>
    UserValidationResult ValidateNationalIdFormat(string nationalId);

    /// <summary>
    /// Validates Iranian phone number format
    /// </summary>
    /// <param name="phoneNumber">Phone number to validate</param>
    /// <returns>Validation result</returns>
    UserValidationResult ValidatePhoneNumberFormat(string phoneNumber);

    /// <summary>
    /// Validates user data for creation
    /// </summary>
    /// <param name="firstName">First name</param>
    /// <param name="lastName">Last name</param>
    /// <param name="phoneNumber">Phone number</param>
    /// <param name="nationalId">National ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Comprehensive validation result</returns>
    Task<UserValidationResult> ValidateUserForCreationAsync(
        string firstName,
        string lastName,
        string phoneNumber,
        string nationalId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates user data for update
    /// </summary>
    /// <param name="userId">User ID being updated</param>
    /// <param name="firstName">First name</param>
    /// <param name="lastName">Last name</param>
    /// <param name="phoneNumber">Phone number</param>
    /// <param name="nationalId">National ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Comprehensive validation result</returns>
    Task<UserValidationResult> ValidateUserForUpdateAsync(
        Guid userId,
        string firstName,
        string lastName,
        string phoneNumber,
        string nationalId,
        CancellationToken cancellationToken = default);
}