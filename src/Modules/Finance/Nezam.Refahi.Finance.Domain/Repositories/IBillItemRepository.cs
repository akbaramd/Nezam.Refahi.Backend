using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Finance.Domain.Entities;

namespace Nezam.Refahi.Finance.Domain.Repositories;

/// <summary>
/// Repository interface for bill item entities
/// </summary>
public interface IBillItemRepository : IRepository<BillItem, Guid>
{
    /// <summary>
    /// Gets bill items by bill ID
    /// </summary>
    Task<IEnumerable<BillItem>> GetByBillIdAsync(Guid billId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets bill items by title pattern
    /// </summary>
    Task<IEnumerable<BillItem>> GetByTitlePatternAsync(string titlePattern, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total amount for bill items by bill ID
    /// </summary>
    Task<decimal> GetTotalAmountByBillIdAsync(Guid billId, CancellationToken cancellationToken = default);
}