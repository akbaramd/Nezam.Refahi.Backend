using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserMemberInfo;
using Nezam.Refahi.Facilities.Application.Services;
using Nezam.Refahi.Facilities.Domain.Enums;
using Nezam.Refahi.Facilities.Domain.Repositories;
using Nezam.Refahi.Facilities.Domain.Services;
using Nezam.Refahi.Membership.Contracts.Dtos;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails;

/// <summary>
/// Handler for getting detailed facility cycle information with user context
/// </summary>
public class GetFacilityCycleDetailsQueryHandler : IRequestHandler<GetFacilityCycleDetailsQuery, ApplicationResult<GetFacilityCycleDetailsResponse>>
{
    private readonly IFacilityCycleRepository _cycleRepository;
    private readonly IFacilityRepository _facilityRepository;
    private readonly IFacilityRequestRepository _requestRepository;
    private readonly IMemberInfoService _memberInfoService;
    private readonly FacilityEligibilityDomainService _eligibilityService;
    private readonly IValidator<GetFacilityCycleDetailsQuery> _validator;
    private readonly ILogger<GetFacilityCycleDetailsQueryHandler> _logger;

    public GetFacilityCycleDetailsQueryHandler(
        IFacilityCycleRepository cycleRepository,
        IFacilityRepository facilityRepository,
        IFacilityRequestRepository requestRepository,
        IMemberInfoService memberInfoService,
        FacilityEligibilityDomainService eligibilityService,
        IValidator<GetFacilityCycleDetailsQuery> validator,
        ILogger<GetFacilityCycleDetailsQueryHandler> logger)
    {
        _cycleRepository = cycleRepository;
        _facilityRepository = facilityRepository;
        _requestRepository = requestRepository;
        _memberInfoService = memberInfoService;
        _eligibilityService = eligibilityService;
        _validator = validator;
        _logger = logger;
    }

    public async Task<ApplicationResult<GetFacilityCycleDetailsResponse>> Handle(
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
                return ApplicationResult<GetFacilityCycleDetailsResponse>.Failure(
                    errors,
                    "اطلاعات ورودی نامعتبر است");
            }

            // Get cycle with all details
            var cycle = await _cycleRepository.GetWithAllDetailsAsync(request.CycleId, cancellationToken);
            if (cycle == null)
            {
                return ApplicationResult<GetFacilityCycleDetailsResponse>.Failure(
                    "دوره تسهیلات مورد نظر یافت نشد");
            }

            // Get facility if requested
            Domain.Entities.Facility? facility = null;
            if (request.IncludeFacilityInfo)
            {
                facility = await _facilityRepository.GetByIdAsync(cycle.FacilityId, cancellationToken);
            }

            // Get user member info if NationalNumber provided
            MemberInfoDto? memberInfo = null;
            if (!string.IsNullOrWhiteSpace(request.NationalNumber))
            {
                memberInfo = await _memberInfoService.GetMemberInfoAsync(new NationalId(request.NationalNumber));
                if (memberInfo == null)
                {
                    return ApplicationResult<GetFacilityCycleDetailsResponse>.Failure(
                        "اطلاعات عضو یافت نشد");
                }
            }

            // Get user request history if requested (optimized)
            List<UserRequestHistoryDto> userRequestHistory = new();
            UserRequestHistoryDto? lastRequest = null;
            if (request.IncludeUserRequestHistory && memberInfo != null)
            {
                var userRequest = await _requestRepository.GetUserRequestForCycleAsync(memberInfo.Id, request.CycleId, cancellationToken);
                if (userRequest != null)
                {
                    userRequestHistory = new List<UserRequestHistoryDto> { MapToRequestHistoryDto(userRequest) };
                    lastRequest = userRequestHistory.First();
                }
            }

            // Calculate detailed eligibility analysis if requested
            DetailedEligibilityDto? eligibilityAnalysis = null;
            if (request.IncludeEligibilityDetails && memberInfo != null)
            {
                eligibilityAnalysis = await CalculateDetailedEligibility(cycle, memberInfo, cancellationToken);
            }

