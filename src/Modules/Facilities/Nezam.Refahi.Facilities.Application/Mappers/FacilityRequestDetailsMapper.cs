using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Facilities.Application.Dtos;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycles;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityDetails;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityRequestDetails;
using Nezam.Refahi.Facilities.Application.Services;
using Nezam.Refahi.Facilities.Domain.Entities;
using Nezam.Refahi.Facilities.Domain.Enums;

namespace Nezam.Refahi.Facilities.Application.Mappers;

public sealed class FacilityRequestDetailsMapper : IMapper<FacilityRequest, FacilityRequestDetailsDto>
{
    private readonly IMapper<Facility, FacilityInfoDto> _facilityInfoMapper;
    private readonly IMapper<Facility, FacilityDetailsDto> _facilityDetailsMapper;
    private readonly IMapper<FacilityCycle, FacilityCycleWithUserDto> _cycleDetailsMapper;

    public FacilityRequestDetailsMapper(
        IMapper<Facility, FacilityInfoDto> facilityInfoMapper,
        IMapper<Facility, FacilityDetailsDto> facilityDetailsMapper,
        IMapper<FacilityCycle, FacilityCycleWithUserDto> cycleDetailsMapper)
    {
        _facilityInfoMapper = facilityInfoMapper;
        _facilityDetailsMapper = facilityDetailsMapper;
        _cycleDetailsMapper = cycleDetailsMapper;
    }

    public async Task<FacilityRequestDetailsDto> MapAsync(FacilityRequest source, CancellationToken cancellationToken = new CancellationToken())
    {
        var facilityInfoTask = _facilityInfoMapper.MapAsync(source.Facility, cancellationToken);
        var facilityDetailsTask = _facilityDetailsMapper.MapAsync(source.Facility, cancellationToken);
        var cycleTask = _cycleDetailsMapper.MapAsync(source.FacilityCycle, cancellationToken);

        await Task.WhenAll(facilityInfoTask, facilityDetailsTask, cycleTask);

        return new FacilityRequestDetailsDto
        {
            Id = source.Id,
            Facility = facilityInfoTask.Result,
            Cycle = cycleTask.Result,
            Applicant = new ApplicantInfoDto
            {
                MemberId = source.MemberId,
                FullName = source.UserFullName,
                NationalId = source.UserNationalId,
            },
            RequestedAmountRials = source.RequestedAmount.AmountRials,
            ApprovedAmountRials = source.ApprovedAmount?.AmountRials,
            Currency = source.RequestedAmount.Currency,
            Status = source.Status.ToString(),
            StatusText = EnumTextMappingService.GetFacilityRequestStatusDescription(source.Status),
            CreatedAt = source.CreatedAt,
            LastModifiedAt = source.LastModifiedAt,
            IsInProgress = EnumTextMappingService.IsRequestInProgress(source.Status),
            IsCompleted = EnumTextMappingService.IsRequestCompleted(source.Status),
            IsRejected = EnumTextMappingService.IsRequestRejected(source.Status),
            IsCancelled = EnumTextMappingService.IsRequestCancelled(source.Status),
            RequestNumber = source.RequestNumber!,
            Description = source.Description,
            RejectionReason = source.RejectionReason,
            Timeline = new RequestTimelineDto
            {
                CreatedAt = source.CreatedAt,
                ApprovedAt = source.ApprovedAt,
                RejectedAt = source.RejectedAt,
                BankAppointmentScheduledAt = source.BankAppointmentScheduledAt,
                BankAppointmentDate = source.BankAppointmentDate,
                DisbursedAt = source.DisbursedAt,
                CompletedAt = source.CompletedAt
            },
            IsTerminal = EnumTextMappingService.IsRequestTerminal(source.Status),
            CanBeCancelled = EnumTextMappingService.CanRequestBeCancelled(source.Status),
            RequiresApplicantAction = EnumTextMappingService.RequiresApplicantAction(source.Status),
            RequiresBankAction = EnumTextMappingService.RequiresBankAction(source.Status),
            FacilityDetails = facilityDetailsTask.Result,
        };
    }

    public Task MapAsync(FacilityRequest source, FacilityRequestDetailsDto destination, CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.CompletedTask;
    }
}


