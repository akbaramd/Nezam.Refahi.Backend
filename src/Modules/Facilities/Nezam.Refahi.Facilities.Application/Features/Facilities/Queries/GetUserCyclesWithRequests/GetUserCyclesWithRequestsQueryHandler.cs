using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserCycleRequests;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserMemberInfo;
using Nezam.Refahi.Facilities.Application.Services;
using Nezam.Refahi.Facilities.Domain.Enums;
using Nezam.Refahi.Facilities.Domain.Repositories;
using Nezam.Refahi.Membership.Contracts.Dtos;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserCyclesWithRequests;

/// <summary>
/// Handler for getting cycles where user has requests with detailed request information
/// </summary>
public class GetUserCyclesWithRequestsQueryHandler : IRequestHandler<GetUserCyclesWithRequestsQuery, ApplicationResult<GetUserCyclesWithRequestsResponse>>
{
    private readonly IFacilityRequestRepository _requestRepository;
    private readonly IFacilityRepository _facilityRepository;
    private readonly IFacilityCycleRepository _cycleRepository;
    private readonly IMemberInfoService _memberInfoService;
    private readonly IValidator<GetUserCyclesWithRequestsQuery> _validator;
    private readonly ILogger<GetUserCyclesWithRequestsQueryHandler> _logger;

    public GetUserCyclesWithRequestsQueryHandler(
        IFacilityRequestRepository requestRepository,
        IFacilityRepository facilityRepository,
        IFacilityCycleRepository cycleRepository,
        IMemberInfoService memberInfoService,
        IValidator<GetUserCyclesWithRequestsQuery> validator,
        ILogger<GetUserCyclesWithRequestsQueryHandler> logger)
    {
        _requestRepository = requestRepository;
        _facilityRepository = facilityRepository;
        _cycleRepository = cycleRepository;
        _memberInfoService = memberInfoService;
        _validator = validator;
        _logger = logger;
    }

    public async Task<ApplicationResult<GetUserCyclesWithRequestsResponse>> Handle(
        GetUserCyclesWithRequestsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate input
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ApplicationResult<GetUserCyclesWithRequestsResponse>.Failure(
                    errors,
                    "اطلاعات ورودی نامعتبر است");
            }

            // Get user member info
            var memberInfo = await _memberInfoService.GetMemberInfoAsync(new NationalId(request.NationalNumber));
            if (memberInfo == null)
            {
                return ApplicationResult<GetUserCyclesWithRequestsResponse>.Failure(
                    "اطلاعات عضو یافت نشد");
            }

            // Get user requests with optimized filtering
            var queryParameters = new FacilityRequestQueryParameters
            {
                MemberId = memberInfo.Id,
                FacilityId = request.FacilityId,
                Status = request.RequestStatus,
                Page = request.Page,
                PageSize = request.PageSize
            };

            // Apply status category filter if specified
            if (request.RequestStatusCategory.HasValue)
            {
                var statusFilter = MapStatusCategoryToStatus(request.RequestStatusCategory.Value);
                if (!string.IsNullOrWhiteSpace(statusFilter))
                {
                    queryParameters.Status = statusFilter.Split(',').FirstOrDefault()?.Trim();
                }
            }

            var paginatedRequests = await _requestRepository.GetFacilityRequestsAsync(queryParameters, cancellationToken);
            var totalCount = await _requestRepository.GetFacilityRequestsCountAsync(queryParameters, cancellationToken);

            // Map requests to cycle DTOs
            var cycleDtos = new List<UserCycleWithRequestDto>();
            foreach (var facilityRequest in paginatedRequests)
            {
                var cycleDto = await MapToUserCycleWithRequestDto(facilityRequest, request, cancellationToken);
                cycleDtos.Add(cycleDto);
            }

            // Calculate statistics (get all user requests for statistics)
            var allUserRequests = await _requestRepository.GetByUserIdAsync(memberInfo.Id, cancellationToken);
            var statistics = CalculateUserCyclesWithRequestsStatistics(allUserRequests);

            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

            _logger.LogInformation("Retrieved {Count} cycles with requests for user {NationalNumber}",
                cycleDtos.Count, request.NationalNumber);

