namespace Nezam.Refahi.Recreation.Contracts.Dtos;

/// <summary>
/// Restricted tour data transfer object
/// </summary>
public class RestrictedTourDto
{
  public Guid Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public DateTime TourStart { get; set; }
  public DateTime TourEnd { get; set; }
  public bool IsActive { get; set; }
}