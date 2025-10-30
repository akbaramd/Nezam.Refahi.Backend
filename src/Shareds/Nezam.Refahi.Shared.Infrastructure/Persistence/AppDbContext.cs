using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Shared.Domain.Entities;
using Nezam.Refahi.Shared.Domain.Services;

namespace Nezam.Refahi.Shared.Infrastructure.Persistence;

/// <summary>
/// Shared application database context for cross-cutting concerns
/// Handles shared infrastructure entities
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Idempotency tracking
    public DbSet<EventIdempotency> EventIdempotencies { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Set default schema for shared infrastructure
        modelBuilder.HasDefaultSchema("shared");
    }
}
