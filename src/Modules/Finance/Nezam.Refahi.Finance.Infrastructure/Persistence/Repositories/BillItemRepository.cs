using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Finance.Infrastructure.Persistence;

namespace Nezam.Refahi.Finance.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for BillItem entity
/// </summary>
public class BillItemRepository : EfRepository<FinanceDbContext, BillItem, Guid>, IBillItemRepository
{
    public BillItemRepository(FinanceDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<BillItem>> GetByBillIdAsync(Guid billId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(bi => bi.BillId == billId)
            .OrderBy(bi => bi.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<BillItem>> GetByTitlePatternAsync(string titlePattern, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(bi => bi.Title.Contains(titlePattern))
            .OrderBy(bi => bi.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalAmountByBillIdAsync(Guid billId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(bi => bi.BillId == billId)
            .SumAsync(bi => bi.LineTotal.AmountRials, cancellationToken);
    }

    public async Task RemoveAllByBillIdAsync(Guid billId, CancellationToken cancellationToken = default)
    {
        var items = await PrepareQuery(_dbSet)
            .Where(bi => bi.BillId == billId)
            .ToListAsync(cancellationToken);

        if (items.Any())
        {
            _dbContext.Set<BillItem>().RemoveRange(items);
        }
    }

    protected override IQueryable<BillItem> PrepareQuery(IQueryable<BillItem> query)
    {
        return query;
    }
}