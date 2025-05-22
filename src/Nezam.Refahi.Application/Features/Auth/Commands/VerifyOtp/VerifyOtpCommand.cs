using MediatR;
using Nezam.Refahi.Application.Common.Models;

namespace Nezam.Refahi.Application.Features.Auth.Commands.VerifyOtp;

/// <summary>
/// Command to verify an OTP code
/// </summary>
public record VerifyOtpCommand : IRequest<ApplicationResult<VerifyOtpResponse>>
{
    /// <summary>
    /// The phone number that received the OTP
    /// </summary>
    public string PhoneNumber { get; init; } = string.Empty;
    
    /// <summary>
    /// The OTP code to verify
    /// </summary>
    public string OtpCode { get; init; } = string.Empty;
    
    /// <summary>
    /// The purpose of the OTP (e.g., "login", "reset-password")
    /// </summary>
    public string Purpose { get; init; } = "login";
}
