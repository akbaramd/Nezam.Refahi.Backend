using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Facilities.Application.Dtos;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycles;
using Nezam.Refahi.Facilities.Application.Services;
using Nezam.Refahi.Facilities.Domain.Entities;

namespace Nezam.Refahi.Facilities.Application.Mappers;

public sealed class FacilityRequestMapper : IMapper<FacilityRequest, FacilityRequestDto>
{
    public Task<FacilityRequestDto> MapAsync(FacilityRequest source, CancellationToken cancellationToken = new CancellationToken())
    {
        var now = DateTime.UtcNow;
        var daysSinceCreated = (now - source.CreatedAt).Days;

        var dto = new FacilityRequestDto
        {
            Id = source.Id,
            Status = source.Status.ToString(),
            StatusText = EnumTextMappingService.GetFacilityRequestStatusDescription(source.Status),
            RequestedAmountRials = source.RequestedAmount.AmountRials,
            ApprovedAmountRials = source.ApprovedAmount?.AmountRials,
            CreatedAt = source.CreatedAt,
            ApprovedAt = source.ApprovedAt,
            RejectedAt = source.RejectedAt,
            RejectionReason = source.RejectionReason,
            DaysSinceCreated = daysSinceCreated,
            IsInProgress = EnumTextMappingService.IsRequestInProgress(source.Status),
            IsCompleted = EnumTextMappingService.IsRequestCompleted(source.Status),
            IsRejected = EnumTextMappingService.IsRequestRejected(source.Status),
            IsCancelled = EnumTextMappingService.IsRequestCancelled(source.Status)
        };

        return Task.FromResult(dto);
    }

    public Task MapAsync(FacilityRequest source, FacilityRequestDto destination, CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.CompletedTask;
    }
}


