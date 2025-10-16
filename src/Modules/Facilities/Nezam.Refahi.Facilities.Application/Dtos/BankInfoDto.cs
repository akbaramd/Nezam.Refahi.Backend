namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails;

/// <summary>
/// Bank information data transfer object
/// </summary>
public record BankInfoDto
{
  /// <summary>
  /// Bank name
  /// </summary>
  public string? BankName { get; init; }

  /// <summary>
  /// Bank code
  /// </summary>
  public string? BankCode { get; init; }

  /// <summary>
  /// Bank account number
  /// </summary>
  public string? BankAccountNumber { get; init; }

  /// <summary>
  /// Indicates if bank information is available
  /// </summary>
  public bool IsAvailable => !string.IsNullOrWhiteSpace(BankName);
}