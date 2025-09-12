namespace Nezam.Refahi.Identity.Application.Services;

/// <summary>
/// Statistics from cleanup operations
/// </summary>
public record CleanupStats
{
  public int ExpiredChallenges { get; init; }
  public int ConsumedChallenges { get; init; }
  public int OldChallenges { get; init; }
  public int TotalCleaned { get; init; }
  public TimeSpan Duration { get; init; }
}