namespace Nezam.Refahi.Facilities.Application.Dtos;

/// <summary>
/// Request financial information data transfer object
/// </summary>
public record RequestFinancialInfoDto
{
  /// <summary>
  /// Requested amount in Rials
  /// </summary>
  public decimal RequestedAmountRials { get; init; }

  /// <summary>
  /// Approved amount in Rials (if different)
  /// </summary>
  public decimal? ApprovedAmountRials { get; init; }

  /// <summary>
  /// Currency code
  /// </summary>
  public string Currency { get; init; } = "IRR";

  /// <summary>
  /// Indicates if amount was modified during approval
  /// </summary>
  public bool AmountWasModified => ApprovedAmountRials.HasValue && ApprovedAmountRials != RequestedAmountRials;

  /// <summary>
  /// Final approved amount
  /// </summary>
  public decimal FinalAmountRials => ApprovedAmountRials ?? RequestedAmountRials;

  /// <summary>
  /// Formatted requested amount
  /// </summary>
  public string FormattedRequestedAmount { get; init; } = null!;

  /// <summary>
  /// Formatted approved amount
  /// </summary>
  public string? FormattedApprovedAmount { get; init; }

  /// <summary>
  /// Formatted final amount
  /// </summary>
  public string FormattedFinalAmount { get; init; } = null!;
}