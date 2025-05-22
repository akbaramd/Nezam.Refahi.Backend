using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Nezam.Refahi.Infrastructure.Persistence;

  /// <summary>
  /// Handles running migrations for the SQLite database
  /// </summary>
  public class MigrationRunner
  {
      private readonly IServiceProvider _serviceProvider;
      private readonly ILogger<MigrationRunner> _logger;

      public MigrationRunner(
          IServiceProvider serviceProvider,
          ILogger<MigrationRunner> logger)
      {
          _serviceProvider = serviceProvider;
          _logger = logger;
      }

      /// <summary>
      /// Runs all pending migrations
      /// </summary>
      public async Task RunMigrationsAsync()
      {
          try
          {
              _logger.LogInformation("Starting migration process");
              
              using var scope = _serviceProvider.CreateScope();
              var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
              
              // Get pending migrations
              var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
              var pendingCount = pendingMigrations.Count();
              
              if (pendingCount > 0)
              {
                  _logger.LogInformation($"Found {pendingCount} pending migrations");
                  
                  // Apply migrations
                  await dbContext.Database.MigrateAsync();
                  
                  _logger.LogInformation("Migrations applied successfully");
              }
              else
              {
                  _logger.LogInformation("No pending migrations found");
              }
          }
          catch (Exception ex)
          {
              _logger.LogError(ex, "An error occurred while running migrations");
              throw;
          }
      }
      
      /// <summary>
      /// Creates a new SQLite database if it doesn't exist
      /// </summary>
      public async Task EnsureDatabaseCreatedAsync()
      {
          try
          {
              _logger.LogInformation("Ensuring database exists");
              
              using var scope = _serviceProvider.CreateScope();
              var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
              
              // Create database if it doesn't exist
              await dbContext.Database.EnsureCreatedAsync();
              
              _logger.LogInformation("Database exists or was created successfully");
          }
          catch (Exception ex)
          {
              _logger.LogError(ex, "An error occurred while creating the database");
              throw;
          }
      }
  }
