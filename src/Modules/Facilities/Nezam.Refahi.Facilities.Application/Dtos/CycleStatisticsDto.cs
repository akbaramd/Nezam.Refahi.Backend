namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails;

/// <summary>
/// Cycle statistics data transfer object
/// </summary>
public record CycleStatisticsDto
{
  /// <summary>
  /// Total quota
  /// </summary>
  public int TotalQuota { get; init; }

  /// <summary>
  /// Used quota
  /// </summary>
  public int UsedQuota { get; init; }

  /// <summary>
  /// Available quota
  /// </summary>
  public int AvailableQuota { get; init; }

  /// <summary>
  /// Quota utilization percentage
  /// </summary>
  public decimal UtilizationPercentage { get; init; }

  /// <summary>
  /// Number of pending requests
  /// </summary>
  public int PendingRequests { get; init; }

  /// <summary>
  /// Number of approved requests
  /// </summary>
  public int ApprovedRequests { get; init; }

  /// <summary>
  /// Number of rejected requests
  /// </summary>
  public int RejectedRequests { get; init; }

  /// <summary>
  /// Average processing time in days
  /// </summary>
  public decimal? AverageProcessingTimeDays { get; init; }

  /// <summary>
  /// Cycle duration in days
  /// </summary>
  public int CycleDurationDays { get; init; }

  /// <summary>
  /// Days elapsed since cycle start
  /// </summary>
  public int DaysElapsed { get; init; }

  /// <summary>
  /// Days remaining until cycle end
  /// </summary>
  public int DaysRemaining { get; init; }

  /// <summary>
  /// Cycle progress percentage
  /// </summary>
  public decimal CycleProgressPercentage { get; init; }
}