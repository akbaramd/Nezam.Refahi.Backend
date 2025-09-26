using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Finance.Infrastructure.Persistence;

namespace Nezam.Refahi.Finance.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Refund entity
/// </summary>
public class RefundRepository : EfRepository<FinanceDbContext, Refund, Guid>, IRefundRepository
{
    public RefundRepository(FinanceDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Refund>> GetByBillIdAsync(Guid billId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(r => r.BillId == billId)
            .OrderByDescending(r => r.RequestedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Refund>> GetByStatusAsync(RefundStatus status, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(r => r.Status == status)
            .OrderByDescending(r => r.RequestedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Refund>> GetByRequesterAsync(Guid externalUserId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(r => r.RequestedByExternalUserId == externalUserId)
            .OrderByDescending(r => r.RequestedAt)  
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Refund>> GetInDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(r => r.RequestedAt >= fromDate && r.RequestedAt <= toDate)
            .OrderByDescending(r => r.RequestedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalRefundAmountAsync(RefundStatus status, DateTime? fromDate = null, DateTime? toDate = null,
      CancellationToken cancellationToken = default)
    {
        var query = PrepareQuery(_dbSet)
            .Where(r => r.Status == status);

        if (fromDate.HasValue)
            query = query.Where(r => r.RequestedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(r => r.RequestedAt <= toDate.Value);

        return await query
            .SumAsync(r => r.Amount.AmountRials, cancellationToken);
    }

    public async Task<IEnumerable<Refund>> GetPendingRefundsAsync(CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(r => r.Status == RefundStatus.Pending)
            .OrderBy(r => r.RequestedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Refund?> GetByGatewayRefundIdAsync(string gatewayRefundId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(r => r.GatewayRefundId == gatewayRefundId, cancellationToken);
    }

    protected override IQueryable<Refund> PrepareQuery(IQueryable<Refund> query)
    {
        return query;
    }
}