using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Enums;

namespace Nezam.Refahi.Finance.Domain.Repositories;

/// <summary>
/// Repository interface for DiscountCode aggregate root
/// </summary>
public interface IDiscountCodeRepository : IRepository<DiscountCode, Guid>
{
    /// <summary>
    /// دریافت کد تخفیف بر اساس کد
    /// </summary>
    Task<DiscountCode?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت کد تخفیف بر اساس شناسه
    /// </summary>
    Task<IEnumerable<DiscountCode>> GetByStatusAsync(DiscountCodeStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت کدهای تخفیف فعال
    /// </summary>
    Task<IEnumerable<DiscountCode>> GetActiveCodesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت کدهای تخفیف منقضی شده
    /// </summary>
    Task<IEnumerable<DiscountCode>> GetExpiredCodesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت کدهای تخفیف بر اساس تاریخ
    /// </summary>
    Task<IEnumerable<DiscountCode>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت کدهای تخفیف بر اساس کاربر ایجادکننده
    /// </summary>
    Task<IEnumerable<DiscountCode>> GetByCreatedByAsync(Guid createdByExternalUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// بررسی وجود کد تخفیف
    /// </summary>
    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);


}
