namespace Nezam.Refahi.Identity.Application.Services;

/// <summary>
/// Result of refresh token validation and rotation
/// </summary>
public record RefreshTokenValidationResult
{
  public bool IsValid { get; init; }
  public string? ErrorMessage { get; init; }
  public Guid? UserId { get; init; }
  public string? NewAccessToken { get; init; }
  public string? NewRefreshToken { get; init; }
  public string? NewJwtId { get; init; }
  public bool IsReuseDetected { get; init; }
  public bool IsSessionCompromised { get; init; }
}