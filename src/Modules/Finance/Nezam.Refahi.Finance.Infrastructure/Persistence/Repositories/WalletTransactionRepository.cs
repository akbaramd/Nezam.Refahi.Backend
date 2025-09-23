using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Finance.Infrastructure.Persistence;

namespace Nezam.Refahi.Finance.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for WalletTransaction entity
/// </summary>
public class WalletTransactionRepository : EfRepository<FinanceDbContext, WalletTransaction, Guid>, IWalletTransactionRepository
{
    public WalletTransactionRepository(FinanceDbContext context) : base(context)
    {
    }

    public async Task<List<WalletTransaction>> GetByWalletIdAsync(Guid walletId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var skip = (pageNumber - 1) * pageSize;
        
        return await PrepareQuery(_dbSet)
            .Where(t => t.WalletId == walletId)
            .OrderByDescending(t => t.CreatedAt)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<WalletTransaction>> GetByWalletIdAndDateRangeAsync(Guid walletId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(t => t.WalletId == walletId && 
                       t.CreatedAt >= startDate && 
                       t.CreatedAt <= endDate)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<WalletTransaction>> GetByWalletIdAndTypeAsync(Guid walletId, WalletTransactionType transactionType, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(t => t.WalletId == walletId && t.TransactionType == transactionType)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<WalletTransaction?> GetLatestByWalletIdAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(t => t.WalletId == walletId)
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<WalletTransaction>> GetByReferenceIdAsync(string referenceId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(t => t.ReferenceId == referenceId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<WalletTransaction>> GetByExternalReferenceAsync(string externalReference, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(t => t.ExternalReference == externalReference)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountByWalletIdAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(t => t.WalletId == walletId, cancellationToken);
    }

    public async Task<int> CountByWalletIdAndDateRangeAsync(Guid walletId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(t => t.WalletId == walletId && 
                           t.CreatedAt >= startDate && 
                           t.CreatedAt <= endDate, cancellationToken);
    }

    public async Task<Dictionary<WalletTransactionType, int>> GetTransactionTypeStatisticsAsync(Guid walletId, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(t => t.WalletId == walletId);

        if (startDate.HasValue)
            query = query.Where(t => t.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(t => t.CreatedAt <= endDate.Value);

        var statistics = await query
            .GroupBy(t => t.TransactionType)
            .Select(g => new { TransactionType = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        return statistics.ToDictionary(s => s.TransactionType, s => s.Count);
    }

    public async Task<List<(DateTime Date, decimal Balance)>> GetBalanceHistoryAsync(Guid walletId, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(t => t.WalletId == walletId);

        if (startDate.HasValue)
            query = query.Where(t => t.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(t => t.CreatedAt <= endDate.Value);

        var transactions = await query
            .OrderBy(t => t.CreatedAt)
            .Select(t => new { t.CreatedAt, t.BalanceAfter })
            .ToListAsync(cancellationToken);

        return transactions.Select(t => (t.CreatedAt.Date, t.BalanceAfter.AmountRials)).ToList();
    }

    protected override IQueryable<WalletTransaction> PrepareQuery(IQueryable<WalletTransaction> query)
    {
        return query
            .Include(t => t.Wallet); // Include wallet for navigation
    }
}
