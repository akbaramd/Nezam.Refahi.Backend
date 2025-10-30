namespace Nezam.Refahi.Facilities.Application.Dtos;

/// <summary>
/// Feature requirements analysis data transfer object
/// </summary>
public record FeatureRequirementsDto
{
  /// <summary>
  /// Indicates if user meets feature requirements
  /// </summary>
  public bool MeetsRequirements { get; init; }

  /// <summary>
  /// Required features
  /// </summary>
  public List<string> RequiredFeatures { get; init; } = new();

  /// <summary>
  /// User's features
  /// </summary>
  public List<string> UserFeatures { get; init; } = new();

  /// <summary>
  /// Missing features
  /// </summary>
  public List<string> MissingFeatures { get; init; } = new();

  /// <summary>
  /// Matching features
  /// </summary>
  public List<string> MatchingFeatures { get; init; } = new();
}