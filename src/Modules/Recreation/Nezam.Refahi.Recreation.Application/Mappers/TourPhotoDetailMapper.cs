using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Application.Mappers;

public class TourPhotoDetailMapper : IMapper<TourPhoto, PhotoDetailDto>
{
    public Task<PhotoDetailDto> MapAsync(TourPhoto source, CancellationToken cancellationToken = default)
    {
        var dto = new PhotoDetailDto
        {
            Id = source.Id,
            Url = source.Url ?? string.Empty,
            Caption = source.Caption,
            DisplayOrder = source.DisplayOrder
        };

        return Task.FromResult(dto);
    }

    public Task MapAsync(TourPhoto source, PhotoDetailDto destination, CancellationToken cancellationToken = default)
    {
        destination.Id = source.Id;
        destination.Url = source.Url ?? string.Empty;
        destination.Caption = source.Caption;
        destination.DisplayOrder = source.DisplayOrder;
        return Task.CompletedTask;
    }
}

