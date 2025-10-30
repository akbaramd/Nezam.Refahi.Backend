namespace Nezam.Refahi.Facilities.Application.Dtos;

/// <summary>
/// User request statistics data transfer object
/// </summary>
public record UserRequestStatisticsDto
{
  /// <summary>
  /// Total number of requests
  /// </summary>
  public int TotalRequests { get; init; }

  /// <summary>
  /// Number of in-progress requests
  /// </summary>
  public int InProgressRequests { get; init; }

  /// <summary>
  /// Number of completed requests
  /// </summary>
  public int CompletedRequests { get; init; }

  /// <summary>
  /// Number of rejected requests
  /// </summary>
  public int RejectedRequests { get; init; }

  /// <summary>
  /// Number of cancelled requests
  /// </summary>
  public int CancelledRequests { get; init; }

  /// <summary>
  /// Number of terminal requests
  /// </summary>
  public int TerminalRequests { get; init; }

  /// <summary>
  /// Success rate percentage
  /// </summary>
  public decimal SuccessRatePercentage { get; init; }

  /// <summary>
  /// Average processing time in days
  /// </summary>
  public decimal? AverageProcessingTimeDays { get; init; }

  /// <summary>
  /// Total amount requested across all requests
  /// </summary>
  public decimal TotalRequestedAmountRials { get; init; }

  /// <summary>
  /// Total amount approved across all requests
  /// </summary>
  public decimal TotalApprovedAmountRials { get; init; }

  /// <summary>
  /// Formatted total requested amount
  /// </summary>
  public string FormattedTotalRequestedAmount { get; init; } = null!;

  /// <summary>
  /// Formatted total approved amount
  /// </summary>
  public string FormattedTotalApprovedAmount { get; init; } = null!;

  /// <summary>
  /// Number of different facilities requested
  /// </summary>
  public int DifferentFacilitiesCount { get; init; }

  /// <summary>
  /// Number of different cycles requested
  /// </summary>
  public int DifferentCyclesCount { get; init; }

  /// <summary>
  /// Most recent request date
  /// </summary>
  public DateTime? MostRecentRequestDate { get; init; }

  /// <summary>
  /// Oldest request date
  /// </summary>
  public DateTime? OldestRequestDate { get; init; }
}