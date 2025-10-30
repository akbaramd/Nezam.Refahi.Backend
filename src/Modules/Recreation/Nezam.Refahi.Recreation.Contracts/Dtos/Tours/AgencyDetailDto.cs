namespace Nezam.Refahi.Recreation.Contracts.Dtos;

public sealed class AgencyDetailDto
{
    public Guid AgencyId { get; set; } = Guid.Empty;
  public string AgencyName { get; set; } = string.Empty;
}