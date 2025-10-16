using System.Reflection;
using Bonyan.Modularity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Nezam.Refahi.Shared.Application;
using System.Text;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Nezam.Refahi.Shared.Application.Common.Behaviors;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Infrastructure.Services;
using Nezam.Refahi.Shared.Infrastructure.Persistence;
using Nezam.Refahi.Shared.Infrastructure.Persistence.Repositories;
using Nezam.Refahi.Shared.Infrastructure.Outbox;
using Hangfire;
using Hangfire.SqlServer;
using Hangfire.Dashboard;
using Microsoft.Extensions.DependencyInjection;
using Nezam.Refahi.Shared.Domain.Repositories;
using Nezam.Refahi.Shared.Domain.Services;

namespace Nezam.Refahi.Shared.Infrastructure;

/// <summary>
/// Hangfire job activator that uses DI container
/// </summary>
public class ContainerJobActivator : JobActivator
{
    private IServiceScopeFactory _container;
   
    public ContainerJobActivator(IServiceScopeFactory container)
    {
        _container = container;
    }

    public override object ActivateJob(Type type)
    {
        // Create scope but don't dispose it here - let Hangfire handle it
        var scope = _container.CreateScope();
        return scope.ServiceProvider.GetRequiredService(type);
    }

    public override JobActivatorScope BeginScope(JobActivatorContext context)
    {
        return new ContainerJobActivatorScope(_container.CreateScope());
    }
}

/// <summary>
/// Scope for Hangfire job activator
/// </summary>
public class ContainerJobActivatorScope : JobActivatorScope
{
    private readonly IServiceScope _scope;

    public ContainerJobActivatorScope(IServiceScope scope)
    {
        _scope = scope;
    }

    public override object Resolve(Type type)
    {
        return _scope.ServiceProvider.GetRequiredService(type);
    }

    public override void DisposeScope()
    {
        _scope?.Dispose();
    }
}

/// <summary>
/// Static accessor for service provider to use in Hangfire jobs
/// </summary>

public class NezamRefahiSharedInfrastructureModule : BonWebModule
{
  public NezamRefahiSharedInfrastructureModule()
  {
    DependOn<NezamRefahiSharedApplicationModule>();
  }

  public override Task OnConfigureAsync(BonConfigurationContext context)
  {
    // Configure Shared DbContext
    context.Services.AddDbContext<AppDbContext>(options =>
    {
      var configuration = context.GetRequireService<IConfiguration>();
      var connectionString = configuration.GetConnectionString("DefaultConnection");

      if (string.IsNullOrEmpty(connectionString))
      {
        throw new InvalidOperationException("Database connection string 'DefaultConnection' is not configured.");
      }

      options.EnableSensitiveDataLogging();
      options.UseSqlServer(connectionString, sqlOptions =>
      {
        sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory__Shared", "shared");
      });
    });
    
    // Configure Hangfire
    var configuration = context.GetRequireService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection") 
      ?? throw new InvalidOperationException("DefaultConnection connection string is not configured");

    // Add Hangfire services with production-grade configuration
    context.Services.AddHangfire(config =>
    {
      config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
        {
          SchemaName = "shared",
          // Recommended for 1.7+ / 1.8+ (schema 7+) to improve throughput
          CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
          SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
          QueuePollInterval = TimeSpan.Zero,         // event-driven fetch
          UseRecommendedIsolationLevel = true,
          DisableGlobalLocks = true,                 // requires schema 7
          JobExpirationCheckInterval = TimeSpan.FromMinutes(5),
          PrepareSchemaIfNecessary = true
        })
        // Set global retention for final states (Succeeded/Deleted)
        .WithJobExpirationTimeout(TimeSpan.FromDays(7)); // adjust to your SLA
    });

