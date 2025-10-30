using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Application.Dtos;

public sealed class AgencySummaryDto
{
  public Guid AgencyId { get; init; }
  public string AgencyName { get; init; } = string.Empty;

  public static AgencySummaryDto From(TourAgency a) =>
    new()
    {
      AgencyId = a.AgencyId,
      AgencyName = a.AgencyName?.Trim() ?? string.Empty
    };
}