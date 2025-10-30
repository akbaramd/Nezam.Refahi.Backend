namespace Nezam.Refahi.Facilities.Application.Dtos;

/// <summary>
/// Dependency requirements analysis data transfer object
/// </summary>
public record DependencyRequirementsDto
{
  /// <summary>
  /// Indicates if user meets dependency requirements
  /// </summary>
  public bool MeetsRequirements { get; init; }

  /// <summary>
  /// Required dependencies
  /// </summary>
  public List<CycleDependencyDto> RequiredDependencies { get; init; } = new();

  /// <summary>
  /// User's completed facilities
  /// </summary>
  public List<Guid> UserCompletedFacilities { get; init; } = new();

  /// <summary>
  /// Missing dependencies
  /// </summary>
  public List<CycleDependencyDto> MissingDependencies { get; init; } = new();

  /// <summary>
  /// Satisfied dependencies
  /// </summary>
  public List<CycleDependencyDto> SatisfiedDependencies { get; init; } = new();
}