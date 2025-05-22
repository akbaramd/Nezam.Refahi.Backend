using FluentValidation;
using System.Text.RegularExpressions;

namespace Nezam.Refahi.Application.Features.Auth.Commands.SendOtp;

/// <summary>
/// Validator for the SendOtpCommand
/// </summary>
public class SendOtpCommandValidator : AbstractValidator<SendOtpCommand>
{
    public SendOtpCommandValidator()
    {
        RuleFor(v => v.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Must(BeValidIranianPhoneNumber).WithMessage("Phone number must be a valid Iranian phone number (e.g., 09123456789).");
            
        RuleFor(v => v.Purpose)
            .NotEmpty().WithMessage("Purpose is required.")
            .MaximumLength(50).WithMessage("Purpose must not exceed 50 characters.");
    }
    
    private bool BeValidIranianPhoneNumber(string phoneNumber)
    {
        // Iranian phone numbers start with 09 and are 11 digits long
        return Regex.IsMatch(phoneNumber, @"^09\d{9}$");
    }
}
