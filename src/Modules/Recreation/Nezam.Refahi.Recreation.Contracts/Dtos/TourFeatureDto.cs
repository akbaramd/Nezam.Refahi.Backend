namespace Nezam.Refahi.Recreation.Contracts.Dtos;

/// <summary>
/// Tour feature data transfer object
/// </summary>
public class TourFeatureDto
{
  public Guid Id { get; set; }
  public Guid TourId { get; set; }
  public Guid FeatureId { get; set; }
  public string? Value { get; set; }
  public FeatureDto? Feature { get; set; }
}