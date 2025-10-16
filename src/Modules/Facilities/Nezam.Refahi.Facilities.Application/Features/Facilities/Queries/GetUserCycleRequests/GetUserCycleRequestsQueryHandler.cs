using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycles;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityRequests;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserMemberInfo;
using Nezam.Refahi.Facilities.Application.Services;
using Nezam.Refahi.Facilities.Domain.Enums;
using Nezam.Refahi.Facilities.Domain.Repositories;
using Nezam.Refahi.Membership.Contracts.Dtos;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserCycleRequests;

/// <summary>
/// Handler for getting user's cycle requests
/// </summary>
public class GetUserCycleRequestsQueryHandler : IRequestHandler<GetUserCycleRequestsQuery, ApplicationResult<GetUserCycleRequestsResponse>>
{
    private readonly IFacilityRequestRepository _requestRepository;
    private readonly IFacilityRepository _facilityRepository;
    private readonly IFacilityCycleRepository _cycleRepository;
    private readonly IMemberInfoService _memberInfoService;
    private readonly IValidator<GetUserCycleRequestsQuery> _validator;
    private readonly ILogger<GetUserCycleRequestsQueryHandler> _logger;

    public GetUserCycleRequestsQueryHandler(
        IFacilityRequestRepository requestRepository,
        IFacilityRepository facilityRepository,
        IFacilityCycleRepository cycleRepository,
        IMemberInfoService memberInfoService,
        IValidator<GetUserCycleRequestsQuery> validator,
        ILogger<GetUserCycleRequestsQueryHandler> logger)
    {
        _requestRepository = requestRepository;
        _facilityRepository = facilityRepository;
        _cycleRepository = cycleRepository;
        _memberInfoService = memberInfoService;
        _validator = validator;
        _logger = logger;
    }

    public async Task<ApplicationResult<GetUserCycleRequestsResponse>> Handle(
        GetUserCycleRequestsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate input
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ApplicationResult<GetUserCycleRequestsResponse>.Failure(
                    errors,
                    "اطلاعات ورودی نامعتبر است");
            }

            // Get user member info
            var memberInfo = await _memberInfoService.GetMemberInfoAsync(new NationalId(request.NationalNumber));
            if (memberInfo == null)
            {
                return ApplicationResult<GetUserCycleRequestsResponse>.Failure(
                    "اطلاعات عضو یافت نشد");
            }

            // Get all user requests
            var allRequests = await _requestRepository.GetByUserIdAsync(memberInfo.Id, cancellationToken);
            
            // Apply filtering
            var filteredRequests = allRequests.AsEnumerable();
            
            if (request.FacilityId.HasValue)
            {
                filteredRequests = filteredRequests.Where(r => r.FacilityId == request.FacilityId.Value);
            }
            
