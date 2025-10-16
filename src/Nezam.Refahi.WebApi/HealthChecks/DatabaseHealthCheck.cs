using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nezam.Refahi.BasicDefinitions.Infrastructure.Persistence;
using Nezam.Refahi.Finance.Infrastructure.Persistence;
using Nezam.Refahi.Identity.Infrastructure.Persistence;
using Nezam.Refahi.Membership.Infrastructure.Persistence;
using Nezam.Refahi.Notifications.Infrastructure.Persistence;
using Nezam.Refahi.Recreation.Infrastructure.Persistence;
using Nezam.Refahi.Settings.Infrastructure.Persistence;
using Nezam.Refahi.Shared.Infrastructure.Persistence;

namespace Nezam.Refahi.WebApi.HealthChecks;

/// <summary>
/// Health check for database connectivity
/// </summary>
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseHealthCheck> _logger;

    public DatabaseHealthCheck(IServiceProvider serviceProvider, ILogger<DatabaseHealthCheck> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            
            // Check shared database (AppDbContext)
            var appDbContext = scope.ServiceProvider.GetService<AppDbContext>();
            if (appDbContext != null)
            {
                await appDbContext.Database.CanConnectAsync(cancellationToken);
            }

            // Check BasicDefinitions database
            var basicDefinitionsDbContext = scope.ServiceProvider.GetService<BasicDefinitionsDbContext>();
            if (basicDefinitionsDbContext != null)
            {
                await basicDefinitionsDbContext.Database.CanConnectAsync(cancellationToken);
            }

            // Check other module databases
            var identityDbContext = scope.ServiceProvider.GetService<IdentityDbContext>();
            if (identityDbContext != null)
            {
                await identityDbContext.Database.CanConnectAsync(cancellationToken);
            }

            var membershipDbContext = scope.ServiceProvider.GetService<MembershipDbContext>();
            if (membershipDbContext != null)
            {
                await membershipDbContext.Database.CanConnectAsync(cancellationToken);
            }

            var financeDbContext = scope.ServiceProvider.GetService<FinanceDbContext>();
            if (financeDbContext != null)
            {
                await financeDbContext.Database.CanConnectAsync(cancellationToken);
            }

            var settingsDbContext = scope.ServiceProvider.GetService<SettingsDbContext>();
            if (settingsDbContext != null)
            {
                await settingsDbContext.Database.CanConnectAsync(cancellationToken);
            }

            var recreationDbContext = scope.ServiceProvider.GetService<RecreationDbContext>();
            if (recreationDbContext != null)
            {
                await recreationDbContext.Database.CanConnectAsync(cancellationToken);
            }

            var notificationDbContext = scope.ServiceProvider.GetService<NotificationDbContext>();
            if (notificationDbContext != null)
            {
                await notificationDbContext.Database.CanConnectAsync(cancellationToken);
            }

            return HealthCheckResult.Healthy("All database connections are healthy");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database health check failed");
            return HealthCheckResult.Unhealthy("Database connection failed", ex);
        }
    }
}
