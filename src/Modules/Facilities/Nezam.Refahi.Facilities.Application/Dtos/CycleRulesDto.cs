namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycles;

/// <summary>
/// Cycle rules data transfer object
/// </summary>
public record CycleRulesDto
{
  /// <summary>
  /// Indicates if facility can be repeated
  /// </summary>
  public bool IsRepeatable { get; init; }

  /// <summary>
  /// Indicates if facility is exclusive
  /// </summary>
  public bool IsExclusive { get; init; }

  /// <summary>
  /// Exclusive set identifier
  /// </summary>
  public string? ExclusiveSetId { get; init; }

  /// <summary>
  /// Maximum active facilities across all cycles
  /// </summary>
  public int? MaxActiveAcrossCycles { get; init; }

  /// <summary>
  /// Indicates if cycle has dependency rules
  /// </summary>
  public bool HasDependencies { get; init; }

  /// <summary>
  /// Indicates if cycle has exclusive rules
  /// </summary>
  public bool HasExclusiveRules => IsExclusive && !string.IsNullOrWhiteSpace(ExclusiveSetId);
}