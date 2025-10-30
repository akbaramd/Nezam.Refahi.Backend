using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Facilities.Application.Dtos;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycles;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityDetails;
using Nezam.Refahi.Facilities.Application.Services;
using Nezam.Refahi.Facilities.Domain.Entities;
using Nezam.Refahi.Facilities.Domain.Enums;

namespace Nezam.Refahi.Facilities.Application.Mappers;

public sealed class FacilityCycleWithUserDetailMapper : IMapper<FacilityCycle, FacilityCycleWithUserDetailDto>
{
  
  private readonly IMapper<FacilityCycle,CycleStatisticsDto> _facilityCycleDetailsMapper;

  public FacilityCycleWithUserDetailMapper(IMapper<FacilityCycle, CycleStatisticsDto> facilityCycleDetailsMapper)
  {
    _facilityCycleDetailsMapper = facilityCycleDetailsMapper;
  }

  public async Task<FacilityCycleWithUserDetailDto> MapAsync(FacilityCycle source, CancellationToken cancellationToken = new CancellationToken())
    {
        var now = DateTime.UtcNow;
        var daysUntilStart = (source.StartDate - now).Days;
        var daysUntilEnd = (source.EndDate - now).Days;
        var hasStarted = now >= source.StartDate;
        var hasEnded = now >= source.EndDate;
        var isActive = source.Status == FacilityCycleStatus.Active && hasStarted && !hasEnded;
        var isAcceptingApplications = isActive && source.UsedQuota < source.Quota;
        var quotaUtilizationPercentage = source.Quota > 0 ? (decimal)source.UsedQuota / source.Quota * 100 : 0;

        var dto = new FacilityCycleWithUserDetailDto
        {
            Id = source.Id,
            Name = source.Name,
            StartDate = source.StartDate,
            EndDate = source.EndDate,
            DaysUntilStart = Math.Max(0, daysUntilStart),
            DaysUntilEnd = Math.Max(0, daysUntilEnd),
            HasStarted = hasStarted,
            HasEnded = hasEnded,
            IsActive = isActive,
            IsAcceptingApplications = isAcceptingApplications,
            Quota = source.Quota,
            UsedQuota = source.UsedQuota,
            AvailableQuota = source.Quota - source.UsedQuota,
            QuotaUtilizationPercentage = quotaUtilizationPercentage,
            Status = source.Status.ToString(),
            StatusText= EnumTextMappingService.GetFacilityCycleStatusDescription(source.Status),
            Description = source.Description,
            FinancialTerms = new FinancialTermsDto
            {
                MinAmountRials = source.MinAmount?.AmountRials,
                MaxAmountRials = source.MaxAmount?.AmountRials,
                DefaultAmountRials = source.DefaultAmount?.AmountRials,
                Currency = source.MinAmount?.Currency ?? source.MaxAmount?.Currency ?? source.DefaultAmount?.Currency ?? "IRR",
                PaymentMonths = source.PaymentMonths,
                InterestRate = source.InterestRate,
                InterestRatePercentage = source.InterestRate.HasValue ? source.InterestRate.Value * 100 : null,
                CooldownDays = source.CooldownDays
            },
            Rules = new CycleRulesDto
            {
                IsRepeatable = source.IsRepeatable,
                IsExclusive = source.IsExclusive,
                ExclusiveSetId = source.ExclusiveSetId,
                MaxActiveAcrossCycles = source.MaxActiveAcrossCycles,
                HasDependencies = source.Dependencies.Any()
            },
            Dependencies = source.Dependencies.Select(d => new FacilityCycleDependencyDto
            {
                Id = d.Id,
                RequiredFacilityId = d.RequiredFacilityId,
                RequiredFacilityName = d.RequiredFacilityName,
                MustBeCompleted = d.MustBeCompleted,
                CreatedAt = d.CreatedAt
            }).ToList(),
            AdmissionStrategy = source.AdmissionStrategy,
            AdmissionStrategyDescription = EnumTextMappingService.GetAdmissionStrategyDescription(source.AdmissionStrategy),
            WaitlistCapacity = source.WaitlistCapacity,
            Metadata = source.Metadata,
            CreatedAt = source.CreatedAt,
            LastModifiedAt = source.LastModifiedAt,
            Statistics = await _facilityCycleDetailsMapper.MapAsync(source,cancellationToken)
        };

        return dto;
    }

    public Task MapAsync(FacilityCycle source, FacilityCycleWithUserDetailDto destination, CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.CompletedTask;
    }
}


