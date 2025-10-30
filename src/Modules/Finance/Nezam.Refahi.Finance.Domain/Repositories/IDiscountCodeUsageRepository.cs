using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Finance.Domain.Entities;

namespace Nezam.Refahi.Finance.Domain.Repositories;

/// <summary>
/// Repository interface for DiscountCodeUsage entity
/// </summary>
public interface IDiscountCodeUsageRepository : IRepository<DiscountCodeUsage, Guid>
{
    /// <summary>
    /// دریافت سابقه استفاده بر اساس شناسه کد تخفیف
    /// </summary>
    Task<IEnumerable<DiscountCodeUsage>> GetByDiscountCodeIdAsync(Guid discountCodeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت سابقه استفاده بر اساس شناسه فاکتور
    /// </summary>
    Task<IEnumerable<DiscountCodeUsage>> GetByBillIdAsync(Guid billId, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت سابقه استفاده بر اساس شناسه کاربر
    /// </summary>
    Task<IEnumerable<DiscountCodeUsage>> GetByExternalUserIdAsync(Guid externalUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت سابقه استفاده بر اساس بازه تاریخ
    /// </summary>
    Task<IEnumerable<DiscountCodeUsage>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت تعداد استفاده از کد تخفیف
    /// </summary>
    Task<int> GetUsageCountAsync(Guid discountCodeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// بررسی استفاده از کد تخفیف توسط کاربر
    /// </summary>
    Task<bool> HasUserUsedCodeAsync(Guid discountCodeId, Guid externalUserId, CancellationToken cancellationToken = default);


}
