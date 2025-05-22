using Nezam.Refahi.Domain.BoundedContexts.Shared.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Shared.Repositories;

namespace Nezam.Refahi.Application.Common.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{
    /// <summary>
    /// آغاز عملیات واحد کاری. در صورت پشتیبانی از تراکنش، آغاز آن را نیز شامل می‌شود.
    /// </summary>
    Task BeginAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// ذخیره تغییرات اعمال‌شده در جریان واحد کاری. این متد همیشه باید فراخوانی شود.
    /// </summary>
    Task SaveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// تایید و نهایی‌سازی عملیات واحد کاری. در صورت وجود تراکنش، commit انجام می‌شود.
    /// </summary>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// بازگرداندن وضعیت در صورت بروز خطا. در سیستم‌هایی که rollback دارند.
    /// </summary>
    Task RollbackAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت درگاه عمومی عملیات داده برای نوع مشخصی از مدل دامنه.
    /// </summary>
    IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity :  BaseEntity;
}