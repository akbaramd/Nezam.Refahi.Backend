using FluentValidation;
using System.Text.RegularExpressions;

namespace Nezam.Refahi.Application.Features.Auth.Commands.VerifyOtp;

/// <summary>
/// Validator for the VerifyOtpCommand
/// </summary>
public class VerifyOtpCommandValidator : AbstractValidator<VerifyOtpCommand>
{
  public VerifyOtpCommandValidator()
  {
    RuleFor(v => v.NationalCode)
      .NotEmpty().WithMessage("National code is required.")
      .Length(10).WithMessage("National code must be exactly 10 digits.")
      .Matches("^[0-9]{10}$").WithMessage("National code must consist of 10 numeric digits.")
      .Must(BeValidIranianNationalCode).WithMessage("National code is invalid.");

    RuleFor(v => v.OtpCode)
      .NotEmpty().WithMessage("OTP code is required.")
      .Length(6).WithMessage("OTP code must be 6 digits.")
      .Matches("^[0-9]{6}$").WithMessage("OTP code must contain only digits.");

    RuleFor(v => v.Purpose)
      .NotEmpty().WithMessage("Purpose is required.")
      .MaximumLength(50).WithMessage("Purpose must not exceed 50 characters.");
  }

  private bool BeValidIranianNationalCode(string code)
  {
    if (string.IsNullOrEmpty(code) || code.Length != 10)
      return false;
    if (new string(code[0], 10) == code)
      return false;
    int sum = 0;
    for (int i = 0; i < 9; i++)
      sum += (code[i] - '0') * (10 - i);
    int remainder = sum % 11;
    int control = code[9] - '0';
    return (remainder < 2 && control == remainder) ||
           (remainder >= 2 && control == 11 - remainder);
  }
}
