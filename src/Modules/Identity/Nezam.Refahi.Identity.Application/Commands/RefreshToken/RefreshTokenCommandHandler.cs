// -----------------------------------------------------------------------------
// RefreshTokenCommandHandler.cs - Production-grade refresh token handler
// -----------------------------------------------------------------------------

using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Commands.RefreshToken;

/// <summary>
/// Production-grade refresh token handler implementing token rotation
/// Follows DDD + EF Core best practices for security
/// </summary>
public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ApplicationResult<RefreshTokenResponse>>
{
    private readonly ITokenService _tokenService;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(
        ITokenService tokenService,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult<RefreshTokenResponse>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // ========================================================================
            // 1) Validate and Rotate Refresh Token
            // ========================================================================
            
            var result = await _tokenService.ValidateAndRotateRefreshTokenAsync(
                request.RefreshToken,
                request.DeviceFingerprint,
                request.IpAddress,
                request.UserAgent);

            if (!result.IsValid)
            {
                _logger.LogWarning("Refresh token validation failed: {ErrorMessage}", result.ErrorMessage);
                
                return ApplicationResult<RefreshTokenResponse>.Failure(
                    result.ErrorMessage ?? "Invalid refresh token");
            }

            // ========================================================================
            // 2) Check for Security Issues
            // ========================================================================
            
            if (result.IsReuseDetected || result.IsSessionCompromised)
            {
                _logger.LogWarning("Refresh token reuse detected for user {UserId} - session compromised", 
                    result.UserId);
                
                return ApplicationResult<RefreshTokenResponse>.Failure(
                    "Security violation detected. Please log in again.");
            }

            // ========================================================================
            // 3) Build Response
            // ========================================================================
            
            var response = new RefreshTokenResponse
            {
                AccessToken = result.NewAccessToken!,
                RefreshToken = result.NewRefreshToken!,
                ExpiryMinutes = 15, // Access token expiry
                UserId = result.UserId!.Value,
                IsSessionCompromised = result.IsSessionCompromised
            };

            _logger.LogInformation("Successfully refreshed tokens for user {UserId}", result.UserId);

            return ApplicationResult<RefreshTokenResponse>.Success(
                response, 
                "Tokens refreshed successfully");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing tokens");
            return ApplicationResult<RefreshTokenResponse>.Failure(
                "Failed to refresh tokens. Please try again.");
        }
    }
}
