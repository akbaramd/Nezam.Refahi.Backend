namespace Nezam.Refahi.Recreation.Contracts.Dtos;

public sealed class PhotoSummaryDto
{
  public Guid Id { get; set; } = Guid.Empty;
  public string Url { get; set; } = string.Empty;
  public int DisplayOrder { get; set; } = 0;
}