namespace Nezam.Refahi.Recreation.Contracts.Dtos;

/// <summary>
/// Tour photo data transfer object
/// </summary>
public class TourPhotoDto
{
  public Guid Id { get; set; }
  public string FilePath { get; set; } = string.Empty;
  public string Url { get; set; } = string.Empty;
  public string? Caption { get; set; }
  public int DisplayOrder { get; set; }
}