using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Facilities.Application.Dtos;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycles;
using Nezam.Refahi.Facilities.Application.Services;
using Nezam.Refahi.Facilities.Domain.Entities;
using Nezam.Refahi.Facilities.Domain.Enums;

namespace Nezam.Refahi.Facilities.Application.Mappers;

public sealed class FacilityCycleWithUserMapper : IMapper<FacilityCycle, FacilityCycleWithUserDto>
{
    private readonly IMapper<FacilityRequest, FacilityRequestDto> _requestMapper;

    public FacilityCycleWithUserMapper(IMapper<FacilityRequest, FacilityRequestDto> requestMapper)
    {
        _requestMapper = requestMapper;
    }

    public Task<FacilityCycleWithUserDto> MapAsync(FacilityCycle source, CancellationToken cancellationToken = new CancellationToken())
    {
        var now = DateTime.UtcNow;
        var daysUntilStart = (source.StartDate - now).Days;
        var daysUntilEnd = (source.EndDate - now).Days;
        var hasStarted = now >= source.StartDate;
        var hasEnded = now >= source.EndDate;
        var isActive = source.Status == FacilityCycleStatus.Active && hasStarted && !hasEnded;
        
        // Calculate UsedQuota from actual requests count (all requests, not just approved)
        var usedQuota = source.Requests.Count;
        var availableQuota = Math.Max(0, source.Quota - usedQuota);
        var isAcceptingApplications = isActive && usedQuota < source.Quota;
        var quotaUtilizationPercentage = source.Quota > 0 ? (decimal)usedQuota / source.Quota * 100 : 0;

        var dto = new FacilityCycleWithUserDto
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
            UsedQuota = usedQuota,
            AvailableQuota = availableQuota,
            QuotaUtilizationPercentage = quotaUtilizationPercentage,
            Status = source.Status.ToString(),
            StatusText = EnumTextMappingService.GetFacilityCycleStatusDescription(source.Status),
            Description = source.Description,
            ApprovalMessage = source.ApprovalMessage,
            FinancialTerms = new FinancialTermsDto
            {
                PriceOptions = source.PriceOptions
                    .Where(po => po.IsActive)
                    .OrderBy(po => po.DisplayOrder)
                    .Select(po => new FacilityCyclePriceOptionDto
                    {
                        Id = po.Id,
                        AmountRials = po.Amount.AmountRials,
                        Currency = po.Amount.Currency,
                        DisplayOrder = po.DisplayOrder,
                        Description = po.Description,
                        IsActive = po.IsActive
                    })
                    .ToList(),
                Currency = source.PriceOptions.FirstOrDefault()?.Amount.Currency ?? "IRR",
                PaymentMonths = source.PaymentMonths,
                InterestRate = source.InterestRate,
                InterestRatePercentage = source.InterestRate.HasValue ? source.InterestRate.Value * 100 : null
            },
            Rules = new CycleRulesDto
            {
                RestrictToPreviousCycles = source.RestrictToPreviousCycles,
                HasDependencies = source.Dependencies.Any()
            },
            RequiredFeatureIds = source.Features.Select(f => f.FeatureId).ToList(),
            RequiredCapabilityIds = source.Capabilities.Select(c => c.CapabilityId).ToList(),
            // User context intentionally omitted in mapper (kept null)
            UserEligibility = null,
            LastRequest = null,
            CreatedAt = source.CreatedAt
        };

        return Task.FromResult(dto);
    }

    public Task MapAsync(FacilityCycle source, FacilityCycleWithUserDto destination, CancellationToken cancellationToken = new CancellationToken())
    {
        // Prefer creating a new DTO instance; in-place mapping is not required here.
        return Task.CompletedTask;
    }
}


