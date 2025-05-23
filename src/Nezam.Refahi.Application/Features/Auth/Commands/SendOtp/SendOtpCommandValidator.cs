using FluentValidation;
using System.Text.RegularExpressions;

namespace Nezam.Refahi.Application.Features.Auth.Commands.SendOtp
{
  /// <summary>
  /// Validator for the SendOtpCommand
  /// </summary>
  public class SendOtpCommandValidator : AbstractValidator<SendOtpCommand>
  {
    public SendOtpCommandValidator()
    {
      RuleFor(v => v.NationalCode)
        .NotEmpty().WithMessage("National code is required.")
        .Length(10).WithMessage("National code must be exactly 10 digits.")
        .Matches("^[0-9]{10}$").WithMessage("National code must consist of 10 numeric digits.");

      RuleFor(v => v.Purpose)
        .NotEmpty().WithMessage("Purpose is required.")
        .MaximumLength(50).WithMessage("Purpose must not exceed 50 characters.");
    }

    private bool BeValidIranianNationalCode(string nationalCode)
    {
      if (string.IsNullOrEmpty(nationalCode) || nationalCode.Length != 10)
        return false;

      // Reject codes with all digits the same (e.g., 0000000000, 1111111111)
      if (new string(nationalCode[0], 10) == nationalCode)
        return false;

      // Iranian national code checksum validation
      int sum = 0;
      for (int i = 0; i < 9; i++)
      {
        sum += (nationalCode[i] - '0') * (10 - i);
      }
      int remainder = sum % 11;
      int controlDigit = nationalCode[9] - '0';

      return (remainder < 2 && controlDigit == remainder) || (remainder >= 2 && controlDigit == 11 - remainder);
    }
  }
}
