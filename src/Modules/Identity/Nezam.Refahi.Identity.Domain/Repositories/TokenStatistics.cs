namespace Nezam.Refahi.Identity.Domain.Repositories;

/// <summary>
/// Token statistics for monitoring and analytics
/// </summary>
public record TokenStatistics
{
  public int TotalActiveTokens { get; init; }
  public int ActiveRefreshTokens { get; init; }
  public int ActiveAccessTokens { get; init; }
  public int RevokedTokens { get; init; }
  public int ExpiredTokens { get; init; }
  public int IdleTokens { get; init; }
}