using Microsoft.Extensions.Options;
using Nezam.Refahi.Recreation.Application.Configuration;
using Nezam.Refahi.Recreation.Application.Services.Contracts;

namespace Nezam.Refahi.Recreation.Application.Services;

/// <summary>
/// Service for validating various input formats and business rules
/// </summary>
public class ValidationService : IValidationService
{
    private readonly ReservationSettings _settings;

    public ValidationService(IOptions<ReservationSettings> settings)
    {
        _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
    }

    /// <summary>
    /// Validates Iranian National ID using the official checksum algorithm
    /// </summary>
    public bool IsValidNationalId(string nationalId)
    {
        if (string.IsNullOrWhiteSpace(nationalId))
            return false;

        // Remove any spaces or special characters
        nationalId = nationalId.Trim().Replace("-", "").Replace(" ", "");

        // Must be exactly 10 digits
        if (nationalId.Length != 10 || !nationalId.All(char.IsDigit))
            return false;

        // Check for invalid repeated digits (like 0000000000, 1111111111, etc.)
        if (nationalId.All(c => c == nationalId[0]))
            return false;

        // Iranian National ID checksum algorithm
        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            sum += (nationalId[i] - '0') * (10 - i);
        }

        int remainder = sum % 11;
        int checkDigit = remainder < 2 ? remainder : 11 - remainder;

        return checkDigit == (nationalId[9] - '0');
    }

    /// <summary>
    /// Validates Iranian phone number format
    /// </summary>
    public bool IsValidPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        // Remove spaces, dashes, and parentheses
        phoneNumber = phoneNumber.Trim()
            .Replace(" ", "")
            .Replace("-", "")
            .Replace("(", "")
            .Replace(")", "");

        // Support formats: 09xxxxxxxxx, +989xxxxxxxxx, 989xxxxxxxxx
        if (phoneNumber.StartsWith("+98"))
            phoneNumber = phoneNumber.Substring(3);
        else if (phoneNumber.StartsWith("98"))
            phoneNumber = phoneNumber.Substring(2);

        // Must be 11 digits starting with 09
        return phoneNumber.Length == 11 && 
               phoneNumber.StartsWith("09") && 
               phoneNumber.All(char.IsDigit);
    }

    /// <summary>
    /// Validates email format
    /// </summary>
    public bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email.Trim();
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Calculates age based on birth date and reference date
    /// </summary>
    public int CalculateAge(DateTime birthDate, DateTime referenceDate)
    {
        var age = referenceDate.Year - birthDate.Year;
        if (referenceDate.Date < birthDate.AddYears(age))
            age--;
        return age;
    }

    /// <summary>
    /// Validates if birth date is reasonable (not in future, not too old)
    /// </summary>
    public bool IsReasonableBirthDate(DateTime birthDate, int? maxAge = null)
    {
        // Check if birth date is in the future
        if (birthDate > DateTime.Now.Date)
            return false;

        // Calculate age and check if it's reasonable
        var age = CalculateAge(birthDate, DateTime.Now);
        var maxReasonableAge = maxAge ?? _settings.MaxReasonableAge;
        
        return age <= maxReasonableAge;
    }
}