            // Map dependencies if requested
            List<CycleDependencyDto>? dependencies = null;
            if (request.IncludeDependencies)
            {
                dependencies = cycle.Dependencies.Select(d => MapToDependencyDto(d)).ToList();
            }

            // Calculate statistics if requested
            CycleStatisticsDto? statistics = null;
            if (request.IncludeStatistics)
            {
                statistics = await CalculateCycleStatistics(cycle, cancellationToken);
            }

            _logger.LogInformation("Retrieved detailed cycle information for cycle {CycleId}, user {NationalNumber}",
                request.CycleId, request.NationalNumber);

            return ApplicationResult<GetFacilityCycleDetailsResponse>.Success(new GetFacilityCycleDetailsResponse
            {
                Cycle = MapCycleToDetailedDto(cycle),
                Facility = facility != null ? MapFacilityToDto(facility) : null,
                UserInfo = memberInfo != null ? MapMemberInfoToDto(memberInfo) : null,
                EligibilityAnalysis = eligibilityAnalysis,
                UserRequestHistory = userRequestHistory,
                LastRequest = lastRequest,
                Dependencies = dependencies,
                Statistics = statistics
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving detailed cycle information for cycle {CycleId}, user {NationalNumber}",
                request.CycleId, request.NationalNumber);
            return ApplicationResult<GetFacilityCycleDetailsResponse>.Failure(
                "خطای داخلی در دریافت جزئیات دوره تسهیلات");
        }
    }

    private FacilityCycleDetailsDto MapCycleToDetailedDto(Domain.Entities.FacilityCycle cycle)
    {
        var now = DateTime.UtcNow;
        var daysUntilStart = (cycle.StartDate - now).Days;
        var daysUntilEnd = (cycle.EndDate - now).Days;
        var hasStarted = now >= cycle.StartDate;
        var hasEnded = now >= cycle.EndDate;
        var isActive = cycle.Status == FacilityCycleStatus.Active && hasStarted && !hasEnded;
        var isAcceptingApplications = isActive && cycle.UsedQuota < cycle.Quota;
        var quotaUtilizationPercentage = cycle.Quota > 0 ? (decimal)cycle.UsedQuota / cycle.Quota * 100 : 0;

        return new FacilityCycleDetailsDto
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
            StatusText = EnumTextMappingService.GetFacilityCycleStatusDescription(cycle.Status),
            Description = cycle.Description,
            FinancialTerms = MapToDetailedFinancialTerms(cycle),
            Rules = MapToDetailedCycleRules(cycle),
            AdmissionStrategy = cycle.AdmissionStrategy,
            AdmissionStrategyDescription = EnumTextMappingService.GetAdmissionStrategyDescription(cycle.AdmissionStrategy),
            WaitlistCapacity = cycle.WaitlistCapacity,
            Metadata = cycle.Metadata,
            CreatedAt = cycle.CreatedAt,
            LastModifiedAt = cycle.LastModifiedAt
        };
    }

    private DetailedFinancialTermsDto MapToDetailedFinancialTerms(Domain.Entities.FacilityCycle cycle)
    {
        var monthlyPaymentAmount = CalculateMonthlyPayment(cycle);

        return new DetailedFinancialTermsDto
        {
            MinAmountRials = cycle.MinAmount?.AmountRials,
            MaxAmountRials = cycle.MaxAmount?.AmountRials,
            DefaultAmountRials = cycle.DefaultAmount?.AmountRials,
            Currency = cycle.MinAmount?.Currency ?? cycle.MaxAmount?.Currency ?? cycle.DefaultAmount?.Currency ?? "IRR",
            PaymentMonths = cycle.PaymentMonths,
            InterestRate = cycle.InterestRate,
            InterestRatePercentage = cycle.InterestRate.HasValue ? cycle.InterestRate.Value * 100 : null,
            CooldownDays = cycle.CooldownDays,
            FormattedMinAmount = FormatAmount(cycle.MinAmount?.AmountRials),
            FormattedMaxAmount = FormatAmount(cycle.MaxAmount?.AmountRials),
            FormattedDefaultAmount = FormatAmount(cycle.DefaultAmount?.AmountRials),
            FormattedInterestRate = cycle.InterestRate.HasValue ? $"{cycle.InterestRate.Value * 100:F2}%" : null,
            MonthlyPaymentAmount = monthlyPaymentAmount
        };
    }

    private DetailedCycleRulesDto MapToDetailedCycleRules(Domain.Entities.FacilityCycle cycle)
    {
        return new DetailedCycleRulesDto
        {
            IsRepeatable = cycle.IsRepeatable,
            IsExclusive = cycle.IsExclusive,
            ExclusiveSetId = cycle.ExclusiveSetId,
            MaxActiveAcrossCycles = cycle.MaxActiveAcrossCycles,
            HasDependencies = cycle.Dependencies.Any(),
            RepeatabilityDescription = cycle.IsRepeatable ? "قابل تکرار" : "غیرقابل تکرار",
            ExclusivityDescription = cycle.IsExclusive ? $"انحصاری (مجموعه: {cycle.ExclusiveSetId})" : "غیرانحصاری",
            DependencyDescription = cycle.Dependencies.Any() ? $"{cycle.Dependencies.Count} وابستگی" : "بدون وابستگی"
        };
    }

    private async Task<DetailedEligibilityDto> CalculateDetailedEligibility(
        Domain.Entities.FacilityCycle cycle,
        MemberInfoDto memberInfo,
        CancellationToken cancellationToken)
    {
        var eligibilityResult = _eligibilityService.ValidateMemberEligibilityWithDetails(
            cycle,
            memberInfo.Features ?? new List<string>(),
            memberInfo.Capabilities ?? new List<string>());

        // Calculate feature requirements
        var requiredFeatures = cycle.Facility.Features
            .Where(f => f.RequirementType == FacilityFeatureRequirementType.Required)
            .Select(f => f.FeatureId)
            .ToList();
        var prohibitedFeatures = cycle.Facility.Features
            .Where(f => f.RequirementType == FacilityFeatureRequirementType.Prohibited)
            .Select(f => f.FeatureId)
            .ToList();
        var userFeatures = memberInfo.Features ?? new List<string>();
        var matchingFeatures = userFeatures.Intersect(requiredFeatures).ToList();
        var missingFeatures = requiredFeatures.Except(userFeatures).ToList();
        var hasProhibitedFeatures = prohibitedFeatures.Intersect(userFeatures).Any();

        var featureRequirements = new FeatureRequirementsDto
        {
            MeetsRequirements = eligibilityResult.IsEligible && !hasProhibitedFeatures,
            RequiredFeatures = requiredFeatures,
            UserFeatures = userFeatures,
            MissingFeatures = missingFeatures,
            MatchingFeatures = matchingFeatures
        };

        // Calculate capability requirements
        var requiredCapabilities = cycle.Facility.CapabilityPolicies
            .Where(cp => cp.PolicyType == CapabilityPolicyType.Required)
            .Select(cp => cp.CapabilityId)
            .ToList();
        var prohibitedCapabilities = cycle.Facility.CapabilityPolicies
            .Where(cp => cp.PolicyType == CapabilityPolicyType.Prohibited)
            .Select(cp => cp.CapabilityId)
            .ToList();
        var userCapabilities = memberInfo.Capabilities ?? new List<string>();
        var matchingCapabilities = userCapabilities.Intersect(requiredCapabilities).ToList();
        var missingCapabilities = requiredCapabilities.Except(userCapabilities).ToList();
        var hasProhibitedCapabilities = prohibitedCapabilities.Intersect(userCapabilities).Any();

        var capabilityRequirements = new CapabilityRequirementsDto
        {
            MeetsRequirements = eligibilityResult.IsEligible && !hasProhibitedCapabilities,
            RequiredCapabilities = requiredCapabilities,
            UserCapabilities = userCapabilities,
            MissingCapabilities = missingCapabilities,
            MatchingCapabilities = matchingCapabilities
        };

        // Calculate dependency requirements
        var userCompletedFacilities = await GetUserCompletedFacilitiesAsync(memberInfo.Id, cancellationToken);
        var requiredDependencies = cycle.Dependencies.ToList();
        var satisfiedDependencies = new List<CycleDependencyDto>();
        var missingDependencies = new List<CycleDependencyDto>();

        foreach (var dependency in requiredDependencies)
        {
            var isSatisfied = userCompletedFacilities.Contains(dependency.RequiredFacilityId);
            var dependencyDto = MapToDependencyDto(dependency, isSatisfied);
            
            if (isSatisfied)
            {
                satisfiedDependencies.Add(dependencyDto);
            }
            else
            {
                missingDependencies.Add(dependencyDto);
            }
        }

        var dependencyRequirements = new DependencyRequirementsDto
        {
            MeetsRequirements = !missingDependencies.Any(),
            RequiredDependencies = requiredDependencies.Select(d => MapToDependencyDto(d)).ToList(),
            UserCompletedFacilities = userCompletedFacilities,
            MissingDependencies = missingDependencies,
            SatisfiedDependencies = satisfiedDependencies
        };

        // Calculate cooldown requirements
        var lastFacilityReceivedAt = await GetUserLastFacilityReceivedAtAsync(memberInfo.Id, cancellationToken);
        var daysSinceLastFacility = lastFacilityReceivedAt.HasValue 
            ? (DateTime.UtcNow - lastFacilityReceivedAt.Value).Days 
            : (int?)null;
        var meetsCooldownRequirement = !lastFacilityReceivedAt.HasValue || 
            daysSinceLastFacility >= cycle.CooldownDays;
        var daysRemainingInCooldown = lastFacilityReceivedAt.HasValue && !meetsCooldownRequirement
            ? cycle.CooldownDays - daysSinceLastFacility
            : (int?)null;

        var cooldownRequirements = new CooldownRequirementsDto
        {
            MeetsRequirements = meetsCooldownRequirement,
            RequiredCooldownDays = cycle.CooldownDays,
            DaysSinceLastFacility = daysSinceLastFacility,
            LastFacilityReceivedAt = lastFacilityReceivedAt,
            DaysRemainingInCooldown = daysRemainingInCooldown
        };

        // Calculate quota availability
        var quotaAvailability = new QuotaAvailabilityDto
        {
            IsAvailable = cycle.UsedQuota < cycle.Quota,
            TotalQuota = cycle.Quota,
            UsedQuota = cycle.UsedQuota,
            AvailableQuota = cycle.Quota - cycle.UsedQuota,
            UtilizationPercentage = cycle.Quota > 0 ? (decimal)cycle.UsedQuota / cycle.Quota * 100 : 0
        };

        return new DetailedEligibilityDto
        {
            IsEligible = eligibilityResult.IsEligible,
            ValidationMessage = eligibilityResult.ErrorMessage,
            ValidationErrors = eligibilityResult.ValidationErrors ?? new List<string>(),
            FeatureRequirements = featureRequirements,
            CapabilityRequirements = capabilityRequirements,
            DependencyRequirements = dependencyRequirements,
            CooldownRequirements = cooldownRequirements,
            QuotaAvailability = quotaAvailability
        };
    }

    private async Task<CycleStatisticsDto> CalculateCycleStatistics(
        Domain.Entities.FacilityCycle cycle,
        CancellationToken cancellationToken)
    {
        // Get all requests for this cycle
        var allRequests = await _requestRepository.GetByCycleIdAsync(cycle.Id, cancellationToken);

        var pendingRequests = allRequests.Count(r => EnumTextMappingService.IsRequestInProgress(r.Status));
        var approvedRequests = allRequests.Count(r => EnumTextMappingService.IsRequestCompleted(r.Status));
        var rejectedRequests = allRequests.Count(r => EnumTextMappingService.IsRequestRejected(r.Status));

        var completedRequests = allRequests.Where(r => EnumTextMappingService.IsRequestCompleted(r.Status)).ToList();
        var averageProcessingTimeDays = completedRequests.Any()
            ? (decimal?)completedRequests.Average(r => (decimal)((r.ApprovedAt ?? r.CompletedAt ?? DateTime.UtcNow) - r.CreatedAt).TotalDays)
            : null;

        var now = DateTime.UtcNow;
        var cycleDurationDays = (cycle.EndDate - cycle.StartDate).Days;
        var daysElapsed = Math.Max(0, (now - cycle.StartDate).Days);
        var daysRemaining = Math.Max(0, (cycle.EndDate - now).Days);
        var cycleProgressPercentage = cycleDurationDays > 0 ? (decimal)daysElapsed / cycleDurationDays * 100 : 0;

        return new CycleStatisticsDto
        {
            TotalQuota = cycle.Quota,
            UsedQuota = cycle.UsedQuota,
            AvailableQuota = cycle.Quota - cycle.UsedQuota,
            UtilizationPercentage = cycle.Quota > 0 ? (decimal)cycle.UsedQuota / cycle.Quota * 100 : 0,
            PendingRequests = pendingRequests,
            ApprovedRequests = approvedRequests,
            RejectedRequests = rejectedRequests,
            AverageProcessingTimeDays = averageProcessingTimeDays,
            CycleDurationDays = cycleDurationDays,
            DaysElapsed = daysElapsed,
            DaysRemaining = daysRemaining,
            CycleProgressPercentage = cycleProgressPercentage
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

    private CycleDependencyDto MapToDependencyDto(Domain.Entities.FacilityCycleDependency dependency, bool isSatisfied = false)
    {
        return new CycleDependencyDto
        {
            Id = dependency.Id,
            RequiredFacilityId = dependency.RequiredFacilityId,
            RequiredFacilityName = dependency.RequiredFacilityName,
            MustBeCompleted = dependency.MustBeCompleted,
            CreatedAt = dependency.CreatedAt,
            IsSatisfied = isSatisfied,
            UserRequestStatus = null // Will be set by caller if needed
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

    private decimal? CalculateMonthlyPayment(Domain.Entities.FacilityCycle cycle)
    {
        if (cycle.DefaultAmount == null || !cycle.InterestRate.HasValue)
            return null;

        var principal = (double)cycle.DefaultAmount.AmountRials;
        var annualRate = cycle.InterestRate.Value;
        var months = cycle.PaymentMonths;

        if (months <= 0) return null;

        var monthlyRate = (double)annualRate / 12;
        var monthlyPayment = principal * (monthlyRate * Math.Pow(1 + monthlyRate, months)) / (Math.Pow(1 + monthlyRate, months) - 1);

        return (decimal)monthlyPayment;
    }

    private string? FormatAmount(decimal? amount)
    {
        if (!amount.HasValue) return null;
        return $"{amount.Value:N0} ریال";
    }

    private async Task<List<Guid>> GetUserCompletedFacilitiesAsync(Guid memberId, CancellationToken cancellationToken)
    {
        var userRequests = await _requestRepository.GetByUserIdAsync(memberId, cancellationToken);
        return userRequests
            .Where(r => EnumTextMappingService.IsRequestCompleted(r.Status))
            .Select(r => r.FacilityId)
            .Distinct()
            .ToList();
    }

    private async Task<DateTime?> GetUserLastFacilityReceivedAtAsync(Guid memberId, CancellationToken cancellationToken)
    {
        var userRequests = await _requestRepository.GetByUserIdAsync(memberId, cancellationToken);
        var completedRequests = userRequests
            .Where(r => EnumTextMappingService.IsRequestCompleted(r.Status))
            .OrderByDescending(r => r.ApprovedAt ?? r.CompletedAt ?? r.CreatedAt)
            .ToList();

        return completedRequests.FirstOrDefault()?.ApprovedAt ?? 
               completedRequests.FirstOrDefault()?.CompletedAt;
    }
}
