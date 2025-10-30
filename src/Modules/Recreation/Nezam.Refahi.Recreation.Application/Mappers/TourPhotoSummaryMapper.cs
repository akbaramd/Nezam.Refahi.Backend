using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Application.Mappers;

public class TourPhotoSummaryMapper : IMapper<TourPhoto, TourPhotoSummaryDto>
{
    public Task<TourPhotoSummaryDto> MapAsync(TourPhoto source, CancellationToken cancellationToken = default)
    {
        var dto = new TourPhotoSummaryDto
        {
            Id = source.Id,
            Url = source.Url ?? string.Empty,
            DisplayOrder = source.DisplayOrder
        };

        return Task.FromResult(dto);
    }

    public Task MapAsync(TourPhoto source, TourPhotoSummaryDto destination, CancellationToken cancellationToken = default)
    {
        destination.Id = source.Id;
        destination.Url = source.Url ?? string.Empty;
        destination.DisplayOrder = source.DisplayOrder;
        return Task.CompletedTask;
    }
}

