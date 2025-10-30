namespace Nezam.Refahi.Recreation.Contracts.Dtos;

public sealed class FeatureSummaryDto
{
  public Guid FeatureId { get; set; } = Guid.Empty;
  public string Name { get; set; } = string.Empty;
}