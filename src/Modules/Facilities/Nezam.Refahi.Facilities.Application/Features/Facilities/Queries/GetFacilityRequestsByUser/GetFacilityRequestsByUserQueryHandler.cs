using FluentValidation;
using MCA.SharedKernel.Application.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Facilities.Application.Dtos;
using Nezam.Refahi.Facilities.Application.Spesifications;
using Nezam.Refahi.Facilities.Domain.Repositories;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityRequestsByUser;

/// <summary>
/// Handler for getting facility requests
/// </summary>
public class GetFacilityRequestsByUserQueryHandler : IRequestHandler<GetFacilityRequestsByUserQuery, ApplicationResult<GetFacilityRequestsByUserQueryResult>>
{
    private readonly IFacilityRequestRepository _requestRepository;
    private readonly IMapper<Domain.Entities.FacilityRequest, FacilityRequestDto> _requestMapper;
    private readonly IValidator<GetFacilityRequestsByUserQuery> _validator;
    private readonly ILogger<GetFacilityRequestsByUserQueryHandler> _logger;
    private readonly IMemberInfoService _memberInfoService;

    public GetFacilityRequestsByUserQueryHandler(
        IFacilityRequestRepository requestRepository,
        IValidator<GetFacilityRequestsByUserQuery> validator,
        ILogger<GetFacilityRequestsByUserQueryHandler> logger,
        IMemberInfoService memberInfoService,
        IMapper<Domain.Entities.FacilityRequest, FacilityRequestDto> requestMapper)
    {
        _requestRepository = requestRepository;
        _validator = validator;
        _logger = logger;
        _memberInfoService = memberInfoService;
        _requestMapper = requestMapper;
    }

    public async Task<ApplicationResult<GetFacilityRequestsByUserQueryResult>> Handle(
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
                return ApplicationResult<GetFacilityRequestsByUserQueryResult>.Failure(
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
                    return ApplicationResult<GetFacilityRequestsByUserQueryResult>.Failure(
                        "عضو یافت نشد");
                }
                memberId = memberInfo.Id;
            }

            var spec = new FacilityRequestPaginatedSpec(
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
            var requests = await _requestRepository.GetPaginatedAsync(spec, cancellationToken);

            // Map to DTOs via DI mapper
            var requestDtos = (await Task.WhenAll(requests.Items.Select(r => _requestMapper.MapAsync(r, cancellationToken)))).ToList();

            _logger.LogInformation("Retrieved {Count} facility requests for page {Page}",
                requestDtos.Count, requestByUser.Page);

            return ApplicationResult<GetFacilityRequestsByUserQueryResult>.Success(
                new GetFacilityRequestsByUserQueryResult
                {
                    Items = requestDtos,
                    TotalCount = requests.TotalCount,
                    PageNumber = requestByUser.Page,
                    PageSize = requestByUser.PageSize
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving facility requests");
            return ApplicationResult<GetFacilityRequestsByUserQueryResult>.Failure(
                "خطای داخلی در دریافت لیست درخواست‌های تسهیلات");
        }
    }

}

