namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails;

/// <summary>
/// Detailed financial terms data transfer object
/// </summary>
public record DetailedFinancialTermsDto
{
  /// <summary>
  /// Minimum amount in Rials
  /// </summary>
  public decimal? MinAmountRials { get; init; }

  /// <summary>
  /// Maximum amount in Rials
  /// </summary>
  public decimal? MaxAmountRials { get; init; }

  /// <summary>
  /// Default amount in Rials
  /// </summary>
  public decimal? DefaultAmountRials { get; init; }

  /// <summary>
  /// Currency code
  /// </summary>
  public string Currency { get; init; } = "IRR";

  /// <summary>
  /// Payment duration in months
  /// </summary>
  public int PaymentMonths { get; init; }

  /// <summary>
  /// Annual interest rate (as decimal, e.g., 0.18 for 18%)
  /// </summary>
  public decimal? InterestRate { get; init; }

  /// <summary>
  /// Interest rate as percentage (e.g., 18.00 for 18%)
  /// </summary>
  public decimal? InterestRatePercentage { get; init; }

  /// <summary>
  /// Cooldown period in days after receiving facility
  /// </summary>
  public int CooldownDays { get; init; }

  /// <summary>
  /// Indicates if financial terms are available
  /// </summary>
  public bool HasFinancialTerms => MinAmountRials.HasValue || MaxAmountRials.HasValue || DefaultAmountRials.HasValue;

  /// <summary>
  /// Formatted minimum amount for display
  /// </summary>
  public string? FormattedMinAmount { get; init; }

  /// <summary>
  /// Formatted maximum amount for display
  /// </summary>
  public string? FormattedMaxAmount { get; init; }

  /// <summary>
  /// Formatted default amount for display
  /// </summary>
  public string? FormattedDefaultAmount { get; init; }

  /// <summary>
  /// Formatted interest rate for display
  /// </summary>
  public string? FormattedInterestRate { get; init; }

  /// <summary>
  /// Monthly payment amount (if calculable)
  /// </summary>
  public decimal? MonthlyPaymentAmount { get; init; }
}