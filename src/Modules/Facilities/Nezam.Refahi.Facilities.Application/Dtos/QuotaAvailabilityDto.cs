namespace Nezam.Refahi.Facilities.Application.Dtos;

/// <summary>
/// Quota availability analysis data transfer object
/// </summary>
public record QuotaAvailabilityDto
{
  /// <summary>
  /// Indicates if quota is available
  /// </summary>
  public bool IsAvailable { get; init; }

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
  /// Indicates if quota is nearly full (over 90%)
  /// </summary>
  public bool IsNearlyFull => UtilizationPercentage >= 90;

  /// <summary>
  /// Indicates if quota is completely full
  /// </summary>
  public bool IsFull => AvailableQuota <= 0;
}