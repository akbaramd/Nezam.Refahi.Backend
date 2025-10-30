using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Application.Mappers;

public class TourFeatureSummaryMapper : IMapper<TourFeature, TourFeatureSummaryDto>
{
    public Task<TourFeatureSummaryDto> MapAsync(TourFeature source, CancellationToken cancellationToken = default)
    {
        var dto = new TourFeatureSummaryDto
        {
            FeatureId = source.FeatureId,
            Name = source.Feature?.Name?.Trim() ?? string.Empty
        };

        return Task.FromResult(dto);
    }

    public Task MapAsync(TourFeature source, TourFeatureSummaryDto destination, CancellationToken cancellationToken = default)
    {
        destination.FeatureId = source.FeatureId;
        destination.Name = source.Feature?.Name?.Trim() ?? string.Empty;
        return Task.CompletedTask;
    }
}