            if (request.FacilityCycleId.HasValue)
            {
                filteredRequests = filteredRequests.Where(r => r.FacilityCycleId == request.FacilityCycleId.Value);
            }
            
            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                if (Enum.TryParse<Domain.Enums.FacilityRequestStatus>(request.Status, true, out var statusEnum))
                {
                    filteredRequests = filteredRequests.Where(r => r.Status == statusEnum);
                }
            }
            
            if (request.StatusCategory.HasValue)
            {
                var statusFilter = MapStatusCategoryToStatus(request.StatusCategory.Value);
                if (!string.IsNullOrWhiteSpace(statusFilter))
                {
                    var statuses = statusFilter.Split(',').Select(s => s.Trim()).ToList();
                    filteredRequests = filteredRequests.Where(r => statuses.Contains(r.Status.ToString()));
                }
            }
            
            if (request.DateFrom.HasValue)
            {
                filteredRequests = filteredRequests.Where(r => r.CreatedAt >= request.DateFrom.Value);
            }
            
            if (request.DateTo.HasValue)
            {
                filteredRequests = filteredRequests.Where(r => r.CreatedAt <= request.DateTo.Value);
            }
            
            var requestsList = filteredRequests.ToList();
            var totalCount = requestsList.Count;
            
            // Apply pagination
            var paginatedRequests = requestsList
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Map requests to DTOs
            var requestDtos = new List<FacilityRequestDto>();
            foreach (var facilityRequest in paginatedRequests)
            {
                var requestDto = await MapToFacilityRequestDto(facilityRequest, request, cancellationToken);
                requestDtos.Add(requestDto);
            }

            // Calculate statistics
            var statistics = CalculateUserRequestStatistics(requestsList);

            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

            _logger.LogInformation("Retrieved {Count} cycle requests for user {NationalNumber}",
                requestDtos.Count, request.NationalNumber);

            return ApplicationResult<GetUserCycleRequestsResponse>.Success(new GetUserCycleRequestsResponse
            {
                UserInfo = MapMemberInfoToDto(memberInfo),
                Requests = requestDtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages,
                Statistics = statistics
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user cycle requests for user {NationalNumber}",
                request.NationalNumber);
            return ApplicationResult<GetUserCycleRequestsResponse>.Failure(
                "خطای داخلی در دریافت درخواست‌های دوره کاربر");
        }
    }

    private async Task<FacilityRequestDto> MapToFacilityRequestDto(
        Domain.Entities.FacilityRequest facilityRequest,
        GetUserCycleRequestsQuery request,
        CancellationToken cancellationToken)
    {
        // Get facility info if requested
        Domain.Entities.Facility? facility = null;
        if (request.IncludeFacilityInfo)
        {
            facility = await _facilityRepository.GetByIdAsync(facilityRequest.FacilityId, cancellationToken);
        }

        // Get cycle info if requested
        Domain.Entities.FacilityCycle? cycle = null;
        if (request.IncludeCycleInfo)
        {
            cycle = await _cycleRepository.GetByIdAsync(facilityRequest.FacilityCycleId, cancellationToken);
        }

        var now = DateTime.UtcNow;
        var daysSinceCreated = (now - facilityRequest.CreatedAt).Days;

        return new FacilityRequestDto
        {
            Id = facilityRequest.Id,
            Facility = facility != null ? MapFacilityToDto(facility) : new FacilityInfoDto
            {
                Id = facilityRequest.FacilityId,
                Name = "Unknown",
                Code = "Unknown",
                Type = "Unknown",
                TypeText = "نامشخص",
                Status = "Unknown",
                StatusText = "نامشخص",
                Description = null,
                BankInfo = null,
                IsActive = false
            },
            Cycle = cycle != null ? MapCycleToDto(cycle) : new FacilityCycleDto
            {
                Id = facilityRequest.FacilityCycleId,
                Name = "Unknown",
                StartDate = DateTime.MinValue,
                EndDate = DateTime.MinValue,
                DaysUntilEnd = 0,
                IsActive = false,
                Quota = 0,
                UsedQuota = 0,
                AvailableQuota = 0,
                Status = "Unknown",
                StatusText = "نامشخص",
                MinAmountRials = 0,
                MaxAmountRials = 0,
                PaymentMonths = 0,
                CooldownDays = 0,
                CreatedAt = facilityRequest.CreatedAt
            },
            Applicant = new ApplicantInfoDto
            {
                MemberId = facilityRequest.MemberId,
                FullName = facilityRequest.UserFullName,
                NationalId = facilityRequest.UserNationalId
            },
            RequestedAmountRials = facilityRequest.RequestedAmount.AmountRials,
            ApprovedAmountRials = facilityRequest.ApprovedAmount?.AmountRials,
            Currency = facilityRequest.RequestedAmount.Currency,
            Status = facilityRequest.Status.ToString(),
            StatusText = EnumTextMappingService.GetFacilityRequestStatusDescription(facilityRequest.Status),
            CreatedAt = facilityRequest.CreatedAt,
            DaysSinceCreated = daysSinceCreated,
            IsInProgress = EnumTextMappingService.IsRequestInProgress(facilityRequest.Status),
            IsCompleted = EnumTextMappingService.IsRequestCompleted(facilityRequest.Status),
            IsRejected = EnumTextMappingService.IsRequestRejected(facilityRequest.Status),
            IsCancelled = EnumTextMappingService.IsRequestCancelled(facilityRequest.Status)
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
            IsActive = EnumTextMappingService.IsFacilityActive(facility.Status)
        };
    }

    private FacilityCycleDto MapCycleToDto(Domain.Entities.FacilityCycle cycle)
    {
        var now = DateTime.UtcNow;
        var daysUntilEnd = (cycle.EndDate - now).Days;
        var isActive = cycle.Status == FacilityCycleStatus.Active && now >= cycle.StartDate && now < cycle.EndDate;

        return new FacilityCycleDto
        {
            Id = cycle.Id,
            Name = cycle.Name,
            StartDate = cycle.StartDate,
            EndDate = cycle.EndDate,
            DaysUntilEnd = Math.Max(0, daysUntilEnd),
            IsActive = isActive,
            Quota = cycle.Quota,
            UsedQuota = cycle.UsedQuota,
            AvailableQuota = cycle.Quota - cycle.UsedQuota,
            Status = cycle.Status.ToString(),
            StatusText = EnumTextMappingService.GetFacilityCycleStatusDescription(cycle.Status),
            MinAmountRials = cycle.MinAmount?.AmountRials ?? 0,
            MaxAmountRials = cycle.MaxAmount?.AmountRials ?? 0,
            PaymentMonths = cycle.PaymentMonths,
            CooldownDays = cycle.CooldownDays,
            CreatedAt = cycle.CreatedAt
        };
    }

    private RequestStatusDto MapRequestStatusToDto(Domain.Entities.FacilityRequest request)
    {
        return new RequestStatusDto
        {
            Status = request.Status.ToString(),
            StatusDescription = EnumTextMappingService.GetFacilityRequestStatusDescription(request.Status),
            Description = request.Description,
            RejectionReason = request.RejectionReason,
            IsTerminal = EnumTextMappingService.IsRequestTerminal(request.Status),
            CanBeCancelled = EnumTextMappingService.CanRequestBeCancelled(request.Status),
            IsInProgress = EnumTextMappingService.IsRequestInProgress(request.Status),
            IsCompleted = EnumTextMappingService.IsRequestCompleted(request.Status),
            IsRejected = EnumTextMappingService.IsRequestRejected(request.Status),
            IsCancelled = EnumTextMappingService.IsRequestCancelled(request.Status),
            RequiresApplicantAction = EnumTextMappingService.RequiresApplicantAction(request.Status),
            RequiresBankAction = EnumTextMappingService.RequiresBankAction(request.Status)
        };
    }

    private RequestFinancialInfoDto MapFinancialInfoToDto(Domain.Entities.FacilityRequest request)
    {
        return new RequestFinancialInfoDto
        {
            RequestedAmountRials = request.RequestedAmount.AmountRials,
            ApprovedAmountRials = request.ApprovedAmount?.AmountRials,
            Currency = request.RequestedAmount.Currency,
            FormattedRequestedAmount = FormatAmount(request.RequestedAmount.AmountRials),
            FormattedApprovedAmount = request.ApprovedAmount?.AmountRials != null 
                ? FormatAmount(request.ApprovedAmount.AmountRials) 
                : null,
            FormattedFinalAmount = FormatAmount(request.ApprovedAmount?.AmountRials ?? request.RequestedAmount.AmountRials)
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

    private UserRequestStatisticsDto CalculateUserRequestStatistics(IEnumerable<Domain.Entities.FacilityRequest> allRequests)
    {
        var requests = allRequests.ToList();
        var totalRequests = requests.Count;

        var inProgressRequests = requests.Count(r => EnumTextMappingService.IsRequestInProgress(r.Status));
        var completedRequests = requests.Count(r => EnumTextMappingService.IsRequestCompleted(r.Status));
        var rejectedRequests = requests.Count(r => EnumTextMappingService.IsRequestRejected(r.Status));
        var cancelledRequests = requests.Count(r => EnumTextMappingService.IsRequestCancelled(r.Status));
        var terminalRequests = requests.Count(r => EnumTextMappingService.IsRequestTerminal(r.Status));

        var successRatePercentage = totalRequests > 0 
            ? (decimal)completedRequests / totalRequests * 100 
            : 0;

        var completedRequestsWithTime = requests
            .Where(r => EnumTextMappingService.IsRequestCompleted(r.Status) && r.CompletedAt.HasValue)
            .ToList();

        var averageProcessingTimeDays = completedRequestsWithTime.Any()
            ? (decimal?)completedRequestsWithTime.Average(r => (r.CompletedAt!.Value - r.CreatedAt).TotalDays)
            : null;

        var totalRequestedAmountRials = requests.Sum(r => r.RequestedAmount.AmountRials);
        var totalApprovedAmountRials = requests
            .Where(r => r.ApprovedAmount != null)
            .Sum(r => r.ApprovedAmount!.AmountRials);

        var differentFacilitiesCount = requests.Select(r => r.FacilityId).Distinct().Count();
        var differentCyclesCount = requests.Select(r => r.FacilityCycleId).Distinct().Count();

        var mostRecentRequestDate = requests.Any() ? requests.Max(r => r.CreatedAt) : (DateTime?)null;
        var oldestRequestDate = requests.Any() ? requests.Min(r => r.CreatedAt) : (DateTime?)null;

        return new UserRequestStatisticsDto
        {
            TotalRequests = totalRequests,
            InProgressRequests = inProgressRequests,
            CompletedRequests = completedRequests,
            RejectedRequests = rejectedRequests,
            CancelledRequests = cancelledRequests,
            TerminalRequests = terminalRequests,
            SuccessRatePercentage = successRatePercentage,
            AverageProcessingTimeDays = averageProcessingTimeDays,
            TotalRequestedAmountRials = totalRequestedAmountRials,
            TotalApprovedAmountRials = totalApprovedAmountRials,
            FormattedTotalRequestedAmount = FormatAmount(totalRequestedAmountRials),
            FormattedTotalApprovedAmount = FormatAmount(totalApprovedAmountRials),
            DifferentFacilitiesCount = differentFacilitiesCount,
            DifferentCyclesCount = differentCyclesCount,
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
