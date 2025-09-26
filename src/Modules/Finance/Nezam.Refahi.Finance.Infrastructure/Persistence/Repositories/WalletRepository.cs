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
    private readonly FinanceDbContext _context;

    public WalletRepository(FinanceDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Wallet?> GetByExternalUserIdAsync(Guid externalUserId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(w => w.ExternalUserId == externalUserId, cancellationToken);
    }

    public async Task<bool> ExistsByExternalUserIdAsync(Guid externalUserId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(w => w.ExternalUserId == externalUserId, cancellationToken);
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
        Guid? externalUserId = null,
        WalletStatus? status = null,
        Money? minBalance = null,
        Money? maxBalance = null,
        DateTime? createdFrom = null,
        DateTime? createdTo = null,
        CancellationToken cancellationToken = default)
    {
        var query = PrepareQuery(_dbSet);

        if (externalUserId.HasValue)
            query = query.Where(w => w.ExternalUserId == externalUserId);

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

    /// <summary>
    /// Get wallets that don't have snapshots for a specific date
    /// </summary>
    public async Task<IEnumerable<Guid>> GetWalletsWithoutSnapshotForDateAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        try
        {
            var targetDate = date.Date;

            // Get all wallet IDs that don't have a snapshot for the specified date
            var walletsWithSnapshots = await _context.WalletSnapshots
                .Where(x => x.SnapshotDate == targetDate)
                .Select(x => x.WalletId)
                .ToListAsync(cancellationToken);

            var allWalletIds = await _context.Wallets
                .Select(w => w.Id)
                .ToListAsync(cancellationToken);

            return allWalletIds.Except(walletsWithSnapshots);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error getting wallets without snapshots for date {date:yyyy-MM-dd}", ex);
        }
    }

    /// <summary>
    /// Get the latest snapshot for a wallet
    /// </summary>
    public async Task<WalletSnapshot?> GetLatestSnapshotAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.WalletSnapshots
                .Where(x => x.WalletId == walletId)
                .OrderByDescending(x => x.SnapshotDate)
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error getting latest snapshot for wallet {walletId}", ex);
        }
    }

    /// <summary>
    /// Get transactions after a specific snapshot date
    /// </summary>
    public async Task<IEnumerable<WalletTransaction>> GetTransactionsAfterSnapshotAsync(
        Guid walletId, 
        DateTime? snapshotDate, 
        DateTime targetDate, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.WalletTransactions
                .Where(t => t.WalletId == walletId);

            if (snapshotDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt.Date > snapshotDate.Value.Date);
            }

            query = query.Where(t => t.CreatedAt.Date <= targetDate.Date);

            return await query
                .OrderBy(t => t.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error getting transactions after snapshot for wallet {walletId}", ex);
        }
    }

    /// <summary>
    /// Calculate wallet balance from snapshots and transactions
    /// </summary>
    public async Task<Money> CalculateWalletBalanceAsync(Guid walletId, DateTime? asOfDate = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var targetDate = asOfDate?.Date ?? DateTime.UtcNow.Date;

            // Get the latest snapshot before or on the target date
            var latestSnapshot = await GetLatestSnapshotAsync(walletId, cancellationToken);
            
            Money baseBalance = Money.Zero;
            DateTime? snapshotDate = null;

            if (latestSnapshot != null && latestSnapshot.SnapshotDate <= targetDate)
            {
                baseBalance = latestSnapshot.Balance;
                snapshotDate = latestSnapshot.SnapshotDate;
            }

            // Get transactions after the snapshot date
            var transactionsAfterSnapshot = await GetTransactionsAfterSnapshotAsync(walletId, snapshotDate, targetDate, cancellationToken);

            // Calculate balance by applying transactions to the snapshot balance
            var currentBalance = baseBalance;
            foreach (var transaction in transactionsAfterSnapshot)
            {
                if (transaction.IsIn())
                {
                    // Deposit, TransferIn, Refund, Interest, positive Adjustment
                    currentBalance = currentBalance.Add(transaction.Amount);
                }
                else if (transaction.IsOut())
                {
                    // Withdrawal, TransferOut, Payment, Fee, negative Adjustment
                    currentBalance = currentBalance.Subtract(transaction.Amount);
                }
            }

            return currentBalance;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error calculating balance for wallet {walletId}", ex);
        }
    }


    /// <summary>
    /// Get wallet with refreshed balance
    /// </summary>
    public async Task<Wallet?> GetByExternalUserIdWithRefreshedBalanceAsync(Guid externalUserId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Balance is now calculated dynamically, so we just return the wallet
            return await GetByExternalUserIdAsync(externalUserId, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error getting wallet for external user ID {externalUserId}", ex);
        }
    }

    /// <summary>
    /// Get wallet by ID with refreshed balance
    /// </summary>
    public async Task<Wallet?> GetByIdWithRefreshedBalanceAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Balance is now calculated dynamically, so we just return the wallet
            return await GetByIdAsync(walletId, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error getting wallet for ID {walletId}", ex);
        }
    }

    protected override IQueryable<Wallet> PrepareQuery(IQueryable<Wallet> query)
    {
        // Include snapshots and transactions for balance calculation
        return query
            .Include(w => w.Snapshots)
            .Include(w => w.Transactions);
    }
}