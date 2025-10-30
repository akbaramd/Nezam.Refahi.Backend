using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Facilities.Application.Dtos;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilities;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails;
using Nezam.Refahi.Facilities.Domain.Entities;
using Nezam.Refahi.Facilities.Domain.Enums;

namespace Nezam.Refahi.Facilities.Application.Mappers;

public sealed class FacilityCycleStatisticsMapper : IMapper<Facility, FacilityCycleStatisticsDto>
{
    public Task<FacilityCycleStatisticsDto> MapAsync(Facility source, CancellationToken cancellationToken = new CancellationToken())
    {
        var activeCycles = source.Cycles.Where(c => c.Status == FacilityCycleStatus.Active).ToList();
        var totalActiveQuota = activeCycles.Sum(c => c.Quota);
        var totalUsedQuota = activeCycles.Sum(c => c.UsedQuota);
        var totalAvailableQuota = totalActiveQuota - totalUsedQuota;
        var quotaUtilizationPercentage = totalActiveQuota > 0 ? (decimal)totalUsedQuota / totalActiveQuota * 100 : 0;

        var dto = new FacilityCycleStatisticsDto
        {
            ActiveCyclesCount = source.Cycles.Count(c => c.Status == FacilityCycleStatus.Active),
            TotalCyclesCount = source.Cycles.Count,
            DraftCyclesCount = source.Cycles.Count(c => c.Status == FacilityCycleStatus.Draft),
            ClosedCyclesCount = source.Cycles.Count(c => c.Status == FacilityCycleStatus.Closed),
            CompletedCyclesCount = source.Cycles.Count(c => c.Status == FacilityCycleStatus.Completed),
            CancelledCyclesCount = source.Cycles.Count(c => c.Status == FacilityCycleStatus.Cancelled),
            TotalActiveQuota = totalActiveQuota,
            TotalUsedQuota = totalUsedQuota,
            TotalAvailableQuota = totalAvailableQuota,
            QuotaUtilizationPercentage = quotaUtilizationPercentage
        };

        return Task.FromResult(dto);
    }

    public Task MapAsync(Facility source, FacilityCycleStatisticsDto destination, CancellationToken cancellationToken = new CancellationToken())
    {
        var activeCycles = source.Cycles.Where(c => c.Status == FacilityCycleStatus.Active).ToList();
        var totalActiveQuota = activeCycles.Sum(c => c.Quota);
        var totalUsedQuota = activeCycles.Sum(c => c.UsedQuota);
        var totalAvailableQuota = totalActiveQuota - totalUsedQuota;
        var quotaUtilizationPercentage = totalActiveQuota > 0 ? (decimal)totalUsedQuota / totalActiveQuota * 100 : 0;

        destination.ActiveCyclesCount = source.Cycles.Count(c => c.Status == FacilityCycleStatus.Active);
        destination.TotalCyclesCount = source.Cycles.Count;
        destination.DraftCyclesCount = source.Cycles.Count(c => c.Status == FacilityCycleStatus.Draft);
        destination.ClosedCyclesCount = source.Cycles.Count(c => c.Status == FacilityCycleStatus.Closed);
        destination.CompletedCyclesCount = source.Cycles.Count(c => c.Status == FacilityCycleStatus.Completed);
        destination.CancelledCyclesCount = source.Cycles.Count(c => c.Status == FacilityCycleStatus.Cancelled);
        destination.TotalActiveQuota = totalActiveQuota;
        destination.TotalUsedQuota = totalUsedQuota;
        destination.TotalAvailableQuota = totalAvailableQuota;
        destination.QuotaUtilizationPercentage = quotaUtilizationPercentage;

        return Task.CompletedTask;
    }
}


