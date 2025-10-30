namespace Nezam.Refahi.Recreation.Contracts.Dtos;

public sealed class TourFeatureSummaryDto
{
  public Guid FeatureId { get; set; } = Guid.Empty;
  public string Name { get; set; } = string.Empty;
}