using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Identity.Infrastructure.Persistence;

namespace Nezam.Refahi.Identity.Infrastructure.Persistence;

public class IdentityUnitOfWork : IIdentityUnitOfWork
{
    private readonly IdentityDbContext _context;
    private IDbContextTransaction? _currentTransaction;
    private bool _isDisposed = false;

    public IdentityUnitOfWork(IdentityDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task BeginAsync(CancellationToken cancellationToken = default)
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(IdentityUnitOfWork));
            
        if (_currentTransaction != null)
            return;

        _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(IdentityUnitOfWork));
            
        var changes = await _context.SaveChangesAsync(cancellationToken);
        Console.WriteLine($"IdentityUnitOfWork.SaveAsync: {changes} entities saved to database");
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(IdentityUnitOfWork));
            
        if (_currentTransaction == null)
            throw new InvalidOperationException("No transaction started.");

        Console.WriteLine("IdentityUnitOfWork.CommitAsync: Committing transaction...");
        await _currentTransaction.CommitAsync(cancellationToken);
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
        Console.WriteLine("IdentityUnitOfWork.CommitAsync: Transaction committed successfully");
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_isDisposed)
            return;
            
        if (_currentTransaction == null)
            return;

        await _currentTransaction.RollbackAsync(cancellationToken);
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed)
            return;
            
        if (_currentTransaction != null)
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }

        await _context.DisposeAsync();
        _isDisposed = true;
    }
}
