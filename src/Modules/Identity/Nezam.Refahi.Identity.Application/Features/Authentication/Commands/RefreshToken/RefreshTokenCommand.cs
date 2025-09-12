// -----------------------------------------------------------------------------
// RefreshTokenCommand.cs - Refresh token command for token rotation
// -----------------------------------------------------------------------------

using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Features.Authentication.Commands.RefreshToken;

/// <summary>
/// Command to refresh access token using refresh token
/// Implements token rotation for enhanced security
/// </summary>
public record RefreshTokenCommand : IRequest<ApplicationResult<RefreshTokenResponse>>
{
    /// <summary>
    /// The refresh token to use for generating new tokens
    /// </summary>
    public string RefreshToken { get; init; } = string.Empty;
    
    /// <summary>
    /// Device fingerprint for token binding validation
    /// </summary>
    public string? DeviceFingerprint { get; init; }
    
    /// <summary>
    /// IP address for token binding validation
    /// </summary>
    public string? IpAddress { get; init; }
    
    /// <summary>
    /// UserDetail agent for token binding validation
    /// </summary>
    public string? UserAgent { get; init; }
}