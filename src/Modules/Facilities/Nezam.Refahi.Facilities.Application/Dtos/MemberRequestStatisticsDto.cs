namespace Nezam.Refahi.Facilities.Application.Dtos;

/// <summary>
/// Member request statistics data transfer object
/// </summary>
public record MemberRequestStatisticsDto
{
  /// <summary>
  /// Total number of requests
  /// </summary>
  public int TotalRequests { get; init; }

  /// <summary>
  /// Number of active requests
  /// </summary>
  public int ActiveRequests { get; init; }

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

  /// <summary>
  /// Days since last request
  /// </summary>
  public int? DaysSinceLastRequest { get; init; }
}