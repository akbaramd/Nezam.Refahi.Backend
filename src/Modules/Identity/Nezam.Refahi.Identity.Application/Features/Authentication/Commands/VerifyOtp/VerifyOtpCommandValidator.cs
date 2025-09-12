using FluentValidation;

namespace Nezam.Refahi.Identity.Application.Features.Authentication.Commands.VerifyOtp;

/// <summary>
/// Validator for VerifyOtpCommand
/// </summary>
public class VerifyOtpCommandValidator : AbstractValidator<VerifyOtpCommand>
{
    public VerifyOtpCommandValidator()
    {
      RuleFor(x => x.ChallengeId)
        .NotEmpty()
        .WithMessage("ChallengeId code is required");

        RuleFor(x => x.OtpCode)
            .NotEmpty()
            .WithMessage("OTP code is required")
            .Length(6)
            .WithMessage("OTP code must be exactly 6 digits")
            .Matches(@"^\d{6}$")
            .WithMessage("OTP code must contain only digits");

        RuleFor(x => x.Purpose)
            .NotEmpty()
            .WithMessage("Purpose is required")
            .MaximumLength(50)
            .WithMessage("Purpose cannot exceed 50 characters");

        RuleFor(x => x.DeviceId)
            .MaximumLength(100)
            .WithMessage("Device ID cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.DeviceId));

        RuleFor(x => x.IpAddress)
            .Must(BeValidIpAddress)
            .WithMessage("IP address must be a valid IPv4 or IPv6 address")
            .When(x => !string.IsNullOrEmpty(x.IpAddress));
    }

    private static bool BeValidIpAddress(string? ipAddress)
    {
        if (string.IsNullOrEmpty(ipAddress))
            return true;

        return System.Net.IPAddress.TryParse(ipAddress, out _);
    }
}