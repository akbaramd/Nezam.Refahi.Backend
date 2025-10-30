namespace Nezam.Refahi.Recreation.Contracts.Dtos;

public sealed class FeatureDetailDto
{
  public Guid FeatureId { get; set; } = Guid.Empty;
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; } = string.Empty;
}