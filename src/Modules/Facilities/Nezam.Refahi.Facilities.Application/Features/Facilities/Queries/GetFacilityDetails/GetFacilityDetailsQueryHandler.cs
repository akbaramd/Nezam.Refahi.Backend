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
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityDetails;

/// <summary>
/// Handler for getting facility details
/// </summary>
public class GetFacilityDetailsQueryHandler : IRequestHandler<GetFacilityDetailsQuery, ApplicationResult<GetFacilityDetailsResult>>
{
    private readonly IFacilityRepository _facilityRepository;
    private readonly IFacilityRequestRepository _requestRepository;
    private readonly IMemberInfoService _memberInfoService;
    private readonly IValidator<GetFacilityDetailsQuery> _validator;
    private readonly ILogger<GetFacilityDetailsQueryHandler> _logger;

    public GetFacilityDetailsQueryHandler(
        IFacilityRepository facilityRepository,
        IFacilityRequestRepository requestRepository,
        IMemberInfoService memberInfoService,
        IValidator<GetFacilityDetailsQuery> validator,
        ILogger<GetFacilityDetailsQueryHandler> logger)
    {
        _facilityRepository = facilityRepository;
        _requestRepository = requestRepository;
        _memberInfoService = memberInfoService;
        _validator = validator;
        _logger = logger;
    }

    public async Task<ApplicationResult<GetFacilityDetailsResult>> Handle(
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
                return ApplicationResult<GetFacilityDetailsResult>.Failure(
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
                return ApplicationResult<GetFacilityDetailsResult>.Failure(
                    "تسهیلات مورد نظر یافت نشد");
            }

            // Get user member info if NationalNumber provided
            MemberInfoDto? memberInfo = null;
            if (!string.IsNullOrWhiteSpace(request.NationalNumber))
            {
                memberInfo = await _memberInfoService.GetMemberInfoAsync(new NationalId(request.NationalNumber));
                if (memberInfo == null)
                {
                    return ApplicationResult<GetFacilityDetailsResult>.Failure(
                        "اطلاعات عضو یافت نشد");
                }
            }

            // Map to DTO
            var facilityDto = await MapToDto(facility, request, memberInfo, cancellationToken);

            _logger.LogInformation("Retrieved facility details for {FacilityId}",
                request.FacilityId);

            return ApplicationResult<GetFacilityDetailsResult>.Success(new GetFacilityDetailsResult
            {
                Facility = facilityDto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving facility details for {FacilityId}",
                request.FacilityId);
            return ApplicationResult<GetFacilityDetailsResult>.Failure(
                "خطای داخلی در دریافت جزئیات تسهیلات");
        }
    }

    private async Task<FacilityDetailsDto> MapToDto(Domain.Entities.Facility facility, GetFacilityDetailsQuery request, MemberInfoDto? memberInfo, CancellationToken cancellationToken)
    {
        var cycles = new List<FacilityCycleDetailsDto>();
        
        if (request.IncludeCycles)
        {
            foreach (var cycle in facility.Cycles)
            {
                var cycleDto = await MapCycleToDetailedDto(cycle, request, memberInfo, cancellationToken);
                cycles.Add(cycleDto);
            }
        }

        return new FacilityDetailsDto
        {
            Id = facility.Id,
            Name = facility.Name,
            Code = facility.Code,
            Type = facility.Type.ToString(),
            Status = facility.Status.ToString(),
            Description = facility.Description,
            BankName = facility.BankName,
            BankCode = facility.BankCode,
            BankAccountNumber = facility.BankAccountNumber,
            Cycles = cycles,
            Features = request.IncludeFeatures ? facility.Features.Select(MapFeatureToDto).ToList() : new List<FacilityFeatureDto>(),
            CapabilityPolicies = request.IncludePolicies ? facility.CapabilityPolicies.Select(MapPolicyToDto).ToList() : new List<FacilityCapabilityPolicyDto>(),
            Metadata = facility.Metadata,
            CreatedAt = facility.CreatedAt,
            LastModifiedAt = facility.LastModifiedAt
        };
    }

