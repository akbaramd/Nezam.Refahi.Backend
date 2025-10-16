namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilities;

/// <summary>
/// Facility cycle statistics data transfer object
/// </summary>
public record FacilityCycleStatisticsDto
{
  /// <summary>
  /// Number of active cycles
  /// </summary>
  public int ActiveCyclesCount { get; init; }

  /// <summary>
  /// Total number of cycles
  /// </summary>
  public int TotalCyclesCount { get; init; }

  /// <summary>
  /// Number of draft cycles
  /// </summary>
  public int DraftCyclesCount { get; init; }

  /// <summary>
  /// Number of closed cycles
  /// </summary>
  public int ClosedCyclesCount { get; init; }

  /// <summary>
  /// Number of completed cycles
  /// </summary>
  public int CompletedCyclesCount { get; init; }

  /// <summary>
  /// Number of cancelled cycles
  /// </summary>
  public int CancelledCyclesCount { get; init; }

  /// <summary>
  /// Total active quota across all active cycles
  /// </summary>
  public int TotalActiveQuota { get; init; }

  /// <summary>
  /// Total used quota across all active cycles
  /// </summary>
  public int TotalUsedQuota { get; init; }

  /// <summary>
  /// Total available quota across all active cycles
  /// </summary>
  public int TotalAvailableQuota { get; init; }

  /// <summary>
  /// Overall quota utilization percentage
  /// </summary>
  public decimal QuotaUtilizationPercentage { get; init; }
}
