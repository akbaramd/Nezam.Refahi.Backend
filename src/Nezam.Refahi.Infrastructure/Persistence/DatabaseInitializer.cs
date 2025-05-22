using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Nezam.Refahi.Infrastructure.Persistence;

  /// <summary>
  /// Handles database initialization, migrations, and seeding.
  /// Following DDD principles, this keeps infrastructure concerns separate from domain logic.
  /// </summary>
  public class DatabaseInitializer
  {
      private readonly IServiceProvider _serviceProvider;
      private readonly ILogger<DatabaseInitializer> _logger;

      public DatabaseInitializer(
          IServiceProvider serviceProvider,
          ILogger<DatabaseInitializer> logger)
      {
          _serviceProvider = serviceProvider;
          _logger = logger;
      }

      /// <summary>
      /// Initializes the database by applying migrations and seeding initial data
      /// </summary>
      public async Task InitializeDatabaseAsync()
      {
          try
          {
              _logger.LogInformation("Starting database initialization");
              
              // Apply migrations
              await ApplyMigrationsAsync();
              
              // Seed initial data if needed
              await SeedDataAsync();
              
              _logger.LogInformation("Database initialization completed successfully");
          }
          catch (Exception ex)
          {
              _logger.LogError(ex, "An error occurred while initializing the database");
              throw;
          }
      }

      /// <summary>
      /// Applies any pending migrations to the database
      /// </summary>
      private async Task ApplyMigrationsAsync()
      {
          _logger.LogInformation("Applying database migrations");
          
          using var scope = _serviceProvider.CreateScope();
          var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
          
          // Apply migrations
          await dbContext.Database.MigrateAsync();
          
          _logger.LogInformation("Database migrations applied successfully");
      }

      /// <summary>
      /// Seeds the database with initial data if needed
      /// </summary>
      private async Task SeedDataAsync()
      {
          _logger.LogInformation("Checking if database seeding is required");
          
          using var scope = _serviceProvider.CreateScope();
          var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
          
          // Check if database is empty and needs seeding
          if (!await dbContext.Users.AnyAsync())
          {
              _logger.LogInformation("Seeding initial data");
              
              // Seed initial data here
              // Example: await SeedUsersAsync(dbContext);
              // Example: await SeedLocationsAsync(dbContext);
              
              await dbContext.SaveChangesAsync();
              
              _logger.LogInformation("Initial data seeding completed");
          }
          else
          {
              _logger.LogInformation("Database already contains data, skipping seeding");
          }
      }
  }
