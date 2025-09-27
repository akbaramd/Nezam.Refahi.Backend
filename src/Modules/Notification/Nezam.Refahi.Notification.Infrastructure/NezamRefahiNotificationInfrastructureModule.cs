
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nezam.Refahi.Notifications.Application;
using Nezam.Refahi.Notifications.Application.Services;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Finance.Domain.Services;
using Nezam.Refahi.Notifications.Domain.Repositories;
using Nezam.Refahi.Notifications.Domain.Services;
using Nezam.Refahi.Notifications.Infrastructure.BackgroundServices;
using Nezam.Refahi.Notifications.Infrastructure.Persistence;
using Nezam.Refahi.Notifications.Infrastructure.Persistence.Repositories;
using Nezam.Refahi.Shared.Infrastructure;
namespace Nezam.Refahi.Notifications.Infrastructure;

/// <summary>
/// Notification Infrastructure Module
/// </summary>
public class NezamRefahiNotificationInfrastructureModule : BonModule
{
    public NezamRefahiNotificationInfrastructureModule()
    {
        DependOn<NezamRefahiNotificationApplicationModule>();
        DependOn<NezamRefahiSharedInfrastructureModule>();
    }
    
    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
       
        // Register DbContext
        context.Services.AddDbContext<NotificationDbContext>(options =>
        {
          var configuration = context.GetRequireService<IConfiguration>();
          var connectionString = configuration.GetConnectionString("DefaultConnection");

            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(NotificationDbContext).Assembly.FullName);
                sqlOptions.MigrationsHistoryTable("__NotificationMigrationsHistory", "notification");
            });
        });
        
        // Register repositories
        context.Services.AddScoped<INotificationRepository, NotificationRepository>();
        
        // Register domain services
        context.Services.AddScoped<INotificationDomainService, NotificationDomainService>();
        
        // Register Unit of Work
        context.Services.AddScoped<INotificationUnitOfWork, NotificationUnitOfWork>();
        
        // Register background services
        // NotificationCleanupService moved to Hangfire jobs - runs at 4:00 AM daily
        
        return base.OnConfigureAsync(context);
    }
}
