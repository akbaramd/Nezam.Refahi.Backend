namespace Nezam.Refahi.Membership.Application.Services;

/// <summary>
/// Unit of Work pattern for Membership bounded context
/// Manages transactions and coordinates saving changes across repositories
/// </summary>
public interface IMembershipUnitOfWork : IDisposable
{
    /// <summary>
    /// Begins a new database transaction
    /// </summary>
    Task BeginAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the current transaction
    /// </summary>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the current transaction
    /// </summary>
    Task RollbackAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves all changes made in the current context
    /// </summary>
    Task<int> SaveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves all changes made in the current context
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}