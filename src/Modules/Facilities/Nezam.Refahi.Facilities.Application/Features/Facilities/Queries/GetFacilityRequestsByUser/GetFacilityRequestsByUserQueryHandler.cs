using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycles;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityRequests;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserCycleRequests;
using Nezam.Refahi.Facilities.Application.Services;
using Nezam.Refahi.Facilities.Domain.Enums;
using Nezam.Refahi.Facilities.Domain.Repositories;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;
using FacilityInfoDto = Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails.FacilityInfoDto;
using MCA.SharedKernel.Application.Contracts;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityRequests;

/// <summary>
/// Handler for getting facility requests
/// </summary>
public class GetFacilityRequestsQueryHandler : IRequestHandler<GetFacilityRequestsByUserQuery, ApplicationResult<GetFacilityRequestsResult>>
{
    private readonly IFacilityRequestRepository _requestRepository;
    private readonly IMapper<Domain.Entities.FacilityRequest, FacilityRequestDto> _requestMapper;
    private readonly IValidator<GetFacilityRequestsByUserQuery> _validator;
    private readonly ILogger<GetFacilityRequestsQueryHandler> _logger;
    private readonly IMemberInfoService _memberInfoService;

    public GetFacilityRequestsQueryHandler(
        IFacilityRequestRepository requestRepository,
        IValidator<GetFacilityRequestsByUserQuery> validator,
        ILogger<GetFacilityRequestsQueryHandler> logger,
        IMemberInfoService memberInfoService,
        IMapper<Domain.Entities.FacilityRequest, FacilityRequestDto> requestMapper)
    {
        _requestRepository = requestRepository;
        _validator = validator;
        _logger = logger;
        _memberInfoService = memberInfoService;
        _requestMapper = requestMapper;
    }

    public async Task<ApplicationResult<GetFacilityRequestsResult>> Handle(
        GetFacilityRequestsByUserQuery requestByUser,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate input
            var validationResult = await _validator.ValidateAsync(requestByUser, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ApplicationResult<GetFacilityRequestsResult>.Failure(
                    errors, 
                    "اطلاعات ورودی نامعتبر است");
            }

            // Build spec like facilities
            Guid? memberId = null;

            // If NationalNumber is provided, get the MemberId
            if (!string.IsNullOrWhiteSpace(requestByUser.NationalNumber))
            {
                var nationalId = new NationalId(requestByUser.NationalNumber);
                var memberInfo = await _memberInfoService.GetMemberInfoAsync(nationalId);
                if (memberInfo == null)
                {
                    return ApplicationResult<GetFacilityRequestsResult>.Failure(
                        "عضو یافت نشد");
                }
                memberId = memberInfo.Id;
            }

            var spec = new Specifications.FacilityRequestPaginatedSpec(
                requestByUser.Page,
                requestByUser.PageSize,
                requestByUser.FacilityId,
                requestByUser.FacilityCycleId,
                memberId,
                requestByUser.Status,
                requestByUser.SearchTerm,
                requestByUser.DateFrom,
                requestByUser.DateTo);

            // Get requests (if repo supports spec, else fallback to parameters)
            var requests = await _requestRepository.GetFacilityRequestsAsync(spec.ToParameters(), cancellationToken);
            var totalCount = await _requestRepository.GetFacilityRequestsCountAsync(spec.ToParameters(), cancellationToken);

            // Map to DTOs via DI mapper
            var requestDtos = (await Task.WhenAll(requests.Select(r => _requestMapper.MapAsync(r, cancellationToken)))).ToList();

            var totalPages = (int)Math.Ceiling((double)totalCount / requestByUser.PageSize);

            _logger.LogInformation("Retrieved {Count} facility requests for page {Page}",
                requestDtos.Count, requestByUser.Page);

            return ApplicationResult<GetFacilityRequestsResult>.Success(
                new GetFacilityRequestsResult
                {
                    Items = requestDtos,
                    TotalCount = totalCount,
                    PageNumber = requestByUser.Page,
                    PageSize = requestByUser.PageSize
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving facility requests");
            return ApplicationResult<GetFacilityRequestsResult>.Failure(
                "خطای داخلی در دریافت لیست درخواست‌های تسهیلات");
        }
    }

}

