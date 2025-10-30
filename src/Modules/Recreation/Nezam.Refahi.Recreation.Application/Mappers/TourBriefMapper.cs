using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Application.Mappers;

public class TourBriefMapper : IMapper<Tour, TourBriefDto>
{
    public Task<TourBriefDto> MapAsync(Tour source, CancellationToken cancellationToken = default)
    {
        var dto = new TourBriefDto
        {
            Id = source.Id,
            Title = source.Title,
            TourStart = source.TourStart,
            TourEnd = source.TourEnd,
            Status = source.Status.ToString(),
            IsActive = source.IsActive
        };

        return Task.FromResult(dto);
    }

    public Task MapAsync(Tour source, TourBriefDto destination, CancellationToken cancellationToken = default)
    {
        destination.Id = source.Id;
        destination.Title = source.Title;
        destination.TourStart = source.TourStart;
        destination.TourEnd = source.TourEnd;
        destination.Status = source.Status.ToString();
        destination.IsActive = source.IsActive;
        return Task.CompletedTask;
    }
}

