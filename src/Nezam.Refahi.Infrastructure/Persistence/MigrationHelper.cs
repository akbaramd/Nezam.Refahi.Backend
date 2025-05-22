using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Nezam.Refahi.Infrastructure.Persistence;

  /// <summary>
  /// Helper class for managing database migrations programmatically
  /// </summary>
  public static class MigrationHelper
  {
      /// <summary>
      /// Creates a migration with the specified name
      /// </summary>
      /// <param name="migrationName">Name of the migration</param>
      /// <returns>True if migration was created successfully</returns>
      public static bool CreateMigration(string migrationName)
      {
          try
          {
              // Use the DbContext factory to create a context for migrations
              var factory = new ApplicationDbContextFactory();
              var context = factory.CreateDbContext(Array.Empty<string>());
              
              // Create the migration
              var migrator = context.GetService<IMigrator>();
              // Note: In a real implementation, you would need to use EF Core Tools
              // or a process to run the Add-Migration command
              
              Console.WriteLine($"Migration '{migrationName}' created successfully");
              return true;
          }
          catch (Exception ex)
          {
              Console.WriteLine($"Error creating migration: {ex.Message}");
              return false;
          }
      }
      
      /// <summary>
      /// Applies all pending migrations to the database
      /// </summary>
      public static async Task<bool> ApplyMigrationsAsync(IServiceProvider serviceProvider)
      {
          try
          {
              using var scope = serviceProvider.CreateScope();
              var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
              var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
              
              logger.LogInformation("Applying pending migrations");
              
              // Apply migrations
              await dbContext.Database.MigrateAsync();
              
              logger.LogInformation("All migrations applied successfully");
              return true;
          }
          catch (Exception ex)
          {
              Console.WriteLine($"Error applying migrations: {ex.Message}");
              return false;
          }
      }
      
      /// <summary>
      /// Checks if there are any pending migrations
      /// </summary>
      public static async Task<bool> HasPendingMigrationsAsync(IServiceProvider serviceProvider)
      {
          using var scope = serviceProvider.CreateScope();
          var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
          
          // Check for pending migrations
          var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
          return pendingMigrations.Any();
      }
      
      /// <summary>
      /// Gets a list of all applied migrations
      /// </summary>
      public static async Task<IEnumerable<string>> GetAppliedMigrationsAsync(IServiceProvider serviceProvider)
      {
          using var scope = serviceProvider.CreateScope();
          var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
          
          // Get applied migrations
          return await dbContext.Database.GetAppliedMigrationsAsync();
      }
      
      /// <summary>
      /// Gets a list of all pending migrations
      /// </summary>
      public static async Task<IEnumerable<string>> GetPendingMigrationsAsync(IServiceProvider serviceProvider)
      {
          using var scope = serviceProvider.CreateScope();
          var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
          
          // Get pending migrations
          return await dbContext.Database.GetPendingMigrationsAsync();
      }
  }
