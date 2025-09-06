using System.Reflection;
using Microsoft.EntityFrameworkCore;

using Nezam.Refahi.Settings.Domain.Entities;

namespace Nezam.Refahi.Settings.Infrastructure.Persistence;

/// <summary>
/// Main database context for the application
/// All entity configurations are automatically applied from the Configurations folder
/// </summary>
public class SettingsDbContext(DbContextOptions<SettingsDbContext> options) : DbContext(options)
{


    // Settings bounded context
    public DbSet<SettingsSection> SettingsSections { get; set; } = default!;
    public DbSet<SettingsCategory> SettingsCategories { get; set; } = default!;
    public DbSet<Setting> Settings { get; set; } = default!;
    public DbSet<SettingChangeEvent> SettingChangeEvents { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from this assembly
        // This automatically discovers and applies all IEntityTypeConfiguration<T> implementations
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
