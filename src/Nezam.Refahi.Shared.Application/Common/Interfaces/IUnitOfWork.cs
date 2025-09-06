namespace Nezam.Refahi.Shared.Application.Common.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{
  Task BeginAsync(CancellationToken cancellationToken = default);
  Task SaveAsync(CancellationToken cancellationToken = default);
  Task CommitAsync(CancellationToken cancellationToken = default);
  Task RollbackAsync(CancellationToken cancellationToken = default);
}