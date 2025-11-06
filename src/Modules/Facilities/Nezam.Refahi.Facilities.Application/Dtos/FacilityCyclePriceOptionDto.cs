namespace Nezam.Refahi.Facilities.Application.Dtos;

/// <summary>
/// Price option data transfer object for facility cycles
/// </summary>
public record FacilityCyclePriceOptionDto
{
  /// <summary>
  /// Unique price option identifier
  /// </summary>
  public Guid Id { get; init; }

  /// <summary>
  /// Amount in Rials
  /// </summary>
  public decimal AmountRials { get; init; }

  /// <summary>
  /// Currency code
  /// </summary>
  public string Currency { get; init; } = "IRR";

  /// <summary>
  /// Display order for sorting
  /// </summary>
  public int DisplayOrder { get; init; }

  /// <summary>
  /// Optional description for this price option
  /// </summary>
  public string? Description { get; init; }

  /// <summary>
  /// Indicates if this price option is active
  /// </summary>
  public bool IsActive { get; init; }
}

