using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Application.Mappers;

public class TourAgencyMapper : IMapper<TourAgency, AgencySummaryDto>
{
    public Task<AgencySummaryDto> MapAsync(TourAgency source, CancellationToken cancellationToken = default)
    {
        var dto = new AgencySummaryDto
        {
            AgencyId = source.AgencyId,
            AgencyName = source.AgencyName?.Trim() ?? string.Empty
        };

        return Task.FromResult(dto);
    }

    public Task MapAsync(TourAgency source, AgencySummaryDto destination, CancellationToken cancellationToken = default)
    {
        destination.AgencyId = source.AgencyId;
        destination.AgencyName = source.AgencyName?.Trim() ?? string.Empty;
        return Task.CompletedTask;
    }
}

