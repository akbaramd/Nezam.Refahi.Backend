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
using Hangfire;
using Hangfire.SqlServer;
using Hangfire.Dashboard;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
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

    // Servers: optimized queues with clear categorization
    context.Services.AddHangfireServer(options =>
    {
      options.WorkerCount = Math.Max(2, Environment.ProcessorCount * 5);
      options.Queues = new[] { 
        "critical",      // High priority jobs
        "default",       // Standard jobs  
        "maintenance"    // Cleanup and maintenance jobs
      };
    });
    
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
    // Schedule maintenance jobs with proper queue assignment
    // Clean up processed messages every hour (retain for 3 days) - FASTER CLEANUP
    // Hangfire maintenance: purge failed jobs older than 7 days - FASTER CLEANUP
    RecurringJob.AddOrUpdate<HangfireMaintenanceService>(
      recurringJobId: "hangfire-maintenance-purge-failed",
      methodCall: service => service.PurgeFailedJobsOlderThanAsync(TimeSpan.FromDays(7)),
      cronExpression: "0 */6 * * *", // Every 6 hours
      queue: "maintenance");

    // Additional maintenance job for old successful jobs cleanup - FASTER CLEANUP
    RecurringJob.AddOrUpdate<HangfireMaintenanceService>(
      recurringJobId: "hangfire-maintenance-purge-successful",
      methodCall: service => service.PurgeOldSuccessfulJobsAsync(TimeSpan.FromDays(3)),
      cronExpression: "0 */12 * * *", // Every 12 hours
      queue: "maintenance");

    // Schedule user-member reconciliation job - runs every 12 hours - FASTER PROCESSING
    RecurringJob.AddOrUpdate<IUserMemberReconciliationService>(
      recurringJobId: "user-member-reconciliation",
      methodCall: service => service.ReconcileOrphanedUsersAsync(CancellationToken.None),
      cronExpression: "0 */12 * * *", // Every 12 hours
      queue: "maintenance");

    // Schedule idempotency cleanup job - runs every 6 hours - FASTER CLEANUP
    RecurringJob.AddOrUpdate<IIdempotencyService>(
      recurringJobId: "idempotency-cleanup",
      methodCall: service => service.CleanupOldRecordsAsync(7, CancellationToken.None),
      cronExpression: "0 */6 * * *", // Every 6 hours
      queue: "maintenance");
    
    return base.OnPostApplicationAsync(context);
  }

}
