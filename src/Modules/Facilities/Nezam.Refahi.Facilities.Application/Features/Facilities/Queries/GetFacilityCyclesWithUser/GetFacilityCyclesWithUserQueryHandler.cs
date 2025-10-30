using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails;
using Nezam.Refahi.Facilities.Application.Services;
using Nezam.Refahi.Facilities.Domain.Enums;
using Nezam.Refahi.Facilities.Domain.Repositories;
using Nezam.Refahi.Facilities.Domain.Services;
using Nezam.Refahi.Membership.Contracts.Dtos;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;
using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Facilities.Application.Dtos;
using Nezam.Refahi.Facilities.Application.Spesifications;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycles;

/// <summary>
/// Handler for getting facility cycles with user context
/// </summary>
public class GetFacilityCyclesWithUserQueryHandler : IRequestHandler<GetFacilityCyclesWithUserQuery, ApplicationResult<GetFacilityCyclesWithUserQueryResponse>>
{
    private readonly IFacilityRepository _facilityRepository;
    private readonly IFacilityCycleRepository _cycleRepository;
    private readonly IFacilityRequestRepository _requestRepository;
    private readonly IMemberInfoService _memberInfoService;
    private readonly FacilityEligibilityDomainService _eligibilityService;
    private readonly IMapper<Domain.Entities.FacilityCycle, FacilityCycleWithUserDto> _cycleMapper;
    private readonly IMapper<Domain.Entities.FacilityRequest, FacilityRequestDto> _requestMapper;
    private readonly IValidator<GetFacilityCyclesWithUserQuery> _validator;
    private readonly ILogger<GetFacilityCyclesWithUserQueryHandler> _logger;

    public GetFacilityCyclesWithUserQueryHandler(
        IFacilityRepository facilityRepository,
        IFacilityCycleRepository cycleRepository,
        IFacilityRequestRepository requestRepository,
        IMemberInfoService memberInfoService,
        FacilityEligibilityDomainService eligibilityService,
        IValidator<GetFacilityCyclesWithUserQuery> validator,
        ILogger<GetFacilityCyclesWithUserQueryHandler> logger,
        IMapper<Domain.Entities.FacilityCycle, FacilityCycleWithUserDto> cycleMapper,
        IMapper<Domain.Entities.FacilityRequest, FacilityRequestDto> requestMapper)
    {
        _facilityRepository = facilityRepository;
        _cycleRepository = cycleRepository;
        _requestRepository = requestRepository;
        _memberInfoService = memberInfoService;
        _eligibilityService = eligibilityService;
        _validator = validator;
        _logger = logger;
        _cycleMapper = cycleMapper;
        _requestMapper = requestMapper;
    }

    public async Task<ApplicationResult<GetFacilityCyclesWithUserQueryResponse>> Handle(
        GetFacilityCyclesWithUserQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate input
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ApplicationResult<GetFacilityCyclesWithUserQueryResponse>.Failure(
                    errors,
                    "اطلاعات ورودی نامعتبر است");
            }

            // Get facility
            var facility = await _facilityRepository.GetByIdAsync(request.FacilityId, cancellationToken);
            if (facility == null)
            {
                return ApplicationResult<GetFacilityCyclesWithUserQueryResponse>.Failure(
                    "تسهیلات مورد نظر یافت نشد");
            }

            // Get user member info if NationalNumber provided
            MemberInfoDto? memberInfo = null;
            if (!string.IsNullOrWhiteSpace(request.NationalNumber))
            {
                memberInfo = await _memberInfoService.GetMemberInfoAsync(new NationalId(request.NationalNumber));
                if (memberInfo == null)
                {
                    return ApplicationResult<GetFacilityCyclesWithUserQueryResponse>.Failure(
                        "اطلاعات عضو یافت نشد");
                }
            }

            // Use specification (pagination/filter/search) similar to facilities
            var spec = new FacilityCyclePaginatedSpec(
                request.FacilityId,
                request.Page,
                request.PageSize,
                request.Status,
                request.SearchTerm,
                request.OnlyActive);

            // If OnlyWithUserRequests: fetch user cycleIds and rebuild spec to filter
            if (request.OnlyWithUserRequests && memberInfo != null)
            {
                // prefetch all cycle ids for facility (unpaged) to intersect user requests
                var allCycleIds = (await _cycleRepository.GetByFacilityIdAsync(request.FacilityId, cancellationToken)).Select(c => c.Id).ToList();
                var userRequestCycleIds = await _requestRepository.GetCyclesWithUserRequestsAsync(memberInfo.Id, allCycleIds, cancellationToken);
                spec = new FacilityCyclePaginatedSpec(
                    request.FacilityId,
                    request.Page,
                    request.PageSize,
                    request.Status,
                    request.SearchTerm,
                    request.OnlyActive,
                    true,
                    userRequestCycleIds);
            }

            // Apply through repository
            var page = await _cycleRepository.GetPaginatedAsync(spec, cancellationToken);

            var paginatedCycles = page.Items;
            var totalCount = page.TotalCount;

        

            // Map cycles to DTOs with user context
            var cycleDtos = new List<FacilityCycleWithUserDto>();
            foreach (var cycle in paginatedCycles)
            {
                var dto = await _cycleMapper.MapAsync(cycle, cancellationToken);
                if (memberInfo != null)
                {
                    var lastRequest = await _requestRepository.GetUserLastRequestForCycleAsync(memberInfo.Id, cycle.Id, cancellationToken);
                    if (lastRequest != null)
                    {
                        var lastDto = await _requestMapper.MapAsync(lastRequest, cancellationToken);
                        dto.LastRequest = lastDto; ;
                    }
                }
                cycleDtos.Add(dto);
            }

          

            _logger.LogInformation("Retrieved {Count} cycles for facility {FacilityId}, user {NationalNumber}",
                cycleDtos.Count, request.FacilityId, request.NationalNumber);

                return ApplicationResult<GetFacilityCyclesWithUserQueryResponse>.Success(new GetFacilityCyclesWithUserQueryResponse
                {
                    Items = cycleDtos,
                    TotalCount = totalCount,
                    PageNumber = request.Page,
                    PageSize = request.PageSize
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving facility cycles for facility {FacilityId}, user {NationalNumber}",
                request.FacilityId, request.NationalNumber);
            return ApplicationResult<GetFacilityCyclesWithUserQueryResponse>.Failure(
                "خطای داخلی در دریافت لیست دوره‌های تسهیلات");
        }
    }



  
  
}
