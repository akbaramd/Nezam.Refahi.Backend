namespace Nezam.Refahi.Facilities.Application.Dtos;

/// <summary>
/// Facility cycle statistics data transfer object
/// </summary>
public record FacilityCycleStatisticsDto
{
  /// <summary>
  /// Number of active cycles
  /// </summary>
  public int ActiveCyclesCount { get; set; }

  /// <summary>
  /// Total number of cycles
  /// </summary>
  public int TotalCyclesCount { get; set; }

  /// <summary>
  /// Number of draft cycles
  /// </summary>
  public int DraftCyclesCount { get; set; }

  /// <summary>
  /// Number of closed cycles
  /// </summary>
  public int ClosedCyclesCount { get; set; }

  /// <summary>
  /// Number of cycles under review
  /// </summary>
  public int UnderReviewCyclesCount { get; set; }

  /// <summary>
  /// Number of completed cycles
  /// </summary>
  public int CompletedCyclesCount { get; set; }

  /// <summary>
  /// Number of cancelled cycles
  /// </summary>
  public int CancelledCyclesCount { get; set; }

  /// <summary>
  /// Total active quota across all active cycles
  /// </summary>
  public int TotalActiveQuota { get; set; }

  /// <summary>
  /// Total used quota across all active cycles
  /// </summary>
  public int TotalUsedQuota { get; set; }

  /// <summary>
  /// Total available quota across all active cycles
  /// </summary>
  public int TotalAvailableQuota { get; set; }

  /// <summary>
  /// Overall quota utilization percentage
  /// </summary>
  public decimal QuotaUtilizationPercentage { get; set; }
}
