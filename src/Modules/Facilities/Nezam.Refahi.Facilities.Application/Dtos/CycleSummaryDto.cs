namespace Nezam.Refahi.Facilities.Application.Dtos;

/// <summary>
/// Cycle summary statistics data transfer object
/// </summary>
public record CycleSummaryDto
{
  /// <summary>
  /// Total number of cycles
  /// </summary>
  public int TotalCycles { get; init; }

  /// <summary>
  /// Number of active cycles
  /// </summary>
  public int ActiveCycles { get; init; }

  /// <summary>
  /// Number of draft cycles
  /// </summary>
  public int DraftCycles { get; init; }

  /// <summary>
  /// Number of closed cycles
  /// </summary>
  public int ClosedCycles { get; init; }

  /// <summary>
  /// Number of completed cycles
  /// </summary>
  public int CompletedCycles { get; init; }

  /// <summary>
  /// Number of cancelled cycles
  /// </summary>
  public int CancelledCycles { get; init; }

  /// <summary>
  /// Total quota across all cycles
  /// </summary>
  public int TotalQuota { get; init; }

  /// <summary>
  /// Total used quota across all cycles
  /// </summary>
  public int TotalUsedQuota { get; init; }

  /// <summary>
  /// Total available quota across all cycles
  /// </summary>
  public int TotalAvailableQuota { get; init; }

  /// <summary>
  /// Overall quota utilization percentage
  /// </summary>
  public decimal OverallQuotaUtilizationPercentage { get; init; }

  /// <summary>
  /// Number of cycles user is eligible for
  /// </summary>
  public int EligibleCycles { get; init; }

  /// <summary>
  /// Number of cycles user has requested
  /// </summary>
  public int RequestedCycles { get; init; }
}