using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilities;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityDetails;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycles;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails;
using Nezam.Refahi.Facilities.Application.Services;
using Nezam.Refahi.Facilities.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityRequests;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserCycleRequests;
using Nezam.Refahi.Facilities.Domain.Enums;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityRequestDetails;

/// <summary>
/// Handler for getting facility request details
/// </summary>
public class GetFacilityRequestDetailsQueryHandler : IRequestHandler<GetFacilityRequestDetailsQuery, ApplicationResult<GetFacilityRequestDetailsResult>>
{
    private readonly IFacilityRequestRepository _requestRepository;
    private readonly IValidator<GetFacilityRequestDetailsQuery> _validator;
    private readonly ILogger<GetFacilityRequestDetailsQueryHandler> _logger;

    public GetFacilityRequestDetailsQueryHandler(
        IFacilityRequestRepository requestRepository,
        IValidator<GetFacilityRequestDetailsQuery> validator,
        ILogger<GetFacilityRequestDetailsQueryHandler> logger)
    {
        _requestRepository = requestRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<ApplicationResult<GetFacilityRequestDetailsResult>> Handle(
        GetFacilityRequestDetailsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate input
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ApplicationResult<GetFacilityRequestDetailsResult>.Failure(
                    errors, 
                    "اطلاعات ورودی نامعتبر است");
            }

            // Get facility request with related data
            var facilityRequest = await _requestRepository.GetByIdWithDetailsAsync(
                request.RequestId,
                request.IncludeFacility,
                request.IncludeCycle,
                cancellationToken);

            if (facilityRequest == null)
            {
                return ApplicationResult<GetFacilityRequestDetailsResult>.Failure(
                    "درخواست تسهیلات مورد نظر یافت نشد");
            }

            // Map to DTO
            var requestDto = MapToDto(facilityRequest, request);

            _logger.LogInformation("Retrieved facility request details for {RequestId}",
                request.RequestId);

            return ApplicationResult<GetFacilityRequestDetailsResult>.Success(new GetFacilityRequestDetailsResult
            {
                Request = requestDto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving facility request details for {RequestId}",
                request.RequestId);
            return ApplicationResult<GetFacilityRequestDetailsResult>.Failure(
                "خطای داخلی در دریافت جزئیات درخواست تسهیلات");
        }
    }

    private static FacilityRequestDetailsDto MapToDto(Domain.Entities.FacilityRequest request, GetFacilityRequestDetailsQuery query)
    {
        return new FacilityRequestDetailsDto
        {
            Id = request.Id,
            Facility = query.IncludeFacility ? MapFacilityToDto(request.Facility) : null!,
            Cycle = query.IncludeCycle ? MapCycleToDto(request.FacilityCycle) : null!,
            Applicant = new ApplicantInfoDto
            {
                MemberId = request.MemberId,
                FullName = request.UserFullName,
                NationalId = request.UserNationalId,
            },
            RequestedAmountRials = request.RequestedAmount.AmountRials,
            ApprovedAmountRials = request.ApprovedAmount?.AmountRials,
            Currency = request.RequestedAmount.Currency,
            Status = request.Status.ToString(),
            StatusText = EnumTextMappingService.GetFacilityRequestStatusDescription(request.Status),
            CreatedAt = request.CreatedAt,
            LastModifiedAt = request.LastModifiedAt,
            IsInProgress = EnumTextMappingService.IsRequestInProgress(request.Status),
            IsCompleted = EnumTextMappingService.IsRequestCompleted(request.Status),
            IsRejected = EnumTextMappingService.IsRequestRejected(request.Status),
            IsCancelled = EnumTextMappingService.IsRequestCancelled(request.Status),
            RequestNumber = request.RequestNumber!,
            Description = request.Description,
            RejectionReason = request.RejectionReason,
            Timeline = new RequestTimelineDto
            {
                CreatedAt = request.CreatedAt,
                ApprovedAt = request.ApprovedAt,
                RejectedAt = request.RejectedAt,
                BankAppointmentScheduledAt = request.BankAppointmentScheduledAt,
                BankAppointmentDate = request.BankAppointmentDate,
                DisbursedAt = request.DisbursedAt,
                CompletedAt = request.CompletedAt
            },
            IsTerminal = EnumTextMappingService.IsRequestTerminal(request.Status),
            CanBeCancelled = EnumTextMappingService.CanRequestBeCancelled(request.Status),
            RequiresApplicantAction = EnumTextMappingService.RequiresApplicantAction(request.Status),
            RequiresBankAction = EnumTextMappingService.RequiresBankAction(request.Status),
            FacilityDetails = query.IncludeFacility ? MapFacilityToDetailedDto(request.Facility) : null,
            CycleDetails = query.IncludeCycle ? MapCycleToDetailedDto(request.FacilityCycle) : null
        };
    }
    