    // Global filters: bounded retries with sane backoff; delete after final failure
    GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute
    {
        Attempts = 3,
        OnAttemptsExceeded = AttemptsExceededAction.Delete
    });

    // Register the job activator with proper scope factory
    context.Services.AddSingleton<JobActivator>(provider => 
        new ContainerJobActivator(provider.GetRequiredService<IServiceScopeFactory>()));

    // Servers: dedicated queues + bounded concurrency per process
    context.Services.AddHangfireServer(options =>
    {
      options.WorkerCount = Math.Max(2, Environment.ProcessorCount * 5);
      options.Queues = new[] { "critical", "default", "slow", "outbox", "wallet-snapshots" };
    });

    // Register Outbox repositories and services
    context.Services.AddScoped<IOutboxRepository, OutboxRepository>();
    context.Services.AddScoped<IOutboxPublisher, OutboxPublisher>();
    context.Services.AddScoped<OutboxProcessor>();
    context.Services.AddScoped<OutboxCleanupService>();
    context.Services.AddScoped<IDlqManagementService, DlqManagementService>();
    
    // Register reconciliation and idempotency services
    context.Services.AddScoped<IUserMemberReconciliationService, UserMemberReconciliationService>();
    context.Services.AddScoped<IIdempotencyService, IdempotencyService>();
    
    // Register Hangfire maintenance service
    context.Services.AddScoped<HangfireMaintenanceService>();
    
    context.Services.AddScoped<ICurrentUserService, CurrentUserService>();
    
    // Add memory cache for shared services
    context.Services.AddMemoryCache();
    
    // Add HTTP context accessor for shared services
    context.Services.AddHttpContextAccessor();
    
    context.Services.AddMediatR(cfg => 
    {
      cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            
      // Add pipeline behaviors
      cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    });
        
    // Add FluentValidation
    context.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    

    // Configure JWT authentication (shared across modules)

    return base.OnConfigureAsync(context);
  }

  public override Task OnPostApplicationAsync(BonWebApplicationContext context)
  {
    var app = context.Application;

    // Configure global Hangfire job activator
    GlobalConfiguration.Configuration.UseActivator(
      new ContainerJobActivator(app.Services.GetRequiredService<IServiceScopeFactory>()));

    // Configure Hangfire Dashboard
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
      Authorization = new[] { new HangfireAuthorizationFilter() },
      DashboardTitle = "Nezam Refahi Background Jobs",
      DisplayStorageConnectionString = false
    });
    // Delete any existing outbox processor jobs
    BackgroundJob.Delete("outbox-processor");
    
    // Set up continuous outbox processing using a single background job
    // This job will run continuously with a while loop
    BackgroundJob.Enqueue<OutboxProcessor>(
      processor => processor.ProcessOutboxMessagesContinuouslyAsync());

    // Schedule outbox cleanup jobs
    // Clean up processed messages every 6 hours (retain for 7 days)
    RecurringJob.AddOrUpdate<OutboxCleanupService>(
      "outbox-cleanup-processed",
      service => service.CleanupProcessedMessagesAsync(7, 1000, CancellationToken.None),
      "0 */6 * * *", // Every 6 hours
      new RecurringJobOptions
      {
        TimeZone = TimeZoneInfo.Utc
      });

    // Clean up failed messages every 24 hours (retain for 30 days)
    RecurringJob.AddOrUpdate<OutboxCleanupService>(
      "outbox-cleanup-failed",
      service => service.CleanupFailedMessagesAsync(30, 1000, CancellationToken.None),
      "0 2 * * *", // Daily at 2 AM UTC
      new RecurringJobOptions
      {
        TimeZone = TimeZoneInfo.Utc
      });

    // Hangfire maintenance: purge failed jobs older than 30 days
    RecurringJob.AddOrUpdate<HangfireMaintenanceService>(
      "hangfire-maintenance:purge-failed-30d",
      service => service.PurgeFailedJobsOlderThanAsync(TimeSpan.FromDays(30)),
      "0 3 * * *", // Daily at 3 AM UTC
      new RecurringJobOptions
      {
        TimeZone = TimeZoneInfo.Utc
      });

    // Additional maintenance job for old successful jobs cleanup
    RecurringJob.AddOrUpdate<HangfireMaintenanceService>(
      "hangfire-maintenance:purge-old-successful",
      service => service.PurgeOldSuccessfulJobsAsync(TimeSpan.FromDays(14)),
      "0 4 * * *", // Daily at 4 AM UTC
      new RecurringJobOptions
      {
        TimeZone = TimeZoneInfo.Utc
      });

    // Full cleanup job runs weekly on Sundays at 3 AM UTC
    RecurringJob.AddOrUpdate<OutboxCleanupService>(
      "outbox-cleanup-full",
      service => service.PerformFullCleanupAsync(7, 30, 1000, CancellationToken.None),
      "0 3 * * 0", // Weekly on Sunday at 3 AM UTC
      new RecurringJobOptions
      {
        TimeZone = TimeZoneInfo.Utc
      });

    // Schedule DLQ processing job - runs every 30 minutes
    RecurringJob.AddOrUpdate<IDlqManagementService>(
      "dlq-processing",
      service => service.ProcessDlqMessagesAsync(CancellationToken.None),
      "*/30 * * * *", // Every 30 minutes
      new RecurringJobOptions
      {
        TimeZone = TimeZoneInfo.Utc
      });

    // Schedule DLQ cleanup job - runs daily at 4 AM UTC
    RecurringJob.AddOrUpdate<IDlqManagementService>(
      "dlq-cleanup",
      service => service.CleanupOldDlqMessagesAsync(30, CancellationToken.None),
      "0 4 * * *", // Daily at 4 AM UTC
      new RecurringJobOptions
      {
        TimeZone = TimeZoneInfo.Utc
      });

    // Schedule user-member reconciliation job - runs daily at 1 AM UTC
    RecurringJob.AddOrUpdate<IUserMemberReconciliationService>(
      "user-member-reconciliation",
      service => service.ReconcileOrphanedUsersAsync(CancellationToken.None),
      "0 1 * * *", // Daily at 1 AM UTC
      new RecurringJobOptions
      {
        TimeZone = TimeZoneInfo.Utc
      });

    // Schedule idempotency cleanup job - runs weekly on Mondays at 5 AM UTC
    RecurringJob.AddOrUpdate<IdempotencyService>(
      "idempotency-cleanup",
      service => service.CleanupOldRecordsAsync(30, CancellationToken.None),
      "0 5 * * 1", // Weekly on Monday at 5 AM UTC
      new RecurringJobOptions
      {
        TimeZone = TimeZoneInfo.Utc
      });
    
    return base.OnPostApplicationAsync(context);
  }

}
