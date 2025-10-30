namespace Nezam.Refahi.Facilities.Application.Dtos;

/// <summary>
/// Detailed eligibility analysis data transfer object
/// </summary>
public record DetailedEligibilityDto
{
  /// <summary>
  /// Overall eligibility status
  /// </summary>
  public bool IsEligible { get; init; }

  /// <summary>
  /// Overall validation message
  /// </summary>
  public string? ValidationMessage { get; init; }

  /// <summary>
  /// List of validation errors
  /// </summary>
  public List<string> ValidationErrors { get; init; } = new();

  /// <summary>
  /// Feature requirements analysis
  /// </summary>
  public FeatureRequirementsDto FeatureRequirements { get; init; } = null!;

  /// <summary>
  /// Capability requirements analysis
  /// </summary>
  public CapabilityRequirementsDto CapabilityRequirements { get; init; } = null!;

  /// <summary>
  /// Dependency requirements analysis
  /// </summary>
  public DependencyRequirementsDto DependencyRequirements { get; init; } = null!;

  /// <summary>
  /// Cooldown requirements analysis
  /// </summary>
  public CooldownRequirementsDto CooldownRequirements { get; init; } = null!;

  /// <summary>
  /// Quota availability analysis
  /// </summary>
  public QuotaAvailabilityDto QuotaAvailability { get; init; } = null!;
}