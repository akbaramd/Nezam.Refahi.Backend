using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Facilities.Application.Services;
using Nezam.Refahi.Facilities.Domain.Enums;
using Nezam.Refahi.Facilities.Domain.Repositories;
using Nezam.Refahi.Facilities.Domain.Services;
using Nezam.Refahi.Membership.Contracts.Dtos;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Shared.Application.Common.Models;
using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Facilities.Application.Dtos;
using Nezam.Refahi.Facilities.Application.Mappers;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails;

/// <summary>
/// Handler for getting detailed facility cycle information with user context
/// </summary>
public class GetFacilityCycleDetailsQueryHandler : IRequestHandler<GetFacilityCycleDetailsQuery, ApplicationResult<FacilityCycleWithUserDetailDto>>
{
    private readonly IFacilityCycleRepository _cycleRepository;
    private readonly IFacilityRepository _facilityRepository;
    private readonly IFacilityRequestRepository _requestRepository;
    private readonly IMemberService _memberService;
    private readonly IValidator<GetFacilityCycleDetailsQuery> _validator;
    private readonly IMapper<Domain.Entities.FacilityCycle, FacilityCycleWithUserDetailDto> _cycleDetailsMapper;
    private readonly IMapper<Domain.Entities.Facility, FacilityInfoDto> _facilityInfoMapper;
    private readonly IMapper<Domain.Entities.FacilityRequest, FacilityRequestDto> _requestMapper;
    private readonly ILogger<GetFacilityCycleDetailsQueryHandler> _logger;

    public GetFacilityCycleDetailsQueryHandler(
        IFacilityCycleRepository cycleRepository,
        IFacilityRepository facilityRepository,
        IFacilityRequestRepository requestRepository,
        IMemberService memberService,
        IValidator<GetFacilityCycleDetailsQuery> validator,
        ILogger<GetFacilityCycleDetailsQueryHandler> logger,
        IMapper<Domain.Entities.FacilityCycle, FacilityCycleWithUserDetailDto> cycleDetailsMapper,
        IMapper<Domain.Entities.Facility, FacilityInfoDto> facilityInfoMapper,
        IMapper<Domain.Entities.FacilityRequest, FacilityRequestDto> requestMapper)
    {
        _cycleRepository = cycleRepository;
        _facilityRepository = facilityRepository;
        _requestRepository = requestRepository;
        _memberService = memberService;
        _validator = validator;
        _logger = logger;
        _cycleDetailsMapper = cycleDetailsMapper;
        _facilityInfoMapper = facilityInfoMapper;
        _requestMapper = requestMapper;
    }

    public async Task<ApplicationResult<FacilityCycleWithUserDetailDto>> Handle(
        GetFacilityCycleDetailsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate input
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ApplicationResult<FacilityCycleWithUserDetailDto>.Failure(
                    errors,
                    "اطلاعات ورودی نامعتبر است");
            }

            // Get cycle with all details
            var cycle = await _cycleRepository.GetByIdAsync(request.CycleId, cancellationToken);
            if (cycle == null)
            {
                return ApplicationResult<FacilityCycleWithUserDetailDto>.Failure(
                    "دوره تسهیلات مورد نظر یافت نشد");
            }


           var data = await _cycleDetailsMapper.MapAsync(cycle, cancellationToken);

            return ApplicationResult<FacilityCycleWithUserDetailDto>.Success(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving detailed cycle information for cycle {CycleId}, user {NationalNumber}",
                request.CycleId, request.NationalNumber);
            return ApplicationResult<FacilityCycleWithUserDetailDto>.Failure(
                "خطای داخلی در دریافت جزئیات دوره تسهیلات");
        }
    }
}