    private static FacilityInfoDto MapFacilityToDto(Domain.Entities.Facility facility)
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

    private static FacilityCycleDetailsDto MapCycleToDto(Domain.Entities.FacilityCycle cycle)
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
            FinancialTerms = new DetailedFinancialTermsDto
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
            LastModifiedAt = cycle.LastModifiedAt
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

    private static FacilityDetailsDto MapFacilityToDetailedDto(Domain.Entities.Facility facility)
    {
        return new FacilityDetailsDto
        {
            Id = facility.Id,
            Name = facility.Name,
            Code = facility.Code,
            Type = facility.Type.ToString(),
            TypeText = EnumTextMappingService.GetFacilityTypeDescription(facility.Type),
                Status = facility.Status.ToString(),
            StatusText = EnumTextMappingService.GetFacilityStatusDescription(facility.Status),
            Description = facility.Description,
            BankName = facility.BankName,
            BankCode = facility.BankCode,
            BankAccountNumber = facility.BankAccountNumber,
            Cycles = facility.Cycles.Select(MapCycleToDto).ToList(),
            Features = facility.Features.Select(f => new FacilityFeatureDto
            {
                Id = f.Id,
                FeatureId = f.FeatureId,
                RequirementType = f.RequirementType.ToString(),
                Notes = f.Notes,
                AssignedAt = f.AssignedAt
            }).ToList(),
            CapabilityPolicies = facility.CapabilityPolicies.Select(cp => new FacilityCapabilityPolicyDto
            {   
                Id = cp.Id,
                PolicyType = cp.PolicyType.ToString(),
                PolicyValue = cp.ModifierValue?.ToString() ?? string.Empty,
                Notes = cp.Notes,
                AssignedAt = cp.AssignedAt
            }).ToList(),
            Metadata = facility.Metadata,
            CreatedAt = facility.CreatedAt,
            LastModifiedAt = facility.LastModifiedAt,
            
       
        };
    }

    private static FacilityCycleDetailsDto MapCycleToDetailedDto(Domain.Entities.FacilityCycle cycle)
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
            FinancialTerms = new DetailedFinancialTermsDto
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
            Rules = new DetailedCycleRulesDto
            {
                IsRepeatable = cycle.IsRepeatable,
                IsExclusive = cycle.IsExclusive,
                ExclusiveSetId = cycle.ExclusiveSetId,
                MaxActiveAcrossCycles = cycle.MaxActiveAcrossCycles,
                HasDependencies = cycle.Dependencies.Any()
            },
            Dependencies = cycle.Dependencies.Select(d => new FacilityCycleDependencyDto
            {
                Id = d.Id,
                RequiredFacilityId = d.RequiredFacilityId,
                RequiredFacilityName = d.RequiredFacilityName,
                MustBeCompleted = d.MustBeCompleted,
                CreatedAt = d.CreatedAt
            }).ToList(),
            AdmissionStrategy = cycle.AdmissionStrategy,
            AdmissionStrategyDescription = EnumTextMappingService.GetAdmissionStrategyDescription(cycle.AdmissionStrategy),
            WaitlistCapacity = cycle.WaitlistCapacity,
            Metadata = cycle.Metadata,
            CreatedAt = cycle.CreatedAt,
            LastModifiedAt = cycle.LastModifiedAt
        };
    }

}
