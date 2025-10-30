using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Application.Mappers;

public class TourRestrictedTourMapper : IMapper<TourRestrictedTour, RestrictedTourSummaryDto>
{
    public Task<RestrictedTourSummaryDto> MapAsync(TourRestrictedTour source, CancellationToken cancellationToken = default)
    {
        var dto = new RestrictedTourSummaryDto
        {
            RestrictedTourId = source.RestrictedTourId,
            Title = source.RestrictedTour?.Title?.Trim() ?? string.Empty
        };

        return Task.FromResult(dto);
    }

    public Task MapAsync(TourRestrictedTour source, RestrictedTourSummaryDto destination, CancellationToken cancellationToken = default)
    {
        destination.RestrictedTourId = source.RestrictedTourId;
        destination.Title = source.RestrictedTour?.Title?.Trim() ?? string.Empty;
        return Task.CompletedTask;
    }
}

