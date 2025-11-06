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
    private readonly IMemberService _memberService;

    public GetFacilityRequestsByUserQueryHandler(
        IFacilityRequestRepository requestRepository,
        IValidator<GetFacilityRequestsByUserQuery> validator,
        ILogger<GetFacilityRequestsByUserQueryHandler> logger,
        IMemberService memberService,
        IMapper<Domain.Entities.FacilityRequest, FacilityRequestDto> requestMapper)
    {
        _requestRepository = requestRepository;
        _validator = validator;
        _logger = logger;
        _memberService = memberService;
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
                var memberDetail = await _memberService.GetMemberDetailByNationalCodeAsync(nationalId);
                if (memberDetail == null)
                {
                    return ApplicationResult<GetFacilityRequestsByUserQueryResult>.Failure(
                        "عضو یافت نشد");
                }
                memberId = memberDetail.Id;
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

            // Determine which requests are the last request for their respective cycles
            // Use the already-loaded requests to avoid N+1 queries
            Dictionary<Guid, Guid> lastRequestByCycle = new();
            if (memberId.HasValue && requests.Items.Any())
            {
                // Group by cycle and find the latest request for each cycle from the loaded data
                var requestsByCycle = requests.Items
                    .GroupBy(r => r.FacilityCycleId)
                    .ToDictionary(g => g.Key, g => g.OrderByDescending(r => r.CreatedAt).First());

                foreach (var kvp in requestsByCycle)
                {
                    lastRequestByCycle[kvp.Key] = kvp.Value.Id;
                }
            }

            // Map to DTOs via DI mapper
            var requestDtos = (await Task.WhenAll(requests.Items.Select(r => _requestMapper.MapAsync(r, cancellationToken)))).ToList();

            // Mark requests as last request if they match
            if (memberId.HasValue && requestDtos.Any() && lastRequestByCycle.Any())
            {
                foreach (var dto in requestDtos)
                {
                    // Get cycle ID from the source entity since mapper might not have populated Cycle yet
                    var sourceRequest = requests.Items.FirstOrDefault(r => r.Id == dto.Id);
                    if (sourceRequest != null && lastRequestByCycle.TryGetValue(sourceRequest.FacilityCycleId, out var lastRequestId) && dto.Id == lastRequestId)
                    {
                        dto.IsLastRequest = true;
                    }
                }
            }

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
            return ApplicationResult<GetFacilityRequestsByUserQueryResult>.Failure(ex,
                "خطای داخلی در دریافت لیست درخواست‌های تسهیلات");
        }
    }

}

