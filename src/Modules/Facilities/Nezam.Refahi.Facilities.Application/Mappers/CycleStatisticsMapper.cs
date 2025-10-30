using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Facilities.Application.Dtos;
using Nezam.Refahi.Facilities.Domain.Entities;
using Nezam.Refahi.Facilities.Domain.Enums;

namespace Nezam.Refahi.Facilities.Application.Mappers;

/// <summary>
/// Mapper for converting FacilityCycle entity to CycleStatisticsDto
/// Provides comprehensive statistics for a specific facility cycle
/// </summary>
public sealed class CycleStatisticsMapper : IMapper<FacilityCycle, CycleStatisticsDto>
{
    /// <summary>
    /// Maps FacilityCycle entity to CycleStatisticsDto
    /// </summary>
    /// <param name="source">Source FacilityCycle entity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Mapped CycleStatisticsDto</returns>
    public Task<CycleStatisticsDto> MapAsync(FacilityCycle source, CancellationToken cancellationToken = new CancellationToken())
    {
        var now = DateTime.UtcNow;
        
        // Calculate quota statistics
        var totalQuota = source.Quota;
        var usedQuota = source.UsedQuota;
        var availableQuota = Math.Max(0, totalQuota - usedQuota);
        var utilizationPercentage = totalQuota > 0 ? (decimal)usedQuota / totalQuota * 100 : 0;

        // Calculate request statistics
        var applications = source.Applications.ToList();
        var pendingRequests = applications.Count(a => a.Status == FacilityRequestStatus.PendingApproval);
        var approvedRequests = applications.Count(a => a.Status == FacilityRequestStatus.Approved);
        var rejectedRequests = applications.Count(a => a.Status == FacilityRequestStatus.Rejected);

        // Calculate average processing time for completed requests
        var completedRequests = applications.Where(a => 
            a.Status == FacilityRequestStatus.Approved || a.Status == FacilityRequestStatus.Rejected)
            .ToList();
        
        decimal? averageProcessingTimeDays = null;
        if (completedRequests.Any())
        {
            var totalProcessingDays = completedRequests.Sum(r =>
            {
                var completionDate = r.ApprovedAt ?? r.RejectedAt ?? now;
                return (completionDate - r.CreatedAt).Days;
            });
            averageProcessingTimeDays = (decimal)totalProcessingDays / completedRequests.Count;
        }

        // Calculate time-based statistics
        var cycleDurationDays = (source.EndDate - source.StartDate).Days;
        var daysElapsed = Math.Max(0, (now - source.StartDate).Days);
        var daysRemaining = Math.Max(0, (source.EndDate - now).Days);
        var cycleProgressPercentage = cycleDurationDays > 0 ? 
            Math.Min(100, (decimal)daysElapsed / cycleDurationDays * 100) : 0;

        var dto = new CycleStatisticsDto
        {
            TotalQuota = totalQuota,
            UsedQuota = usedQuota,
            AvailableQuota = availableQuota,
            UtilizationPercentage = utilizationPercentage,
            PendingRequests = pendingRequests,
            ApprovedRequests = approvedRequests,
            RejectedRequests = rejectedRequests,
            AverageProcessingTimeDays = averageProcessingTimeDays,
            CycleDurationDays = cycleDurationDays,
            DaysElapsed = daysElapsed,
            DaysRemaining = daysRemaining,
            CycleProgressPercentage = cycleProgressPercentage
        };

        return Task.FromResult(dto);
    }

    /// <summary>
    /// Maps FacilityCycle entity to existing CycleStatisticsDto instance
    /// </summary>
    /// <param name="source">Source FacilityCycle entity</param>
    /// <param name="destination">Destination CycleStatisticsDto</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the mapping operation</returns>
    public Task MapAsync(FacilityCycle source, CycleStatisticsDto destination, CancellationToken cancellationToken = new CancellationToken())
    {
        var now = DateTime.UtcNow;
        
        // Calculate quota statistics
        var totalQuota = source.Quota;
        var usedQuota = source.UsedQuota;
        var availableQuota = Math.Max(0, totalQuota - usedQuota);
        var utilizationPercentage = totalQuota > 0 ? (decimal)usedQuota / totalQuota * 100 : 0;

        // Calculate request statistics
        var applications = source.Applications.ToList();
        var pendingRequests = applications.Count(a => a.Status == FacilityRequestStatus.PendingApproval);
        var approvedRequests = applications.Count(a => a.Status == FacilityRequestStatus.Approved);
        var rejectedRequests = applications.Count(a => a.Status == FacilityRequestStatus.Rejected);

        // Calculate average processing time for completed requests
        var completedRequests = applications.Where(a => 
            a.Status == FacilityRequestStatus.Approved || a.Status == FacilityRequestStatus.Rejected)
            .ToList();
        
        decimal? averageProcessingTimeDays = null;
        if (completedRequests.Any())
        {
            var totalProcessingDays = completedRequests.Sum(r =>
            {
                var completionDate = r.ApprovedAt ?? r.RejectedAt ?? now;
                return (completionDate - r.CreatedAt).Days;
            });
            averageProcessingTimeDays = (decimal)totalProcessingDays / completedRequests.Count;
        }

        // Calculate time-based statistics
        var cycleDurationDays = (source.EndDate - source.StartDate).Days;
        var daysElapsed = Math.Max(0, (now - source.StartDate).Days);
        var daysRemaining = Math.Max(0, (source.EndDate - now).Days);
        var cycleProgressPercentage = cycleDurationDays > 0 ? 
            Math.Min(100, (decimal)daysElapsed / cycleDurationDays * 100) : 0;

        // Note: CycleStatisticsDto is a record with init-only properties
        // This method is provided for interface compliance but cannot modify the destination
        // In practice, use the MapAsync(source, cancellationToken) overload for record DTOs
        return Task.CompletedTask;
    }
}
