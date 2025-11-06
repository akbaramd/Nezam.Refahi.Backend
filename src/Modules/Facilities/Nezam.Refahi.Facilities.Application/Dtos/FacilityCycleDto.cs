namespace Nezam.Refahi.Facilities.Application.Dtos;

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
  /// Cycle status (Draft, Active, Closed, UnderReview, Completed, Cancelled)
  /// </summary>
  public string Status { get; init; } = null!;

  /// <summary>
  /// Human-readable status text
  /// </summary>
  public string StatusText { get; init; } = null!;

  /// <summary>
  /// Cycle description
  /// </summary>
  public string? Description { get; init; }

  /// <summary>
  /// List of available price options for this cycle
  /// </summary>
  public List<FacilityCyclePriceOptionDto> PriceOptions { get; init; } = new();

  /// <summary>
  /// Number of payment months (optional)
  /// </summary>
  public int? PaymentMonths { get; init; }

  /// <summary>
  /// Annual interest rate (as decimal, e.g., 0.18 for 18%)
  /// </summary>
  public decimal? InterestRate { get; init; }

  /// <summary>
  /// Interest rate as percentage (e.g., 18.00 for 18%)
  /// </summary>
  public decimal? InterestRatePercentage { get; init; }

  /// <summary>
  /// Cycle creation timestamp
  /// </summary>
  public DateTime CreatedAt { get; init; }
}