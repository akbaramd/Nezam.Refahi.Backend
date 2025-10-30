namespace Nezam.Refahi.Facilities.Application.Dtos;

/// <summary>
/// Detailed cycle rules data transfer object
/// </summary>
public record DetailedCycleRulesDto
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

  /// <summary>
  /// Repeatability description
  /// </summary>
  public string RepeatabilityDescription { get; init; } = null!;

  /// <summary>
  /// Exclusivity description
  /// </summary>
  public string ExclusivityDescription { get; init; } = null!;

  /// <summary>
  /// Dependency description
  /// </summary>
  public string DependencyDescription { get; init; } = null!;
}