            return ApplicationResult<GetUserCyclesWithRequestsResponse>.Success(new GetUserCyclesWithRequestsResponse
            {
                UserInfo = MapMemberInfoToDto(memberInfo),
                Cycles = cycleDtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages,
                Summary = statistics
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user cycles with requests for user {NationalNumber}",
                request.NationalNumber);
            return ApplicationResult<GetUserCyclesWithRequestsResponse>.Failure(
                "خطای داخلی در دریافت دوره‌های کاربر با درخواست‌ها");
        }
    }

    private async Task<UserCycleWithRequestDto> MapToUserCycleWithRequestDto(
        Domain.Entities.FacilityRequest facilityRequest,
        GetUserCyclesWithRequestsQuery request,
        CancellationToken cancellationToken)
    {
        // Get cycle info
        var cycle = await _cycleRepository.GetByIdAsync(facilityRequest.FacilityCycleId, cancellationToken);
        if (cycle == null)
        {
            throw new InvalidOperationException($"Cycle {facilityRequest.FacilityCycleId} not found");
        }

        // Get facility info if requested
        Domain.Entities.Facility? facility = null;
        if (request.IncludeFacilityInfo)
        {
            facility = await _facilityRepository.GetByIdAsync(facilityRequest.FacilityId, cancellationToken);
        }

        var now = DateTime.UtcNow;
        var daysUntilStart = (cycle.StartDate - now).Days;
        var daysUntilEnd = (cycle.EndDate - now).Days;
        var hasStarted = now >= cycle.StartDate;
        var hasEnded = now >= cycle.EndDate;
        var isActive = cycle.Status == FacilityCycleStatus.Active && hasStarted && !hasEnded;
        var isAcceptingApplications = isActive && cycle.UsedQuota < cycle.Quota;
        var quotaUtilizationPercentage = cycle.Quota > 0 ? (decimal)cycle.UsedQuota / cycle.Quota * 100 : 0;

        return new UserCycleWithRequestDto
        {
            Id = cycle.Id,
            Name = cycle.Name,
            StartDate = cycle.StartDate,
            EndDate = cycle.EndDate,
            DaysUntilStart = Math.Max(0, daysUntilStart),
            DaysUntilEnd = Math.Max(0, daysUntilEnd),
            HasStarted = hasStarted,
            HasEnded = hasEnded,
            IsActive = isActive,
            IsAcceptingApplications = isAcceptingApplications,
            Quota = cycle.Quota,
            UsedQuota = cycle.UsedQuota,
            AvailableQuota = cycle.Quota - cycle.UsedQuota,
            QuotaUtilizationPercentage = quotaUtilizationPercentage,
            Status = cycle.Status.ToString(),
            StatusDescription = EnumTextMappingService.GetFacilityCycleStatusDescription(cycle.Status),
            Description = cycle.Description,
            Facility = facility != null ? MapFacilityToDto(facility) : null,
            UserRequest = MapToUserRequestDetailsDto(facilityRequest),
            CreatedAt = cycle.CreatedAt
        };
    }

    private UserRequestDetailsDto MapToUserRequestDetailsDto(Domain.Entities.FacilityRequest request)
    {
        var now = DateTime.UtcNow;
        var daysSinceCreated = (now - request.CreatedAt).Days;

        return new UserRequestDetailsDto
        {
            RequestId = request.Id,
            Status = request.Status.ToString(),
            StatusDescription = EnumTextMappingService.GetFacilityRequestStatusDescription(request.Status),
            RequestedAmountRials = request.RequestedAmount.AmountRials,
            ApprovedAmountRials = request.ApprovedAmount?.AmountRials,
            Currency = request.RequestedAmount.Currency,
            FormattedRequestedAmount = FormatAmount(request.RequestedAmount.AmountRials),
            FormattedApprovedAmount = request.ApprovedAmount?.AmountRials != null 
                ? FormatAmount(request.ApprovedAmount.AmountRials) 
                : null,
            CreatedAt = request.CreatedAt,
            ApprovedAt = request.ApprovedAt,
            RejectedAt = request.RejectedAt,
            RejectionReason = request.RejectionReason,
            DaysSinceCreated = daysSinceCreated,
            IsInProgress = EnumTextMappingService.IsRequestInProgress(request.Status),
            IsCompleted = EnumTextMappingService.IsRequestCompleted(request.Status),
            IsRejected = EnumTextMappingService.IsRequestRejected(request.Status),
            IsCancelled = EnumTextMappingService.IsRequestCancelled(request.Status),
            Timeline = MapTimelineToDto(request)
        };
    }

    private RequestTimelineDto MapTimelineToDto(Domain.Entities.FacilityRequest request)
    {
        var now = DateTime.UtcNow;
        var daysSinceCreated = (now - request.CreatedAt).Days;
        var daysUntilBankAppointment = request.BankAppointmentScheduledAt.HasValue
            ? (int?)(request.BankAppointmentScheduledAt.Value - now).Days
            : null;
        var isBankAppointmentOverdue = request.BankAppointmentScheduledAt.HasValue
            && request.BankAppointmentScheduledAt.Value < now
            && !request.BankAppointmentDate.HasValue;

        var processingTimeDays = request.CompletedAt.HasValue
            ? (int?)(request.CompletedAt.Value - request.CreatedAt).Days
            : null;

        return new RequestTimelineDto
        {
            CreatedAt = request.CreatedAt,
            ApprovedAt = request.ApprovedAt,
            RejectedAt = request.RejectedAt,
            BankAppointmentScheduledAt = request.BankAppointmentScheduledAt,
            BankAppointmentDate = request.BankAppointmentDate,
            DisbursedAt = request.DisbursedAt,
            CompletedAt = request.CompletedAt,
            DaysSinceCreated = daysSinceCreated,
            DaysUntilBankAppointment = daysUntilBankAppointment,
            IsBankAppointmentOverdue = isBankAppointmentOverdue,
            ProcessingTimeDays = processingTimeDays
        };
    }

    private FacilityInfoDto MapFacilityToDto(Domain.Entities.Facility facility)
    {
        return new FacilityInfoDto
        {
            Id = facility.Id,
            Name = facility.Name,
            Code = facility.Code,
            Type = facility.Type.ToString(),
            TypeText = EnumTextMappingService.GetFacilityTypeDescription(facility.Type),
            Status = facility.Status.ToString(),
            StatusText = EnumTextMappingService.GetFacilityStatusDescription(facility.Status),
            Description = facility.Description,
            BankInfo = new BankInfoDto
            {
                BankName = facility.BankName,
                BankCode = facility.BankCode,
                BankAccountNumber = facility.BankAccountNumber
            },
            IsActive = EnumTextMappingService.IsFacilityActive(facility.Status)
        };
    }

    private UserMemberInfoDto MapMemberInfoToDto(MemberInfoDto memberInfo)
    {
        return new UserMemberInfoDto
        {
            Id = memberInfo.Id,
            NationalId = memberInfo.NationalCode,
            FullName = memberInfo.FullName,
            PhoneNumber = memberInfo.PhoneNumber,
        };
    }

    private UserCyclesWithRequestsSummaryDto CalculateUserCyclesWithRequestsStatistics(IEnumerable<Domain.Entities.FacilityRequest> allRequests)
    {
        var requests = allRequests.ToList();
        var totalRequests = requests.Count;

        var inProgressRequests = requests.Count(r => EnumTextMappingService.IsRequestInProgress(r.Status));
        var completedRequests = requests.Count(r => EnumTextMappingService.IsRequestCompleted(r.Status));
        var rejectedRequests = requests.Count(r => EnumTextMappingService.IsRequestRejected(r.Status));

        var totalRequestedAmountRials = requests.Sum(r => r.RequestedAmount.AmountRials);
        var totalApprovedAmountRials = requests
            .Where(r => r.ApprovedAmount != null)
            .Sum(r => r.ApprovedAmount!.AmountRials);

        var differentFacilitiesCount = requests.Select(r => r.FacilityId).Distinct().Count();
        var differentCyclesCount = requests.Select(r => r.FacilityCycleId).Distinct().Count();

        var mostRecentRequestDate = requests.Any() ? requests.Max(r => r.CreatedAt) : (DateTime?)null;
        var oldestRequestDate = requests.Any() ? requests.Min(r => r.CreatedAt) : (DateTime?)null;

        return new UserCyclesWithRequestsSummaryDto
        {
            TotalCycles = differentCyclesCount,
            ActiveCycles = 0, // Will be calculated separately if needed
            CyclesWithInProgressRequests = requests.Where(r => EnumTextMappingService.IsRequestInProgress(r.Status))
                .Select(r => r.FacilityCycleId).Distinct().Count(),
            CyclesWithCompletedRequests = requests.Where(r => EnumTextMappingService.IsRequestCompleted(r.Status))
                .Select(r => r.FacilityCycleId).Distinct().Count(),
            CyclesWithRejectedRequests = requests.Where(r => EnumTextMappingService.IsRequestRejected(r.Status))
                .Select(r => r.FacilityCycleId).Distinct().Count(),
            TotalRequestedAmountRials = totalRequestedAmountRials,
            TotalApprovedAmountRials = totalApprovedAmountRials,
            FormattedTotalRequestedAmount = FormatAmount(totalRequestedAmountRials),
            FormattedTotalApprovedAmount = FormatAmount(totalApprovedAmountRials),
            DifferentFacilitiesCount = differentFacilitiesCount,
            MostRecentRequestDate = mostRecentRequestDate,
            OldestRequestDate = oldestRequestDate
        };
    }

    private string? MapStatusCategoryToStatus(RequestStatusCategory category)
    {
        return category switch
        {
            RequestStatusCategory.InProgress => "PendingApproval,PendingDocuments,Waitlisted,ReturnedForAmendment,UnderReview,QueuedForDispatch,SentToBank,BankScheduled,ProcessedByBank",
            RequestStatusCategory.Completed => "Completed,Disbursed",
            RequestStatusCategory.Rejected => "Rejected,Expired,BankCancelled",
            RequestStatusCategory.Cancelled => "Cancelled",
            RequestStatusCategory.Terminal => "Approved,Rejected,Cancelled,Completed,Disbursed,Expired,BankCancelled",
            _ => null
        };
    }

    private string FormatAmount(decimal amount)
    {
        return $"{amount:N0} ریال";
    }
}
