using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Application.Mappers;

public class TourFeatureDetailMapper : IMapper<TourFeature, FeatureDetailDto>
{
    public Task<FeatureDetailDto> MapAsync(TourFeature source, CancellationToken cancellationToken = default)
    {
        var dto = new FeatureDetailDto
        {
            FeatureId = source.FeatureId,
            Name = source.Feature?.Name?.Trim() ?? string.Empty,
            Description = source.Feature?.Description
        };

        return Task.FromResult(dto);
    }

    public Task MapAsync(TourFeature source, FeatureDetailDto destination, CancellationToken cancellationToken = default)
    {
        destination.FeatureId = source.FeatureId;
        destination.Name = source.Feature?.Name?.Trim() ?? string.Empty;
        destination.Description = source.Feature?.Description;
        return Task.CompletedTask;
    }
}

