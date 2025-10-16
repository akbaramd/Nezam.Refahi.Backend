using MCA.SharedKernel.Domain.AggregateRoots;
using MCA.SharedKernel.Domain.Contracts;
using MCA.SharedKernel.Domain.Contracts.AggregateRoots;
using MCA.SharedKernel.Domain.Contracts.Repositories;
using MCA.SharedKernel.Domain.Events;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Shared.Application.Common.Interfaces;

namespace Nezam.Refahi.Shared.Infrastructure.Persistence;

/// <summary>
/// Base implementation of Unit of Work pattern with domain event publishing
/// Manages database transactions and coordinates domain event publishing through MediatR
/// </summary>
public abstract class BaseUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
{
    protected readonly TContext _context;
    protected readonly IMediator _mediator;
    protected readonly ILogger _logger;
    private readonly List<IDomainEvent> _domainEvents = new();
    private IDbContextTransaction? _currentTransaction;
    private bool _disposed;

    protected BaseUnitOfWork(TContext context, IMediator mediator, ILogger logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task BeginAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            _logger.LogWarning("Transaction already began. Cannot begin a new transaction.");
            return;
        }

        _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        _logger.LogDebug("Database transaction began");
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null)
        {
            _logger.LogWarning("No active transaction to commit");
            return;
        }
        
        try
        {
            // Collect domain events from aggregate roots before committing
            await CollectDomainEventsFromEntitiesAsync();
            
            // Commit the transaction
            await _currentTransaction.CommitAsync(cancellationToken);
            _logger.LogDebug("Database transaction committed");

            // Publish domain events after successful commit
            await PublishDomainEventsAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to commit transaction");
            await RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null)
        {
            _logger.LogWarning("No active transaction to rollback");
            return;
        }

        try
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
            _logger.LogDebug("Database transaction rolled back");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to rollback transaction");
            throw;
        }
        finally
        {
            // Clear domain events on rollback
            ClearDomainEvents();
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
    {
        // Collect domain events but don't publish them yet
        await CollectDomainEventsFromEntitiesAsync();
        
        try
        {
            var changes = await _context.SaveChangesAsync(cancellationToken);
            _logger.LogDebug("Saved {Changes} changes to database", changes);
            return changes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save changes to database");
            throw;
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Collect domain events from aggregate roots
        await CollectDomainEventsFromEntitiesAsync();
        
        try
        {
            var changes = await _context.SaveChangesAsync(cancellationToken);
            _logger.LogDebug("Saved {Changes} changes to database", changes);

            // Publish domain events after successful save
            await PublishDomainEventsAsync(cancellationToken);

            return changes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save changes to database");
            // Clear events on failure
            ClearDomainEvents();
            
            // Provide more specific error information for constraint violations
            if (ex is DbUpdateException dbEx && dbEx.InnerException is SqlException sqlEx)
            {
                switch (sqlEx.Number)
                {
                    case 2627: // Primary key constraint violation
                        _logger.LogWarning("Primary key constraint violation: {Message}", sqlEx.Message);
                        break;
                    case 2601: // Unique constraint violation
                        _logger.LogWarning("Unique constraint violation: {Message}", sqlEx.Message);
                        break;
                    case 547: // Foreign key constraint violation
                        _logger.LogWarning("Foreign key constraint violation: {Message}", sqlEx.Message);
                        break;
                    default:
                        _logger.LogError(sqlEx, "SQL Server error {Number}: {Message}", sqlEx.Number, sqlEx.Message);
                        break;
                }
            }
            
            throw;
        }
    }

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        if (domainEvent != null)
        {
            _domainEvents.Add(domainEvent);
        }
    }

    public IReadOnlyList<IDomainEvent> GetDomainEvents()
    {
        return _domainEvents.AsReadOnly();
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
        
        // Also clear events from aggregate roots
        ClearDomainEventsFromEntities();
    }

    /// <summary>
    /// Collects domain events from all aggregate roots in the context
    /// </summary>
    private Task CollectDomainEventsFromEntitiesAsync()
    {
        var aggregateRoots = _context.ChangeTracker.Entries<IAggregateRoot>()
            .Where(entry => entry.Entity.DomainEvents.Any())
            .Select(entry => entry.Entity)
            .ToList();

        var allEvents = aggregateRoots
            .SelectMany(root => root.DomainEvents)
            .ToList();

        foreach (var domainEvent in allEvents)
        {
            if (!_domainEvents.Contains(domainEvent))
            {
                _domainEvents.Add(domainEvent);
            }
        }

        _logger.LogDebug("Collected {EventCount} domain events from {AggregateCount} aggregate roots", 
            allEvents.Count, aggregateRoots.Count);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Publishes all collected domain events through MediatR
    /// </summary>
    private async Task PublishDomainEventsAsync(CancellationToken cancellationToken = default)
    {
        if (!_domainEvents.Any())
        {
            return;
        }

        var eventsToPublish = _domainEvents.ToList();
        ClearDomainEvents();

        _logger.LogDebug("Publishing {EventCount} domain events", eventsToPublish.Count);

        foreach (var domainEvent in eventsToPublish)
        {
            try
            {
                await _mediator.Publish(domainEvent, cancellationToken:cancellationToken);
                _logger.LogDebug("Published domain event: {EventType}", domainEvent.GetType().Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish domain event: {EventType}", domainEvent.GetType().Name);
                throw;
            }
        }

        _logger.LogInformation("Successfully published {EventCount} domain events", eventsToPublish.Count);
    }

    /// <summary>
    /// Clears domain events from all aggregate roots in the context
    /// </summary>
    private void ClearDomainEventsFromEntities()
    {
        var aggregateRoots = _context.ChangeTracker.Entries<IAggregateRoot>()
            .Where(entry => entry.Entity.DomainEvents.Any())
            .Select(entry => entry.Entity)
            .ToList();

        foreach (var aggregateRoot in aggregateRoots)
        {
            aggregateRoot.ClearDomainEvents();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _currentTransaction?.Dispose();
            ClearDomainEvents();
            _disposed = true;
        }
    }
}