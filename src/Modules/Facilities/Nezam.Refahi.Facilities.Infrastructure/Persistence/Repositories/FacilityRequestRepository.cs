using Microsoft.EntityFrameworkCore;
using MCA.SharedKernel.Infrastructure.Repositories;
using Nezam.Refahi.Facilities.Domain.Entities;
using Nezam.Refahi.Facilities.Domain.Enums;
using Nezam.Refahi.Facilities.Domain.Repositories;
using Nezam.Refahi.Facilities.Infrastructure.Persistence;

namespace Nezam.Refahi.Facilities.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for FacilityRequest entity
/// </summary>
public class FacilityRequestRepository : EfRepository<FacilitiesDbContext, FacilityRequest, Guid>, IFacilityRequestRepository
{
    public FacilityRequestRepository(FacilitiesDbContext context) : base(context)
    {
    }

    protected override IQueryable<FacilityRequest> PrepareQuery(IQueryable<FacilityRequest> query)
    {
        return query
            .Include(r => r.Facility)
                .ThenInclude(f => f.Features)
            .Include(r => r.Facility)
                .ThenInclude(f => f.CapabilityPolicies)
            .Include(r => r.FacilityCycle)
                .ThenInclude(c => c.Dependencies);
    }

    public async Task<IEnumerable<FacilityRequest>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(r => r.MemberId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<FacilityRequest>> GetByCycleIdAsync(Guid cycleId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(r => r.FacilityCycleId == cycleId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<FacilityRequest>> GetByFacilityIdAsync(Guid facilityId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(r => r.FacilityId == facilityId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<FacilityRequest>> GetByStatusAsync(FacilityRequestStatus status, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(r => r.Status == status)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<FacilityRequest>> GetPendingBankProcessingAsync(CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(r => r.Status == FacilityRequestStatus.Approved)
            .OrderBy(r => r.ApprovedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<FacilityRequest>> GetWithBankAppointmentsAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(r => r.BankAppointmentDate.HasValue && 
                       r.BankAppointmentDate.Value.Date == date.Date)
            .OrderBy(r => r.BankAppointmentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<FacilityRequest>> GetCompletedApplicationsAsync(CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(r => r.Status == FacilityRequestStatus.Completed)
            .OrderByDescending(r => r.CompletedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<FacilityRequest>> GetRejectedApplicationsAsync(CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(r => r.Status == FacilityRequestStatus.Rejected)
            .OrderByDescending(r => r.RejectedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasActiveApplicationAsync(Guid userId, Guid facilityId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .AnyAsync(r => r.MemberId == userId && 
                          r.FacilityId == facilityId && 
                          (r.Status == FacilityRequestStatus.PendingApproval || 
                           r.Status == FacilityRequestStatus.UnderReview || 
                           r.Status == FacilityRequestStatus.Approved), cancellationToken);
    }

    public async Task<FacilityRequest?> GetByApplicationNumberAsync(string applicationNumber, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(r => r.RequestNumber == applicationNumber, cancellationToken);
    }

    public async Task<FacilityRequest?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(r => r.IdempotencyKey == idempotencyKey, cancellationToken);
    }

    public async Task<IEnumerable<FacilityRequest>> GetByCorrelationIdAsync(string correlationId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(r => r.CorrelationId == correlationId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<FacilityRequest>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(r => r.CreatedAt >= startDate && r.CreatedAt <= endDate)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<ApplicationStatistics> GetApplicationStatisticsAsync(Guid? facilityId = null, Guid? cycleId = null, CancellationToken cancellationToken = default)
    {
        var query = PrepareQuery(_dbSet);

        if (facilityId.HasValue)
        {
            query = query.Where(r => r.FacilityId == facilityId.Value);
        }

        if (cycleId.HasValue)
        {
            query = query.Where(r => r.FacilityCycleId == cycleId.Value);
        }

        var statistics = new ApplicationStatistics
        {
            TotalApplications = await query.CountAsync(cancellationToken),
            SubmittedApplications = await query.CountAsync(r => r.Status == FacilityRequestStatus.PendingApproval, cancellationToken),
            UnderReviewApplications = await query.CountAsync(r => r.Status == FacilityRequestStatus.UnderReview, cancellationToken),
            ApprovedApplications = await query.CountAsync(r => r.Status == FacilityRequestStatus.Approved, cancellationToken),
            RejectedApplications = await query.CountAsync(r => r.Status == FacilityRequestStatus.Rejected, cancellationToken),
            CancelledApplications = await query.CountAsync(r => r.Status == FacilityRequestStatus.Cancelled, cancellationToken),
            CompletedApplications = await query.CountAsync(r => r.Status == FacilityRequestStatus.Completed, cancellationToken),
            PendingBankProcessing = await query.CountAsync(r => r.Status == FacilityRequestStatus.Approved, cancellationToken),
            TotalRequestedAmount = await query.SumAsync(r => r.RequestedAmount.AmountRials, cancellationToken),
            TotalApprovedAmount = await query.Where(r => r.ApprovedAmount != null).SumAsync(r => r.ApprovedAmount!.AmountRials, cancellationToken)
        };

        return statistics;
    }

    public async Task<IEnumerable<FacilityRequest>> GetFacilityRequestsAsync(FacilityRequestQueryParameters parameters, CancellationToken cancellationToken = default)
    {
        var query = PrepareQuery(_dbSet);

        // Apply filters
        if (parameters.FacilityId.HasValue)
        {
            query = query.Where(r => r.FacilityId == parameters.FacilityId.Value);
        }

        if (parameters.FacilityCycleId.HasValue)
        {
            query = query.Where(r => r.FacilityCycleId == parameters.FacilityCycleId.Value);
        }

        if (parameters.MemberId.HasValue)
        {
            query = query.Where(r => r.MemberId == parameters.MemberId.Value);
        }

        if (!string.IsNullOrEmpty(parameters.Status) && Enum.TryParse<FacilityRequestStatus>(parameters.Status, out var status))
        {
            query = query.Where(r => r.Status == status);
        }

        if (!string.IsNullOrEmpty(parameters.SearchTerm))
        {
            var searchTerm = parameters.SearchTerm.ToLower();
            query = query.Where(r => r.RequestNumber!.ToLower().Contains(searchTerm) ||
                                    r.UserFullName!.ToLower().Contains(searchTerm) ||
                                    (r.UserNationalId != null && r.UserNationalId.ToLower().Contains(searchTerm)));
        }

        if (parameters.DateFrom.HasValue)
        {
            query = query.Where(r => r.CreatedAt >= parameters.DateFrom.Value);
        }

        if (parameters.DateTo.HasValue)
        {
            query = query.Where(r => r.CreatedAt <= parameters.DateTo.Value);
        }

        // Apply pagination
        var skip = (parameters.Page - 1) * parameters.PageSize;
        return await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip(skip)
            .Take(parameters.PageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetFacilityRequestsCountAsync(FacilityRequestQueryParameters parameters, CancellationToken cancellationToken = default)
    {
        var query = PrepareQuery(_dbSet);

        // Apply filters
        if (parameters.FacilityId.HasValue)
        {
            query = query.Where(r => r.FacilityId == parameters.FacilityId.Value);
        }

        if (parameters.FacilityCycleId.HasValue)
        {
            query = query.Where(r => r.FacilityCycleId == parameters.FacilityCycleId.Value);
        }

        if (parameters.MemberId.HasValue)
        {
            query = query.Where(r => r.MemberId == parameters.MemberId.Value);
        }

        if (!string.IsNullOrEmpty(parameters.Status) && Enum.TryParse<FacilityRequestStatus>(parameters.Status, out var status))
        {
            query = query.Where(r => r.Status == status);
        }

        if (!string.IsNullOrEmpty(parameters.SearchTerm))
        {
            var searchTerm = parameters.SearchTerm.ToLower();
            query = query.Where(r => r.RequestNumber!.ToLower().Contains(searchTerm) ||
                                    r.UserFullName!.ToLower().Contains(searchTerm) ||
                                    (r.UserNationalId != null && r.UserNationalId.ToLower().Contains(searchTerm)));
        }

        if (parameters.DateFrom.HasValue)
        {
            query = query.Where(r => r.CreatedAt >= parameters.DateFrom.Value);
        }

        if (parameters.DateTo.HasValue)
        {
            query = query.Where(r => r.CreatedAt <= parameters.DateTo.Value);
        }

        return await query.CountAsync(cancellationToken);
    }

    public async Task<FacilityRequest?> GetByIdWithDetailsAsync(Guid requestId, bool includeFacility = true, bool includeCycle = true, CancellationToken cancellationToken = default)
    {
        // Since PrepareQuery already includes all relationships, we can use it directly
        // The parameters are kept for API compatibility but don't affect the query
        return await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(r => r.Id == requestId, cancellationToken);
    }

    public async Task<bool> HasUserRequestForCycleAsync(Guid userId, Guid cycleId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.MemberId == userId && r.FacilityCycleId == cycleId)
            .AnyAsync(cancellationToken);
    }

    public async Task<FacilityRequest?> GetUserRequestForCycleAsync(Guid userId, Guid cycleId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(r => r.MemberId == userId && r.FacilityCycleId == cycleId)
            .OrderByDescending(r => r.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Dictionary<Guid, FacilityRequest>> GetUserRequestsForCyclesAsync(Guid userId, IEnumerable<Guid> cycleIds, CancellationToken cancellationToken = default)
    {
        var cycleIdsList = cycleIds.ToList();
        if (!cycleIdsList.Any())
            return new Dictionary<Guid, FacilityRequest>();

        var requests = await PrepareQuery(_dbSet)
            .Where(r => r.MemberId == userId && cycleIdsList.Contains(r.FacilityCycleId))
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

        // Group by cycle and take the most recent request for each cycle
        return requests
            .GroupBy(r => r.FacilityCycleId)
            .ToDictionary(g => g.Key, g => g.First());
    }

    public async Task<HashSet<Guid>> GetCyclesWithUserRequestsAsync(Guid userId, IEnumerable<Guid> cycleIds, CancellationToken cancellationToken = default)
    {
        var cycleIdsList = cycleIds.ToList();
        if (!cycleIdsList.Any())
            return new HashSet<Guid>();

        var cycleIdsWithRequests = await _dbSet
            .Where(r => r.MemberId == userId && cycleIdsList.Contains(r.FacilityCycleId))
            .Select(r => r.FacilityCycleId)
            .Distinct()
            .ToListAsync(cancellationToken);

        return cycleIdsWithRequests.ToHashSet();
    }
}
