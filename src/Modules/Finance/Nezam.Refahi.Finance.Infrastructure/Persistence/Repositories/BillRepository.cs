using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Finance.Infrastructure.Persistence;

namespace Nezam.Refahi.Finance.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Bill entity
/// </summary>
public class BillRepository : EfRepository<FinanceDbContext, Bill, Guid>, IBillRepository
{
    public BillRepository(FinanceDbContext context) : base(context)
    {
    }

    public async Task<Bill?> GetByBillNumberAsync(string billNumber, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(b => b.BillNumber == billNumber, cancellationToken);
    }

    public async Task<Bill?> GetByReferenceAsync(string referenceId, string billType, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(b => b.ReferenceId == referenceId && b.BillType == billType, cancellationToken);
    }

    public async Task<IEnumerable<Bill>> GetByExternalUserIdAsync(Guid externalUserId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(b => b.ExternalUserId == externalUserId)
            .OrderByDescending(b => b.IssueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Bill>> GetByStatusAsync(BillStatus status, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(b => b.Status == status)
            .OrderByDescending(b => b.IssueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Bill>> GetByBillTypeAsync(string billType, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(b => b.BillType == billType)
            .OrderByDescending(b => b.IssueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Bill>> GetOverdueBillsAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await PrepareQuery(_dbSet)
            .Where(b => b.DueDate.HasValue &&
                       b.DueDate.Value < now &&
                       b.Status != BillStatus.FullyPaid &&
                       b.Status != BillStatus.Cancelled)
            .OrderByDescending(b => b.DueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Bill?> GetWithItemsAsync(Guid billId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(b => b.Id == billId, cancellationToken);
    }

    public async Task<Bill?> GetWithPaymentsAsync(Guid billId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(b => b.Id == billId, cancellationToken);
    }

    public async Task<Bill?> GetWithRefundsAsync(Guid billId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(b => b.Id == billId, cancellationToken);
    }

    public async Task<Bill?> GetWithAllDataAsync(Guid billId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(b => b.Id == billId, cancellationToken);
    }

    public async Task<IEnumerable<Bill>> GetInDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(b => b.IssueDate >= fromDate && b.IssueDate <= toDate)
            .OrderByDescending(b => b.IssueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalAmountAsync(BillStatus status, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default)
    {
        var query = PrepareQuery(_dbSet)
            .Where(b => b.Status == status);

        if (fromDate.HasValue)
            query = query.Where(b => b.IssueDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(b => b.IssueDate <= toDate.Value);

        return await query
            .SumAsync(b => b.TotalAmount.AmountRials, cancellationToken);
    }

    public async Task<Dictionary<BillStatus, int>> GetStatusStatisticsAsync(DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default)
    {
        var query = PrepareQuery(_dbSet);

        if (fromDate.HasValue)
            query = query.Where(b => b.IssueDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(b => b.IssueDate <= toDate.Value);

        var statistics = await query
            .GroupBy(b => b.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        return statistics.ToDictionary(s => s.Status, s => s.Count);
    }

    protected override IQueryable<Bill> PrepareQuery(IQueryable<Bill> query)
    {
        return query
            .Include(b => b.Items)
            .Include(b => b.Payments)
                .ThenInclude(p => p.Transactions)
            .Include(b => b.Refunds);
    }
}