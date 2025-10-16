namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails;

/// <summary>
/// Capability requirements analysis data transfer object
/// </summary>
public record CapabilityRequirementsDto
{
  /// <summary>
  /// Indicates if user meets capability requirements
  /// </summary>
  public bool MeetsRequirements { get; init; }

  /// <summary>
  /// Required capabilities
  /// </summary>
  public List<string> RequiredCapabilities { get; init; } = new();

  /// <summary>
  /// User's capabilities
  /// </summary>
  public List<string> UserCapabilities { get; init; } = new();

  /// <summary>
  /// Missing capabilities
  /// </summary>
  public List<string> MissingCapabilities { get; init; } = new();

  /// <summary>
  /// Matching capabilities
  /// </summary>
  public List<string> MatchingCapabilities { get; init; } = new();
}