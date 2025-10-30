using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Finance.Infrastructure.Persistence;

namespace Nezam.Refahi.Finance.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Payment entity
/// </summary>
public class PaymentRepository : EfRepository<FinanceDbContext, Payment, Guid>, IPaymentRepository
{
    public PaymentRepository(FinanceDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Payment>> GetByBillIdAsync(Guid billId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(p => p.BillId == billId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Payment?> GetByGatewayTransactionIdAsync(string gatewayTransactionId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(p => p.GatewayTransactionId == gatewayTransactionId, cancellationToken);
    }

    public async Task<Payment?> GetByGatewayReferenceAsync(string gatewayReference, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(p => p.GatewayReference == gatewayReference, cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetByStatusAsync(PaymentStatus status, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(p => p.Status == status)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetExpiredPaymentsAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await PrepareQuery(_dbSet)
            .Where(p => p.ExpiryDate.HasValue &&
                       p.ExpiryDate.Value < now &&
                       p.Status == PaymentStatus.Pending)
            .OrderBy(p => p.ExpiryDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Payment?> GetWithTransactionsAsync(Guid paymentId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(p => p.Id == paymentId, cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetByGatewayAsync(PaymentGateway gateway, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(p => p.Gateway == gateway)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetInDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(p => p.CreatedAt >= fromDate && p.CreatedAt <= toDate)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalAmountAsync(PaymentStatus status, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default)
    {
        var query = PrepareQuery(_dbSet)
            .Where(p => p.Status == status);

        if (fromDate.HasValue)
            query = query.Where(p => p.CreatedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(p => p.CreatedAt <= toDate.Value);

        return await query
            .SumAsync(p => p.Amount.AmountRials, cancellationToken);
    }

    protected override IQueryable<Payment> PrepareQuery(IQueryable<Payment> query)
    {
        return query.Include(x=>x.Bill).Include(p => p.Transactions);
    }
}