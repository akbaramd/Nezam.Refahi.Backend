namespace Nezam.Refahi.Identity.Application.Features.Authentication.Commands.RefreshToken;

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
  /// UserDetail ID
  /// </summary>
  public Guid UserId { get; init; }
    
  /// <summary>
  /// Whether the session was compromised (reuse detected)
  /// </summary>
  public bool IsSessionCompromised { get; init; }
}