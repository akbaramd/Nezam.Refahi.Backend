// -----------------------------------------------------------------------------
// RefreshTokenCommand.cs - Refresh token command for token rotation
// -----------------------------------------------------------------------------

using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Commands.RefreshToken;

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
    /// User agent for token binding validation
    /// </summary>
    public string? UserAgent { get; init; }
}

/// <summary>
/// Response containing new access and refresh tokens
/// </summary>
public record RefreshTokenResponse
{
    /// <summary>
    /// New access token (JWT)
    /// </summary>
    public string AccessToken { get; init; } = string.Empty;
    
    /// <summary>
    /// New refresh token (for next refresh)
    /// </summary>
    public string RefreshToken { get; init; } = string.Empty;
    
    /// <summary>
    /// Access token expiry in minutes
    /// </summary>
    public int ExpiryMinutes { get; init; }
    
    /// <summary>
    /// User ID
    /// </summary>
    public Guid UserId { get; init; }
    
    /// <summary>
    /// Whether the session was compromised (reuse detected)
    /// </summary>
    public bool IsSessionCompromised { get; init; }
}
