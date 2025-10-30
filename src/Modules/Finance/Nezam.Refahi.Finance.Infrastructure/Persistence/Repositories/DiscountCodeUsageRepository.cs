using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Finance.Infrastructure.Persistence;

namespace Nezam.Refahi.Finance.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for DiscountCodeUsage entity
/// </summary>
public class DiscountCodeUsageRepository : EfRepository<FinanceDbContext, DiscountCodeUsage, Guid>, IDiscountCodeUsageRepository
{
    public DiscountCodeUsageRepository(FinanceDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<DiscountCodeUsage>> GetByDiscountCodeIdAsync(Guid discountCodeId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(x => x.DiscountCodeId == discountCodeId)
            .OrderByDescending(x => x.UsedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<DiscountCodeUsage>> GetByBillIdAsync(Guid billId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(x => x.BillId == billId)
            .OrderByDescending(x => x.UsedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<DiscountCodeUsage>> GetByExternalUserIdAsync(Guid externalUserId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(x => x.ExternalUserId == externalUserId)
            .OrderByDescending(x => x.UsedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<DiscountCodeUsage>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(x => x.UsedAt >= fromDate && x.UsedAt <= toDate)
            .OrderByDescending(x => x.UsedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetUsageCountAsync(Guid discountCodeId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .CountAsync(x => x.DiscountCodeId == discountCodeId, cancellationToken);
    }

    public async Task<bool> HasUserUsedCodeAsync(Guid discountCodeId, Guid externalUserId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .AnyAsync(x => x.DiscountCodeId == discountCodeId && 
                          x.ExternalUserId == externalUserId, cancellationToken);
    }
}