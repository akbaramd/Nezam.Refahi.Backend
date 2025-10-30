using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Facilities.Application.Dtos;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityDetails;
using Nezam.Refahi.Facilities.Domain.Entities;

namespace Nezam.Refahi.Facilities.Application.Mappers;

public sealed class FacilityCapabilityPolicyMapper : IMapper<FacilityCapability, FacilityCapabilityPolicyDto>
{
    public Task<FacilityCapabilityPolicyDto> MapAsync(FacilityCapability source, CancellationToken cancellationToken = new CancellationToken())
    {
        var dto = new FacilityCapabilityPolicyDto
        {
            Id = source.Id,
            CapabilityId = source.CapabilityId
        };
        return Task.FromResult(dto);
    }

    public Task MapAsync(FacilityCapability source, FacilityCapabilityPolicyDto destination, CancellationToken cancellationToken = new CancellationToken())
    {
        destination.Id = source.Id;
        destination.CapabilityId = source.CapabilityId;
        return Task.CompletedTask;
    }
}


