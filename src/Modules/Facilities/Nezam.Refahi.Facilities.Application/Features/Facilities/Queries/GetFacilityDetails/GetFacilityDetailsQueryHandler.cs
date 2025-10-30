using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycles;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityDetails;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails;
using Nezam.Refahi.Facilities.Application.Services;
using Nezam.Refahi.Facilities.Domain.Enums;
using Nezam.Refahi.Facilities.Domain.Repositories;
using Nezam.Refahi.Membership.Contracts.Dtos;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Shared.Application.Common.Models;
using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Facilities.Application.Dtos;
using Nezam.Refahi.Facilities.Application.Mappers;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityDetails;

/// <summary>
/// Handler for getting facility details
/// </summary>
public class GetFacilityDetailsQueryHandler : IRequestHandler<GetFacilityDetailsQuery, ApplicationResult<FacilityDetailsDto>>
{
    private readonly IFacilityRepository _facilityRepository;
    private readonly IFacilityRequestRepository _requestRepository;
    private readonly IMemberInfoService _memberInfoService;
    private readonly IMapper<Domain.Entities.Facility, FacilityDetailsDto> _facilityDetailsMapper;
    private readonly IValidator<GetFacilityDetailsQuery> _validator;
    private readonly ILogger<GetFacilityDetailsQueryHandler> _logger;

    public GetFacilityDetailsQueryHandler(
        IFacilityRepository facilityRepository,
        IFacilityRequestRepository requestRepository,
        IMemberInfoService memberInfoService,
        IValidator<GetFacilityDetailsQuery> validator,
        ILogger<GetFacilityDetailsQueryHandler> logger,
        IMapper<Domain.Entities.Facility, FacilityDetailsDto> facilityDetailsMapper)
    {
        _facilityRepository = facilityRepository;
        _requestRepository = requestRepository;
        _memberInfoService = memberInfoService;
        _facilityDetailsMapper = facilityDetailsMapper;
        _validator = validator;
        _logger = logger;
    }

    public async Task<ApplicationResult<FacilityDetailsDto>> Handle(
        GetFacilityDetailsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate input
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ApplicationResult<FacilityDetailsDto >.Failure(
                    errors, 
                    "اطلاعات ورودی نامعتبر است");
            }

            // Get facility with related data
            var facility = await _facilityRepository.GetByIdWithDetailsAsync(
                request.FacilityId,
                request.IncludeCycles,
                request.IncludeFeatures,
                request.IncludePolicies,
                cancellationToken);

            if (facility == null)
            {
                return ApplicationResult<FacilityDetailsDto>.Failure(
                    "تسهیلات مورد نظر یافت نشد");
            }

            // Get user member info if NationalNumber provided
            MemberInfoDto? memberInfo = null;
            if (!string.IsNullOrWhiteSpace(request.NationalNumber))
            {
                memberInfo = await _memberInfoService.GetMemberInfoAsync(new NationalId(request.NationalNumber));
                if (memberInfo == null)
                {
                    return ApplicationResult<FacilityDetailsDto>.Failure(
                        "اطلاعات عضو یافت نشد");
                }
            }

            // Map to DTO via mapper
            var facilityDto = await _facilityDetailsMapper.MapAsync(facility, cancellationToken);

            _logger.LogInformation("Retrieved facility details for {FacilityId}",
                request.FacilityId);

            return ApplicationResult<FacilityDetailsDto>.Success(facilityDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving facility details for {FacilityId}",
                request.FacilityId);
            return ApplicationResult<FacilityDetailsDto>.Failure(
                "خطای داخلی در دریافت جزئیات تسهیلات");
        }
    }


  
}
