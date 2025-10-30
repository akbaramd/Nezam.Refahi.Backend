namespace Nezam.Refahi.Recreation.Contracts.Dtos;

public sealed class RestrictedTourSummaryDto
{
  public Guid RestrictedTourId { get; set; } = Guid.Empty;
  public string Title { get; set; } = string.Empty;
}