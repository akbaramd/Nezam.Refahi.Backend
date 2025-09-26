using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Repositories;
using MCA.SharedKernel.Infrastructure.Repositories;

namespace Nezam.Refahi.Finance.Infrastructure.Persistence.Repositories;

/// <summary>
/// Entity Framework implementation of IWalletSnapshotRepository
/// </summary>
public class WalletSnapshotRepository : EfRepository<FinanceDbContext, WalletSnapshot, Guid>, IWalletSnapshotRepository
{
    public WalletSnapshotRepository(FinanceDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<WalletSnapshot>> GetByWalletIdAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(x => x.WalletId == walletId)
            .OrderByDescending(x => x.SnapshotDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WalletSnapshot>> GetByExternalUserIdAsync(Guid externalUserId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(x => x.ExternalUserId == externalUserId)
            .OrderByDescending(x => x.SnapshotDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<WalletSnapshot?> GetLatestByWalletIdAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(x => x.WalletId == walletId)
            .OrderByDescending(x => x.SnapshotDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<WalletSnapshot?> GetLatestByExternalUserIdAsync(Guid externalUserId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(x => x.ExternalUserId == externalUserId)
            .OrderByDescending(x => x.SnapshotDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<WalletSnapshot?> GetByWalletIdAndDateAsync(Guid walletId, DateTime date, CancellationToken cancellationToken = default)
    {
        var targetDate = date.Date;
        return await PrepareQuery(_dbSet)
            .Where(x => x.WalletId == walletId && x.SnapshotDate == targetDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<WalletSnapshot>> GetByWalletIdAndDateRangeAsync(
        Guid walletId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        var fromDateOnly = fromDate.Date;
        var toDateOnly = toDate.Date;

        return await PrepareQuery(_dbSet)
            .Where(x => x.WalletId == walletId && 
                       x.SnapshotDate >= fromDateOnly && 
                       x.SnapshotDate <= toDateOnly)
            .OrderByDescending(x => x.SnapshotDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WalletSnapshot>> GetByUserAndDateRangeAsync(
        Guid externalUserId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        var fromDateOnly = fromDate.Date;
        var toDateOnly = toDate.Date;

        return await PrepareQuery(_dbSet)
            .Where(x => x.ExternalUserId == externalUserId && 
                       x.SnapshotDate >= fromDateOnly && 
                       x.SnapshotDate <= toDateOnly)
            .OrderByDescending(x => x.SnapshotDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByWalletIdAndDateAsync(Guid walletId, DateTime date, CancellationToken cancellationToken = default)
    {
        var targetDate = date.Date;
        return await PrepareQuery(_dbSet)
            .AnyAsync(x => x.WalletId == walletId && x.SnapshotDate == targetDate, cancellationToken);
    }

    public async Task<IEnumerable<Guid>> GetWalletsWithoutSnapshotForDateAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        var targetDate = date.Date;

        // Get all wallet IDs that don't have a snapshot for the specified date
        var walletsWithSnapshots = await PrepareQuery(_dbSet)
            .Where(x => x.SnapshotDate == targetDate)
            .Select(x => x.WalletId)
            .ToListAsync(cancellationToken);

        var allWalletIds = await _dbContext.Wallets
            .Select(w => w.Id)
            .ToListAsync(cancellationToken);

        return allWalletIds.Except(walletsWithSnapshots);
    }

    protected override IQueryable<WalletSnapshot> PrepareQuery(IQueryable<WalletSnapshot> query)
    {
        return query
            .Include(x => x.Wallet);
    }
}
