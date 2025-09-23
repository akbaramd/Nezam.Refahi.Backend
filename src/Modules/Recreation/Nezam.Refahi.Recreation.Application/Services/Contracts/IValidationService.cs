namespace Nezam.Refahi.Recreation.Application.Services.Contracts;

/// <summary>
/// Service for validating various input formats and business rules
/// </summary>
public interface IValidationService
{
    /// <summary>
    /// Validates Iranian National ID using the official checksum algorithm
    /// </summary>
    /// <param name="nationalId">National ID to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    bool IsValidNationalId(string nationalId);

    /// <summary>
    /// Validates Iranian phone number format
    /// Supports formats: 09xxxxxxxxx, +989xxxxxxxxx, 989xxxxxxxxx
    /// </summary>
    /// <param name="phoneNumber">Phone number to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    bool IsValidPhoneNumber(string phoneNumber);

    /// <summary>
    /// Validates email address format
    /// </summary>
    /// <param name="email">Email address to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    bool IsValidEmail(string email);

    /// <summary>
    /// Calculates age based on birth date and reference date
    /// </summary>
    /// <param name="birthDate">Birth date</param>
    /// <param name="referenceDate">Reference date (usually current date or tour start date)</param>
    /// <returns>Age in years</returns>
    int CalculateAge(DateTime birthDate, DateTime referenceDate);

    /// <summary>
    /// Validates if birth date is reasonable (not in future, not too old)
    /// </summary>
    /// <param name="birthDate">Birth date to validate</param>
    /// <param name="maxAge">Maximum reasonable age (optional, uses configuration if not provided)</param>
    /// <returns>True if reasonable, false otherwise</returns>
    bool IsReasonableBirthDate(DateTime birthDate, int? maxAge = null);
}
