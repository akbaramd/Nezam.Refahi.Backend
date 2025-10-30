using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Application.Mappers;

public class TourPhotoMapper : IMapper<TourPhoto, PhotoSummaryDto>
{
    public Task<PhotoSummaryDto> MapAsync(TourPhoto source, CancellationToken cancellationToken = default)
    {
        var dto = new PhotoSummaryDto
        {
            Id = source.Id,
            Url = source.Url ?? string.Empty,
            DisplayOrder = source.DisplayOrder
        };

        return Task.FromResult(dto);
    }

    public Task MapAsync(TourPhoto source, PhotoSummaryDto destination, CancellationToken cancellationToken = default)
    {
        destination.Id = source.Id;
        destination.Url = source.Url ?? string.Empty;
        destination.DisplayOrder = source.DisplayOrder;
        return Task.CompletedTask;
    }
}

