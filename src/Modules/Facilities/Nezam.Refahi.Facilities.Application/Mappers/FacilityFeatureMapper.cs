using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Facilities.Application.Dtos;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityDetails;
using Nezam.Refahi.Facilities.Domain.Entities;

namespace Nezam.Refahi.Facilities.Application.Mappers;

public sealed class FacilityFeatureMapper : IMapper<FacilityFeature, FacilityFeatureDto>
{
    public Task<FacilityFeatureDto> MapAsync(FacilityFeature source, CancellationToken cancellationToken = new CancellationToken())
    {
        var dto = new FacilityFeatureDto
        {
            Id = source.Id,
            FeatureId = source.FeatureId,
            RequirementType = source.RequirementType.ToString(),
            Notes = source.Notes,
            AssignedAt = source.AssignedAt
        };

        return Task.FromResult(dto);
    }

    public Task MapAsync(FacilityFeature source, FacilityFeatureDto destination, CancellationToken cancellationToken = new CancellationToken())
    {
        destination.Id = source.Id;
        destination.FeatureId = source.FeatureId;
        destination.RequirementType = source.RequirementType.ToString();
        destination.Notes = source.Notes;
        destination.AssignedAt = source.AssignedAt;
        return Task.CompletedTask;
    }
}


