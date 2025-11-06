namespace Nezam.Refahi.Facilities.Application.Dtos;

/// <summary>
/// Cycle rules data transfer object
/// </summary>
public record CycleRulesDto
{
  /// <summary>
  /// Indicates if cycle restricts users who had approved requests in previous cycles of the same facility
  /// </summary>
  public bool RestrictToPreviousCycles { get; init; }

  /// <summary>
  /// Indicates if cycle has dependency rules
  /// </summary>
  public bool HasDependencies { get; init; }
}