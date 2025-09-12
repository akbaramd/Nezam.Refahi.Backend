using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Features.Authentication.Commands.VerifyOtp;

/// <summary>
/// Command to verify an OTP code
/// </summary>
public record VerifyOtpCommand : IRequest<ApplicationResult<VerifyOtpResponse>>
{
    /// <summary>
    /// The phone number that received the OTP
    /// </summary>
    public Guid ChallengeId { get; init; } 
    
    /// <summary>
    /// The OTP code to verify
    /// </summary>
    public string OtpCode { get; init; } = string.Empty;
    
    /// <summary>
    /// The purpose of the OTP (e.g., "login", "reset-password")
    /// </summary>
    public string Purpose { get; init; } = "login";

    public string? IpAddress { get; set; }
    public string? DeviceId { get; set; }
    public string? UserAgent { get; set; }
    
    /// <summary>
    /// The requested scope (panel or app)
    /// </summary>
    public string Scope { get; set; } = "app";
}
