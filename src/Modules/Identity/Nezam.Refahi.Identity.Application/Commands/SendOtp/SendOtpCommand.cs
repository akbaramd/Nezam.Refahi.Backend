using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Commands.SendOtp;

/// <summary>
/// Command to send an OTP code to a user's phone number
/// </summary>
public record SendOtpCommand : IRequest<ApplicationResult<SendOtpResponse>>
{
    /// <summary>
    /// The phone number to send the OTP to
    /// </summary>
    public string NationalCode { get; init; } = string.Empty;
    
    /// <summary>
    /// The purpose of the OTP (e.g., "login", "reset-password")
    /// </summary>
    public string Purpose { get; init; } = "login";
    
    /// <summary>
    /// Device identifier for security tracking
    /// </summary>
    public string? DeviceId { get; init; }
    
    /// <summary>
    /// IP address for security tracking
    /// </summary>
    public string? IpAddress { get; init; }
}
