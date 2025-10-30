using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Finance.Infrastructure.Persistence;

namespace Nezam.Refahi.Finance.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for DiscountCode aggregate root
/// </summary>
public class DiscountCodeRepository : EfRepository<FinanceDbContext, DiscountCode, Guid>, IDiscountCodeRepository
{
    public DiscountCodeRepository(FinanceDbContext context) : base(context)
    {
    }

    public async Task<DiscountCode?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        return await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(x => x.Code == code.Trim().ToUpperInvariant(), cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            return false;

        return await PrepareQuery(_dbSet)
            .AnyAsync(x => x.Code == code.Trim().ToUpperInvariant(), cancellationToken);
    }

    public async Task<IEnumerable<DiscountCode>> GetActiveDiscountCodesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await PrepareQuery(_dbSet)
            .Where(x => x.IsActive && 
                       x.ValidFrom <= now && 
                       x.ValidTo >= now)
            .OrderBy(x => x.ValidTo)
            .ToListAsync(cancellationToken);
    }

    protected override IQueryable<DiscountCode> PrepareQuery(IQueryable<DiscountCode> query)
    {
        return query.Include(x => x.Usages);
    }

    public async Task<IEnumerable<DiscountCode>> GetByStatusAsync(DiscountCodeStatus status, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<DiscountCode>> GetActiveCodesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await PrepareQuery(_dbSet)
            .Where(x => x.IsActive && 
                       x.Status == DiscountCodeStatus.Active &&
                       x.ValidFrom <= now && 
                       x.ValidTo >= now)
            .OrderBy(x => x.ValidTo)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<DiscountCode>> GetExpiredCodesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await PrepareQuery(_dbSet)
            .Where(x => x.ValidTo < now || x.Status == DiscountCodeStatus.Expired)
            .OrderByDescending(x => x.ValidTo)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<DiscountCode>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(x => x.CreatedAt >= fromDate && x.CreatedAt <= toDate)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<DiscountCode>> GetByCreatedByAsync(Guid createdByExternalUserId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(x => x.CreatedByExternalUserId == createdByExternalUserId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }


}