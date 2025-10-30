using MCA.SharedKernel.Domain.Contracts;
using MCA.SharedKernel.Domain.Contracts.Repositories;

namespace Nezam.Refahi.Shared.Application.Common.Interfaces;

/// <summary>
/// Base Unit of Work pattern interface with domain event handling
/// Manages transactions and coordinates saving changes across repositories with domain event publishing
/// Integration Events are handled separately through IOutboxPublisher
/// </summary>
public interface IUnitOfWork : IDisposable
{
  /// <summary>
  /// Begins a new database transaction
  /// </summary>
  Task BeginAsync(CancellationToken cancellationToken = default);

  /// <summary>
  /// Commits the current transaction and publishes domain events
  /// </summary>
  Task CommitAsync(CancellationToken cancellationToken = default);

  /// <summary>
  /// Rolls back the current transaction and clears domain events
  /// </summary>
  Task RollbackAsync(CancellationToken cancellationToken = default);

  /// <summary>
  /// Saves all changes made in the current context without committing transaction
  /// </summary>
  Task<int> SaveAsync(CancellationToken cancellationToken = default);

  /// <summary>
  /// Saves all changes made in the current context and publishes domain events
  /// </summary>
  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

  /// <summary>
  /// Adds a domain event to be published when transaction commits
  /// </summary>
  void AddDomainEvent(IDomainEvent domainEvent);

  /// <summary>
  /// Gets all pending domain events
  /// </summary>
  IReadOnlyList<IDomainEvent> GetDomainEvents();

  /// <summary>
  /// Clears all pending domain events
  /// </summary>
  void ClearDomainEvents();
}
