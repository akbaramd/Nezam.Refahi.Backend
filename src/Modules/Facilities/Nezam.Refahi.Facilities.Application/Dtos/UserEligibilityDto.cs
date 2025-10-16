namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycles;

/// <summary>
/// User eligibility information data transfer object
/// </summary>
public record UserEligibilityDto
{
  /// <summary>
  /// Indicates if user is eligible for this cycle
  /// </summary>
  public bool IsEligible { get; init; }

  /// <summary>
  /// Eligibility validation message
  /// </summary>
  public string? ValidationMessage { get; init; }

  /// <summary>
  /// List of validation errors
  /// </summary>
  public List<string> ValidationErrors { get; init; } = new();

  /// <summary>
  /// Indicates if user meets feature requirements
  /// </summary>
  public bool MeetsFeatureRequirements { get; init; }

  /// <summary>
  /// Indicates if user meets capability requirements
  /// </summary>
  public bool MeetsCapabilityRequirements { get; init; }

  /// <summary>
  /// Missing features (if any)
  /// </summary>
  public List<string> MissingFeatures { get; init; } = new();

  /// <summary>
  /// Missing capabilities (if any)
  /// </summary>
  public List<string> MissingCapabilities { get; init; } = new();
}