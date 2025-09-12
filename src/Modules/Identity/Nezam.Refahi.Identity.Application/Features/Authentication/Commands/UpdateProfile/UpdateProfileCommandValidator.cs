using System.Text.RegularExpressions;
using FluentValidation;

namespace Nezam.Refahi.Identity.Application.Features.Authentication.Commands.UpdateProfile;

/// <summary>
/// Validator for the UpdateProfileCommand
/// </summary>
public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.")
            .Matches(new Regex(@"^[\u0600-\u06FFa-zA-Z\s]+$")).WithMessage("First name can only contain letters and spaces.");
            
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.")
            .Matches(new Regex(@"^[\u0600-\u06FFa-zA-Z\s]+$")).WithMessage("Last name can only contain letters and spaces.");
            
        RuleFor(x => x.NationalId)
            .NotEmpty().WithMessage("National ID is required.")
            .Length(10).WithMessage("National ID must be exactly 10 digits.")
            .Matches(new Regex("^[0-9]{10}$")).WithMessage("National ID must contain only digits.")
            .Must(BeValidNationalId).WithMessage("National ID is not valid.");

        RuleFor(x => x.PhoneNumber)
            .Must(BeValidPhoneNumber)
            .WithMessage("Phone number must be a valid Iranian mobile number (11 digits starting with 09).")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
            
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserDetail ID is required.");
    }

    /// <summary>
    /// Validates Iranian national ID using checksum algorithm
    /// </summary>
    private static bool BeValidNationalId(string nationalId)
    {
        if (string.IsNullOrEmpty(nationalId) || nationalId.Length != 10)
            return false;

        // Check if all digits are the same (invalid national IDs)
        if (nationalId.All(c => c == nationalId[0]))
            return false;

        // Calculate checksum
        var sum = 0;
        for (var i = 0; i < 9; i++)
        {
            sum += int.Parse(nationalId[i].ToString()) * (10 - i);
        }

        var remainder = sum % 11;
        var checkDigit = remainder < 2 ? remainder : 11 - remainder;
        
        return checkDigit == int.Parse(nationalId[9].ToString());
    }

    /// <summary>
    /// Validates Iranian mobile phone number
    /// </summary>
    private static bool BeValidPhoneNumber(string? phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber))
            return true; // Optional field

        // Iranian mobile number: 11 digits starting with 09
        var phoneRegex = new Regex(@"^09\d{9}$");
        return phoneRegex.IsMatch(phoneNumber);
    }
}