    private async Task<FacilityCycleDetailsDto> MapCycleToDetailedDto(Domain.Entities.FacilityCycle cycle, GetFacilityDetailsQuery request, MemberInfoDto? memberInfo, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var daysUntilStart = (cycle.StartDate - now).Days;
        var daysUntilEnd = (cycle.EndDate - now).Days;
        var hasStarted = now >= cycle.StartDate;
        var hasEnded = now >= cycle.EndDate;
        var isActive = cycle.Status == FacilityCycleStatus.Active && hasStarted && !hasEnded;
        var isAcceptingApplications = isActive && cycle.UsedQuota < cycle.Quota;
        var quotaUtilizationPercentage = cycle.Quota > 0 ? (decimal)cycle.UsedQuota / cycle.Quota * 100 : 0;
        var monthlyPaymentAmount = CalculateMonthlyPayment(cycle);

        // Get user request history if requested
        List<UserRequestHistoryDto> userRequestHistory = new();
        UserRequestHistoryDto? lastRequest = null;
        if (request.IncludeUserRequestHistory && memberInfo != null)
        {
            var userRequest = await _requestRepository.GetUserRequestForCycleAsync(memberInfo.Id, cycle.Id, cancellationToken);
            if (userRequest != null)
            {
                userRequestHistory = new List<UserRequestHistoryDto> { MapToRequestHistoryDto(userRequest) };
                lastRequest = userRequestHistory.First();
            }
        }

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
            FinancialTerms = new DetailedFinancialTermsDto
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
            },
            Rules = new DetailedCycleRulesDto
            {
                IsRepeatable = cycle.IsRepeatable,
                IsExclusive = cycle.IsExclusive,
                ExclusiveSetId = cycle.ExclusiveSetId,
                MaxActiveAcrossCycles = cycle.MaxActiveAcrossCycles,
                HasDependencies = cycle.Dependencies.Any()
            },
            Dependencies = cycle.Dependencies.Select(MapDependencyToDto).ToList(),
            AdmissionStrategy = cycle.AdmissionStrategy,
            AdmissionStrategyDescription = EnumTextMappingService.GetAdmissionStrategyDescription(cycle.AdmissionStrategy),
            WaitlistCapacity = cycle.WaitlistCapacity,
            Metadata = cycle.Metadata,
            CreatedAt = cycle.CreatedAt,
            LastModifiedAt = cycle.LastModifiedAt,
            UserRequestHistory = userRequestHistory
        };
    }

    private static FacilityCycleDependencyDto MapDependencyToDto(Domain.Entities.FacilityCycleDependency dependency)
    {
        return new FacilityCycleDependencyDto
        {
            Id = dependency.Id,
            RequiredFacilityId = dependency.RequiredFacilityId,
            RequiredFacilityName = dependency.RequiredFacilityName,
            MustBeCompleted = dependency.MustBeCompleted,
            CreatedAt = dependency.CreatedAt
        };
    }
    private static decimal? CalculateMonthlyPayment(Domain.Entities.FacilityCycle cycle)
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
    private static FacilityFeatureDto MapFeatureToDto(Domain.Entities.FacilityFeature feature)
    {
        return new FacilityFeatureDto
        {
            Id = feature.Id,
            FeatureId = feature.FeatureId,
            RequirementType = feature.RequirementType.ToString(),
            Notes = feature.Notes,
            AssignedAt = feature.AssignedAt
        };
    }

    private static FacilityCapabilityPolicyDto MapPolicyToDto(Domain.Entities.FacilityCapabilityPolicy policy)
    {
        return new FacilityCapabilityPolicyDto
        {
            Id = policy.Id,
            PolicyType = policy.PolicyType.ToString(),
            PolicyValue = policy.ModifierValue?.ToString() ?? string.Empty,
            Notes = policy.Notes,
            AssignedAt = policy.AssignedAt
        };
    }
    private static string? FormatAmount(decimal? amount)
    {
      if (!amount.HasValue) return null;
      return $"{amount.Value:N0} ریال";
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
