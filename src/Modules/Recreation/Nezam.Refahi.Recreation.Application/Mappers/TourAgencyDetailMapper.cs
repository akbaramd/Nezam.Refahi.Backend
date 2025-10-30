using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Application.Mappers;

public class TourAgencyDetailMapper : IMapper<TourAgency, AgencyDetailDto>
{
    public Task<AgencyDetailDto> MapAsync(TourAgency source, CancellationToken cancellationToken = default)
    {
        var dto = new AgencyDetailDto
        {
            AgencyId = source.AgencyId,
            AgencyName = source.AgencyName?.Trim() ?? string.Empty
        };

        return Task.FromResult(dto);
    }

    public Task MapAsync(TourAgency source, AgencyDetailDto destination, CancellationToken cancellationToken = default)
    {
        destination.AgencyId = source.AgencyId;
        destination.AgencyName = source.AgencyName?.Trim() ?? string.Empty;
        return Task.CompletedTask;
    }
}

