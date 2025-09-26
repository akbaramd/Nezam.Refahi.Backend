using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Notifications.Domain.Entities;
using Nezam.Refahi.Notifications.Domain.Repositories;
using Nezam.Refahi.Notifications.Infrastructure.Persistence;

namespace Nezam.Refahi.Notifications.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Notification entity
/// </summary>
public class NotificationRepository : EfRepository<NotificationDbContext, Notification, Guid>, INotificationRepository
{
    public NotificationRepository(NotificationDbContext context) : base(context)
    {
    }

    public async Task<List<Notification>> GetUserNotificationsAsync(
        Guid externalUserId, 
        int limit = 50, 
        int offset = 0,
        CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(n => n.ExternalUserId == externalUserId)
            .Where(n => n.ExpiresAt == null || n.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(n => n.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<List<Notification>> GetUserNotificationsAsync(
        Guid externalUserId, 
        int limit = 50, 
        int offset = 0,
        string? context = null,
        string? action = null,
        bool? isRead = null,
        CancellationToken cancellationToken = default)
    {
        var query = PrepareQuery(_dbSet)
            .Where(n => n.ExternalUserId == externalUserId)
            .Where(n => n.ExpiresAt == null || n.ExpiresAt > DateTime.UtcNow);
        
        // Apply context filter
        if (!string.IsNullOrWhiteSpace(context))
        {
            query = query.Where(n => n.Context == context);
        }
        
        // Apply action filter
        if (!string.IsNullOrWhiteSpace(action))
        {
            query = query.Where(n => n.Action == action);
        }
        
        // Apply read status filter
        if (isRead.HasValue)
        {
            query = query.Where(n => n.IsRead == isRead.Value);
        }
        
        return await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<List<Notification>> GetNotificationsByContextAsync(
        string context,
        int limit = 50,
        int offset = 0,
        CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(n => n.Context == context)
            .Where(n => n.ExpiresAt == null || n.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(n => n.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<List<Notification>> GetNotificationsByActionAsync(
        string action,
        int limit = 50,
        int offset = 0,
        CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(n => n.Action == action)
            .Where(n => n.ExpiresAt == null || n.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(n => n.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<List<Notification>> GetNotificationsByContextAndActionAsync(
        string context,
        string action,
        int limit = 50,
        int offset = 0,
        CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(n => n.Context == context && n.Action == action)
            .Where(n => n.ExpiresAt == null || n.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(n => n.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<int> GetUnreadCountAsync(Guid externalUserId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(n => n.ExternalUserId == externalUserId && !n.IsRead)
            .Where(n => n.ExpiresAt == null || n.ExpiresAt > DateTime.UtcNow)
            .CountAsync(cancellationToken);
    }
    
    public async Task<int> GetTotalCountAsync(Guid externalUserId, string? context = null, string? action = null, bool? isRead = null, CancellationToken cancellationToken = default)
    {
        var query = PrepareQuery(_dbSet)
            .Where(n => n.ExternalUserId == externalUserId)
            .Where(n => n.ExpiresAt == null || n.ExpiresAt > DateTime.UtcNow);
        
        // Apply context filter
        if (!string.IsNullOrWhiteSpace(context))
        {
            query = query.Where(n => n.Context == context);
        }
        
        // Apply action filter
        if (!string.IsNullOrWhiteSpace(action))
        {
            query = query.Where(n => n.Action == action);
        }
        
        // Apply read status filter
        if (isRead.HasValue)
        {
            query = query.Where(n => n.IsRead == isRead.Value);
        }
        
        return await query.CountAsync(cancellationToken);
    }
    
    public async Task<int> GetUnreadCountByContextAsync(
        Guid externalUserId, 
        string context, 
        CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(n => n.ExternalUserId == externalUserId && !n.IsRead && n.Context == context)
            .Where(n => n.ExpiresAt == null || n.ExpiresAt > DateTime.UtcNow)
            .CountAsync(cancellationToken);
    }
    
    public async Task<int> GetUnreadCountByActionAsync(
        Guid externalUserId, 
        string action, 
        CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(n => n.ExternalUserId == externalUserId && !n.IsRead && n.Action == action)
            .Where(n => n.ExpiresAt == null || n.ExpiresAt > DateTime.UtcNow)
            .CountAsync(cancellationToken);
    }
    
    public async Task<int> GetUnreadCountByContextAndActionAsync(
        Guid externalUserId, 
        string context, 
        string action, 
        CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(n => n.ExternalUserId == externalUserId && !n.IsRead && n.Context == context && n.Action == action)
            .Where(n => n.ExpiresAt == null || n.ExpiresAt > DateTime.UtcNow)
            .CountAsync(cancellationToken);
    }
    
    public async Task<Dictionary<string, int>> GetUnreadCountByContextAsync(
        Guid externalUserId, 
        CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(n => n.ExternalUserId == externalUserId && !n.IsRead)
            .Where(n => n.ExpiresAt == null || n.ExpiresAt > DateTime.UtcNow)
            .GroupBy(n => n.Context)
            .Select(g => new { Context = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Context, x => x.Count, cancellationToken);
    }
    
    public async Task<Dictionary<string, int>> GetUnreadCountByActionAsync(
        Guid externalUserId, 
        CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(n => n.ExternalUserId == externalUserId && !n.IsRead)
            .Where(n => n.ExpiresAt == null || n.ExpiresAt > DateTime.UtcNow)
            .GroupBy(n => n.Action)
            .Select(g => new { Action = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Action, x => x.Count, cancellationToken);
    }
    
    public async Task MarkAsReadAsync(Guid notificationId, CancellationToken cancellationToken = default)
    {
        var notification = await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(n => n.Id == notificationId, cancellationToken);
        
        if (notification != null)
        {
            notification.MarkAsRead();
        }
    }
    
    public async Task MarkAllAsReadAsync(Guid externalUserId, CancellationToken cancellationToken = default)
    {
        var notifications = await PrepareQuery(_dbSet)
            .Where(n => n.ExternalUserId == externalUserId && !n.IsRead)
            .ToListAsync(cancellationToken);
        
        foreach (var notification in notifications)
        {
            notification.MarkAsRead();
        }
    }
    
    public async Task MarkAsReadByContextAsync(
        Guid externalUserId, 
        string context, 
        CancellationToken cancellationToken = default)
    {
        var notifications = await PrepareQuery(_dbSet)
            .Where(n => n.ExternalUserId == externalUserId && !n.IsRead && n.Context == context)
            .ToListAsync(cancellationToken);
        
        foreach (var notification in notifications)
        {
            notification.MarkAsRead();
        }
    }
    
    public async Task MarkAsReadByActionAsync(
        Guid externalUserId, 
        string action, 
        CancellationToken cancellationToken = default)
    {
        var notifications = await PrepareQuery(_dbSet)
            .Where(n => n.ExternalUserId == externalUserId && !n.IsRead && n.Action == action)
            .ToListAsync(cancellationToken);
        
        foreach (var notification in notifications)
        {
            notification.MarkAsRead();
        }
    }
    
    public async Task DeleteExpiredAsync(CancellationToken cancellationToken = default)
    {
        var expiredNotifications = await PrepareQuery(_dbSet)
            .Where(n => n.ExpiresAt != null && n.ExpiresAt < DateTime.UtcNow)
            .ToListAsync(cancellationToken);
        
        if (expiredNotifications.Any())
        {
            _dbSet.RemoveRange(expiredNotifications);
        }
    }
    
    public async Task DeleteByContextAsync(
        Guid externalUserId, 
        string context, 
        CancellationToken cancellationToken = default)
    {
        var notifications = await PrepareQuery(_dbSet)
            .Where(n => n.ExternalUserId == externalUserId && n.Context == context)
            .ToListAsync(cancellationToken);
        
        if (notifications.Any())
        {
            _dbSet.RemoveRange(notifications);
        }
    }
    
    public async Task DeleteByActionAsync(
        Guid externalUserId, 
        string action, 
        CancellationToken cancellationToken = default)
    {
        var notifications = await PrepareQuery(_dbSet)
            .Where(n => n.ExternalUserId == externalUserId && n.Action == action)
            .ToListAsync(cancellationToken);
        
        if (notifications.Any())
        {
            _dbSet.RemoveRange(notifications);
        }
    }

    protected override IQueryable<Notification> PrepareQuery(IQueryable<Notification> query)
    {
        return query;
    }
}