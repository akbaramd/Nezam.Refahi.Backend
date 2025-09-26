using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Finance.Domain.Repositories;
using MCA.SharedKernel.Infrastructure.Repositories;

namespace Nezam.Refahi.Finance.Infrastructure.Persistence.Repositories;

/// <summary>
/// Entity Framework implementation of IWalletDepositRepository
/// </summary>
public class WalletDepositRepository : EfRepository<FinanceDbContext, WalletDeposit, Guid>, IWalletDepositRepository
{
    public WalletDepositRepository(FinanceDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<WalletDeposit>> GetByWalletIdAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(x => x.WalletId == walletId)
            .OrderByDescending(x => x.RequestedAt)
            .ToListAsync(cancellationToken);
    }
   public async Task<WalletDeposit?> GetByTrackingCodeAsync(string trackingCode, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(x => x.TrackingCode == trackingCode)
            .OrderByDescending(x => x.RequestedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<WalletDeposit>> GetByExternalUserIdAsync(Guid externalUserId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(x => x.ExternalUserId == externalUserId)
            .OrderByDescending(x => x.RequestedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WalletDeposit>> GetByStatusAsync(WalletDepositStatus status, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.RequestedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<WalletDeposit?> GetByExternalReferenceAsync(string externalReference, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(x => x.ExternalReference == externalReference, cancellationToken);
    }

    public async Task<IEnumerable<WalletDeposit>> GetPendingDepositsAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(x => x.WalletId == walletId && x.Status == WalletDepositStatus.Pending)
            .OrderBy(x => x.RequestedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WalletDeposit>> GetDepositsByDateRangeAsync(
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(x => x.RequestedAt >= fromDate && x.RequestedAt <= toDate)
            .OrderByDescending(x => x.RequestedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WalletDeposit>> GetDepositsByUserAndDateRangeAsync(
        Guid externalUserId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(x => x.ExternalUserId == externalUserId &&
                       x.RequestedAt >= fromDate &&
                       x.RequestedAt <= toDate)
            .OrderByDescending(x => x.RequestedAt)
            .ToListAsync(cancellationToken);
    }
    
    protected override IQueryable<WalletDeposit> PrepareQuery(IQueryable<WalletDeposit> query)
    {
        return query
            .Include(x => x.Wallet);
    }
}
