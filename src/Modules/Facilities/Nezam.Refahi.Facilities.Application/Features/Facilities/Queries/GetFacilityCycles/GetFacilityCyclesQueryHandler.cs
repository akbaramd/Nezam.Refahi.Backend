using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserCycleRequests;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserMemberInfo;
using Nezam.Refahi.Facilities.Application.Services;
using Nezam.Refahi.Facilities.Domain.Enums;
using Nezam.Refahi.Facilities.Domain.Repositories;
using Nezam.Refahi.Facilities.Domain.Services;
using Nezam.Refahi.Membership.Contracts.Dtos;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycles;

/// <summary>
/// Handler for getting facility cycles with user context
/// </summary>
public class GetFacilityCyclesQueryHandler : IRequestHandler<GetFacilityCyclesQuery, ApplicationResult<GetFacilityCyclesResponse>>
{
    private readonly IFacilityRepository _facilityRepository;
    private readonly IFacilityCycleRepository _cycleRepository;
    private readonly IFacilityRequestRepository _requestRepository;
    private readonly IMemberInfoService _memberInfoService;
    private readonly FacilityEligibilityDomainService _eligibilityService;
    private readonly IValidator<GetFacilityCyclesQuery> _validator;
    private readonly ILogger<GetFacilityCyclesQueryHandler> _logger;

    public GetFacilityCyclesQueryHandler(
        IFacilityRepository facilityRepository,
        IFacilityCycleRepository cycleRepository,
        IFacilityRequestRepository requestRepository,
        IMemberInfoService memberInfoService,
        FacilityEligibilityDomainService eligibilityService,
        IValidator<GetFacilityCyclesQuery> validator,
        ILogger<GetFacilityCyclesQueryHandler> logger)
    {
        _facilityRepository = facilityRepository;
        _cycleRepository = cycleRepository;
        _requestRepository = requestRepository;
        _memberInfoService = memberInfoService;
        _eligibilityService = eligibilityService;
        _validator = validator;
        _logger = logger;
    }

    public async Task<ApplicationResult<GetFacilityCyclesResponse>> Handle(
        GetFacilityCyclesQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate input
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ApplicationResult<GetFacilityCyclesResponse>.Failure(
                    errors,
                    "اطلاعات ورودی نامعتبر است");
            }

            // Get facility
            var facility = await _facilityRepository.GetByIdAsync(request.FacilityId, cancellationToken);
            if (facility == null)
            {
                return ApplicationResult<GetFacilityCyclesResponse>.Failure(
                    "تسهیلات مورد نظر یافت نشد");
            }

            // Get user member info if NationalNumber provided
            MemberInfoDto? memberInfo = null;
            if (!string.IsNullOrWhiteSpace(request.NationalNumber))
            {
                memberInfo = await _memberInfoService.GetMemberInfoAsync(new NationalId(request.NationalNumber));
                if (memberInfo == null)
                {
                    return ApplicationResult<GetFacilityCyclesResponse>.Failure(
                        "اطلاعات عضو یافت نشد");
                }
            }

            // Get cycles by facility ID
            var cycles = await _cycleRepository.GetByFacilityIdAsync(request.FacilityId, cancellationToken);
            
            // Apply filtering
            var filteredCycles = cycles.AsEnumerable();
            
