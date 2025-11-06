namespace Nezam.Refahi.Facilities.Application.Dtos;

/// <summary>
/// Financial terms data transfer object
/// </summary>
public record FinancialTermsDto
{
  /// <summary>
  /// List of available price options for this cycle
  /// </summary>
  public List<FacilityCyclePriceOptionDto> PriceOptions { get; init; } = new();

  /// <summary>
  /// Currency code
  /// </summary>
  public string Currency { get; init; } = "IRR";

  /// <summary>
  /// Payment duration in months
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
  /// Indicates if financial terms are available
  /// </summary>
  public bool HasFinancialTerms => PriceOptions.Any() || PaymentMonths.HasValue || InterestRate.HasValue;
}