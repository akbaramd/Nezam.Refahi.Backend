namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycles;

/// <summary>
/// Simple facility cycle data transfer object for lists
/// Contains only essential fields for list/array display
/// </summary>
public record FacilityCycleDto
{
  /// <summary>
  /// Unique cycle identifier
  /// </summary>
  public Guid Id { get; init; }

  /// <summary>
  /// Cycle display name
  /// </summary>
  public string Name { get; init; } = null!;

  /// <summary>
  /// Cycle start date
  /// </summary>
  public DateTime StartDate { get; init; }

  /// <summary>
  /// Cycle end date
  /// </summary>
  public DateTime EndDate { get; init; }

  /// <summary>
  /// Days remaining until cycle ends
  /// </summary>
  public int DaysUntilEnd { get; init; }

  /// <summary>
  /// Indicates if cycle is currently active
  /// </summary>
  public bool IsActive { get; init; }

  /// <summary>
  /// Total quota for this cycle
  /// </summary>
  public int Quota { get; init; }

  /// <summary>
  /// Used quota count
  /// </summary>
  public int UsedQuota { get; init; }

  /// <summary>
  /// Available quota count
  /// </summary>
  public int AvailableQuota { get; init; }

  /// <summary>
  /// Cycle status (Draft, Active, Closed, Completed, Cancelled)
  /// </summary>
  public string Status { get; init; } = null!;

  /// <summary>
  /// Human-readable status text
  /// </summary>
  public string StatusText { get; init; } = null!;

  /// <summary>
  /// Minimum amount for this cycle
  /// </summary>
  public decimal MinAmountRials { get; init; }

  /// <summary>
  /// Maximum amount for this cycle
  /// </summary>
  public decimal MaxAmountRials { get; init; }

  /// <summary>
  /// Number of payment months
  /// </summary>
  public int PaymentMonths { get; init; }

  /// <summary>
  /// Cooldown period in days
  /// </summary>
  public int CooldownDays { get; init; }

  /// <summary>
  /// Cycle creation timestamp
  /// </summary>
  public DateTime CreatedAt { get; init; }
}