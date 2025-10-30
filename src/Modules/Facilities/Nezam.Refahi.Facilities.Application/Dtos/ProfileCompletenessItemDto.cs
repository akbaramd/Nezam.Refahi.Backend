namespace Nezam.Refahi.Facilities.Application.Dtos;

/// <summary>
/// Profile completeness item data transfer object
/// </summary>
public record ProfileCompletenessItemDto
{
  /// <summary>
  /// Field name
  /// </summary>
  public string FieldName { get; init; } = null!;

  /// <summary>
  /// Field display name
  /// </summary>
  public string DisplayName { get; init; } = null!;

  /// <summary>
  /// Indicates if field is required
  /// </summary>
  public bool IsRequired { get; init; }

  /// <summary>
  /// Indicates if field is completed
  /// </summary>
  public bool IsCompleted { get; init; }

  /// <summary>
  /// Field value (if completed)
  /// </summary>
  public string? Value { get; init; }

  /// <summary>
  /// Field description
  /// </summary>
  public string? Description { get; init; }

  /// <summary>
  /// Field category
  /// </summary>
  public string? Category { get; init; }

  /// <summary>
  /// Completion weight (for percentage calculation)
  /// </summary>
  public decimal Weight { get; init; }
}