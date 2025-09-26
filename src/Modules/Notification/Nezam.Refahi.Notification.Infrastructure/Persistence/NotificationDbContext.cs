using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Notifications.Domain.Entities;

namespace Nezam.Refahi.Notifications.Infrastructure.Persistence;

/// <summary>
/// Database context for Notification module
/// </summary>
public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
    {
    }
    
    // DbSets
    public DbSet<Notification> Notifications { get; set; } = default!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply all entity configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
