using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Finance.Infrastructure.Persistence;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Wallet entity
/// </summary>
public class WalletRepository : EfRepository<FinanceDbContext, Wallet, Guid>, IWalletRepository
{
    public WalletRepository(FinanceDbContext context) : base(context)
    {
    }

    public async Task<Wallet?> GetByNationalNumberAsync(string nationalNumber, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(w => w.NationalNumber == nationalNumber, cancellationToken);
    }

    public async Task<bool> ExistsByNationalNumberAsync(string nationalNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(w => w.NationalNumber == nationalNumber, cancellationToken);
    }

    public async Task<List<Wallet>> GetByStatusAsync(WalletStatus status, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(w => w.Status == status)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Wallet>> GetWithMinimumBalanceAsync(Money minimumBalance, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(w => w.Balance.AmountRials >= minimumBalance.AmountRials)
            .OrderByDescending(w => w.Balance.AmountRials)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Wallet>> GetActiveWalletsAsync(CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(w => w.Status == WalletStatus.Active)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Wallet>> GetWalletsWithRecentActivityAsync(int days = 7, CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-days);
        
        return await PrepareQuery(_dbSet)
            .Where(w => w.LastTransactionAt.HasValue && w.LastTransactionAt.Value >= cutoffDate)
            .OrderByDescending(w => w.LastTransactionAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Wallet>> GetWalletsByBalanceRangeAsync(Money minBalance, Money maxBalance, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(w => w.Balance.AmountRials >= minBalance.AmountRials && 
                       w.Balance.AmountRials <= maxBalance.AmountRials)
            .OrderByDescending(w => w.Balance.AmountRials)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalWalletBalanceAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(w => w.Status == WalletStatus.Active)
            .SumAsync(w => w.Balance.AmountRials, cancellationToken);
    }

    public async Task<Dictionary<WalletStatus, int>> GetWalletStatusStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var statistics = await _dbSet
            .GroupBy(w => w.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        return statistics.ToDictionary(s => s.Status, s => s.Count);
    }

    public async Task<List<Wallet>> SearchWalletsAsync(
        string? nationalNumber = null,
        WalletStatus? status = null,
        Money? minBalance = null,
        Money? maxBalance = null,
        DateTime? createdFrom = null,
        DateTime? createdTo = null,
        CancellationToken cancellationToken = default)
    {
        var query = PrepareQuery(_dbSet);

        if (!string.IsNullOrWhiteSpace(nationalNumber))
            query = query.Where(w => w.NationalNumber.Contains(nationalNumber));

        if (status.HasValue)
            query = query.Where(w => w.Status == status.Value);

        if (minBalance != null)
            query = query.Where(w => w.Balance.AmountRials >= minBalance.AmountRials);

        if (maxBalance != null)
            query = query.Where(w => w.Balance.AmountRials <= maxBalance.AmountRials);

        if (createdFrom.HasValue)
            query = query.Where(w => w.CreatedAt >= createdFrom.Value);

        if (createdTo.HasValue)
            query = query.Where(w => w.CreatedAt <= createdTo.Value);

        return await query
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    protected override IQueryable<Wallet> PrepareQuery(IQueryable<Wallet> query)
    {
        // Don't include transactions by default for better performance
        // Transactions should be accessed through WalletTransactionRepository
        return query;
    }
}