            if (request.OnlyActive)
            {
                filteredCycles = filteredCycles.Where(c => c.Status == Domain.Enums.FacilityCycleStatus.Active);
            }
            
            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                if (Enum.TryParse<Domain.Enums.FacilityCycleStatus>(request.Status, true, out var statusEnum))
                {
                    filteredCycles = filteredCycles.Where(c => c.Status == statusEnum);
                }
            }

            var cyclesList = filteredCycles.ToList();
            
            // Filter by user requests if requested (optimized)
            if (request.OnlyWithUserRequests && memberInfo != null)
            {
                var cycleIds = cyclesList.Select(c => c.Id).ToList();
                var userRequestCycleIds = await _requestRepository.GetCyclesWithUserRequestsAsync(memberInfo.Id, cycleIds, cancellationToken);
                
                cyclesList = cyclesList.Where(c => userRequestCycleIds.Contains(c.Id)).ToList();
            }
            var totalCount = cyclesList.Count;
            
            // Apply pagination
            var paginatedCycles = cyclesList
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Get user requests for these cycles if NationalNumber provided (optimized)
            Dictionary<Guid, Domain.Entities.FacilityRequest>? userRequests = null;
            if (!string.IsNullOrWhiteSpace(request.NationalNumber) && memberInfo != null)
            {
                var cycleIds = paginatedCycles.Select(c => c.Id).ToList();
                userRequests = await _requestRepository.GetUserRequestsForCyclesAsync(memberInfo.Id, cycleIds, cancellationToken);
            }

            // Map cycles to DTOs with user context
            var cycleDtos = new List<FacilityCycleWithUserContextDto>();
            var eligibleCyclesCount = 0;
            var requestedCyclesCount = 0;

            foreach (var cycle in paginatedCycles)
            {
                var cycleDto = MapCycleToDto(cycle, memberInfo, userRequests, request.IncludeUserRequestStatus, cancellationToken).Result;
                cycleDtos.Add(cycleDto);

                if (cycleDto.UserEligibility?.IsEligible == true)
                    eligibleCyclesCount++;

                if (cycleDto.LastRequest != null)
                    requestedCyclesCount++;
            }

            // Calculate summary statistics
            var baseSummary = CalculateSummary(facility, memberInfo, userRequests, cancellationToken).Result;
            var summary = new CycleSummaryDto
            {
                TotalCycles = baseSummary.TotalCycles,
                ActiveCycles = baseSummary.ActiveCycles,
                EligibleCycles = eligibleCyclesCount,
                RequestedCycles = requestedCyclesCount
            };

            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

            _logger.LogInformation("Retrieved {Count} cycles for facility {FacilityId}, user {NationalNumber}",
                cycleDtos.Count, request.FacilityId, request.NationalNumber);

            return ApplicationResult<GetFacilityCyclesResponse>.Success(new GetFacilityCyclesResponse
            {
                Facility = MapFacilityToDto(facility),
                UserInfo = memberInfo != null ? MapMemberInfoToDto(memberInfo) : null,
                Cycles = cycleDtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages,
                Summary = summary
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving facility cycles for facility {FacilityId}, user {NationalNumber}",
                request.FacilityId, request.NationalNumber);
            return ApplicationResult<GetFacilityCyclesResponse>.Failure(
                "خطای داخلی در دریافت لیست دوره‌های تسهیلات");
        }
    }

    private Task<FacilityCycleWithUserContextDto> MapCycleToDto(
        Domain.Entities.FacilityCycle cycle,
        MemberInfoDto? memberInfo,
        Dictionary<Guid, Domain.Entities.FacilityRequest>? userRequests,
        bool includeUserRequestStatus,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var daysUntilStart = (cycle.StartDate - now).Days;
        var daysUntilEnd = (cycle.EndDate - now).Days;
        var hasStarted = now >= cycle.StartDate;
        var hasEnded = now >= cycle.EndDate;
        var isActive = cycle.Status == FacilityCycleStatus.Active && hasStarted && !hasEnded;
        var isAcceptingApplications = isActive && cycle.UsedQuota < cycle.Quota;
        var quotaUtilizationPercentage = cycle.Quota > 0 ? (decimal)cycle.UsedQuota / cycle.Quota * 100 : 0;

        // Calculate user eligibility
        UserEligibilityDto? userEligibility = null;
        if (memberInfo != null)
        {
            var eligibilityResult = _eligibilityService.ValidateMemberEligibilityWithDetails(
                cycle,
                memberInfo.Features ?? new List<string>(),
                memberInfo.Capabilities ?? new List<string>());

            // Extract missing features and capabilities
            var requiredFeatures = cycle.Facility.Features
                .Where(f => f.RequirementType == FacilityFeatureRequirementType.Required)
                .Select(f => f.FeatureId)
                .ToList();
            var requiredCapabilities = cycle.Facility.CapabilityPolicies
                .Where(cp => cp.PolicyType == CapabilityPolicyType.Required)
                .Select(cp => cp.CapabilityId)
                .ToList();
            var userFeatures = memberInfo.Features ?? new List<string>();
            var userCapabilities = memberInfo.Capabilities ?? new List<string>();
            var missingFeatures = requiredFeatures.Except(userFeatures).ToList();
            var missingCapabilities = requiredCapabilities.Except(userCapabilities).ToList();

            userEligibility = new UserEligibilityDto
            {
                IsEligible = eligibilityResult.IsEligible,
                ValidationMessage = eligibilityResult.ErrorMessage,
                ValidationErrors = eligibilityResult.ValidationErrors ?? new List<string>(),
                MeetsFeatureRequirements = eligibilityResult.IsEligible,
                MeetsCapabilityRequirements = eligibilityResult.IsEligible,
                MissingFeatures = missingFeatures,
                MissingCapabilities = missingCapabilities
            };
        }

        // Get user request history
        List<UserRequestHistoryDto> userRequestHistory = new();
        UserRequestHistoryDto? lastRequest = null;
        if (includeUserRequestStatus && userRequests != null && userRequests.TryGetValue(cycle.Id, out var userRequest))
        {
            userRequestHistory = new List<UserRequestHistoryDto> { MapToRequestHistoryDto(userRequest) };
            lastRequest = userRequestHistory.First();
        }
        // If user doesn't have request, userRequestHistory remains empty list

        return Task.FromResult(new FacilityCycleWithUserContextDto
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
            FinancialTerms = new FinancialTermsDto
            {
                MinAmountRials = cycle.MinAmount?.AmountRials,
                MaxAmountRials = cycle.MaxAmount?.AmountRials,
                DefaultAmountRials = cycle.DefaultAmount?.AmountRials,
                Currency = cycle.MinAmount?.Currency ?? cycle.MaxAmount?.Currency ?? cycle.DefaultAmount?.Currency ?? "IRR",
                PaymentMonths = cycle.PaymentMonths,
                InterestRate = cycle.InterestRate,
                InterestRatePercentage = cycle.InterestRate.HasValue ? cycle.InterestRate.Value * 100 : null,
                CooldownDays = cycle.CooldownDays
            },
            Rules = new CycleRulesDto
            {
                IsRepeatable = cycle.IsRepeatable,
                IsExclusive = cycle.IsExclusive,
                ExclusiveSetId = cycle.ExclusiveSetId,
                MaxActiveAcrossCycles = cycle.MaxActiveAcrossCycles,
                HasDependencies = cycle.Dependencies.Any()
            },
            UserEligibility = userEligibility,
            UserRequestHistory = userRequestHistory,
            LastRequest = lastRequest,
            CreatedAt = cycle.CreatedAt
        });
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

    private Task<CycleSummaryDto> CalculateSummary(
        Domain.Entities.Facility facility,
        MemberInfoDto? memberInfo,
        Dictionary<Guid, Domain.Entities.FacilityRequest>? userRequests,
        CancellationToken cancellationToken)
    {
        var allCycles = facility.Cycles.ToList();
        var activeCycles = allCycles.Count(c => c.Status == FacilityCycleStatus.Active);
        var draftCycles = allCycles.Count(c => c.Status == FacilityCycleStatus.Draft);
        var closedCycles = allCycles.Count(c => c.Status == FacilityCycleStatus.Closed);
        var completedCycles = allCycles.Count(c => c.Status == FacilityCycleStatus.Completed);
        var cancelledCycles = allCycles.Count(c => c.Status == FacilityCycleStatus.Cancelled);

        var totalQuota = allCycles.Sum(c => c.Quota);
        var totalUsedQuota = allCycles.Sum(c => c.UsedQuota);
        var totalAvailableQuota = totalQuota - totalUsedQuota;
        var overallQuotaUtilizationPercentage = totalQuota > 0 ? (decimal)totalUsedQuota / totalQuota * 100 : 0;

        return Task.FromResult(new CycleSummaryDto
        {
            TotalCycles = allCycles.Count,
            ActiveCycles = activeCycles,
            DraftCycles = draftCycles,
            ClosedCycles = closedCycles,
            CompletedCycles = completedCycles,
            CancelledCycles = cancelledCycles,
            TotalQuota = totalQuota,
            TotalUsedQuota = totalUsedQuota,
            TotalAvailableQuota = totalAvailableQuota,
            OverallQuotaUtilizationPercentage = overallQuotaUtilizationPercentage,
            EligibleCycles = 0, // Will be set by caller
            RequestedCycles = 0 // Will be set by caller
        });
    }

    private string FormatAmount(decimal amount)
    {
        return $"{amount:N0} ریال";
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

    private UserRequestHistoryDto MapToRequestHistoryDto(Domain.Entities.FacilityRequest request)
    {
        var now = DateTime.UtcNow;
        var daysSinceCreated = (now - request.CreatedAt).Days;

        return new UserRequestHistoryDto
        {
            RequestId = request.Id,
            Status = request.Status.ToString(),
            StatusText = EnumTextMappingService.GetFacilityRequestStatusDescription(request.Status),
            RequestedAmountRials = request.RequestedAmount.AmountRials,
            ApprovedAmountRials = request.ApprovedAmount?.AmountRials,
            CreatedAt = request.CreatedAt,
            ApprovedAt = request.ApprovedAt,
            RejectedAt = request.RejectedAt,
            RejectionReason = request.RejectionReason,
            DaysSinceCreated = daysSinceCreated,
            IsInProgress = EnumTextMappingService.IsRequestInProgress(request.Status),
            IsCompleted = EnumTextMappingService.IsRequestCompleted(request.Status),
            IsRejected = EnumTextMappingService.IsRequestRejected(request.Status),
            IsCancelled = EnumTextMappingService.IsRequestCancelled(request.Status)
        };
    }
}
