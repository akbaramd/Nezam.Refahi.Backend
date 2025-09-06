using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nezam.Refahi.Settings.Application;
using Nezam.Refahi.Settings.Application.Services;
using Nezam.Refahi.Settings.Domain.Repositories;
using Nezam.Refahi.Settings.Infrastructure.Persistence;
using Nezam.Refahi.Settings.Infrastructure.Persistence.Repositories;
using Nezam.Refahi.Settings.Infrastructure.Persistence.Seeding;
using Nezam.Refahi.Shared.Infrastructure;

namespace Nezam.Refahi.Settings.Infrastructure;

public class NezamRefahiSettingsInfrastructureModule : BonModule
{
  public NezamRefahiSettingsInfrastructureModule()
  {
    DependOn<NezamRefahiSharedInfrastructureModule<SettingsDbContext>>();
    DependOn<NezamRefahiSettingsApplicationModule>();
  }

  public override Task OnConfigureAsync(BonConfigurationContext context)
  {
    context.Services.AddDbContext<SettingsDbContext>(options =>
    {
      var configuration = context.GetRequireService<IConfiguration>();
      var connectionString = configuration.GetConnectionString("DefaultConnection");
            
      if (string.IsNullOrEmpty(connectionString))
      {
        throw new InvalidOperationException("Database connection string 'DefaultConnection' is not configured.");
      }
      options.UseSqlServer(connectionString, sqlOptions =>
      {
        sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory__Settings","settings");
      });
    });

    context.Services.AddScoped<ISettingsRepository, SettingsRepository>();
    context.Services.AddScoped<ISettingChangeEventRepository, SettingChangeEventRepository>();

    // Register Unit of Work
    context.Services.AddScoped<ISettingsUnitOfWork, SettingsUnitOfWork>();

    // Register settings seeding service
    context.Services.AddHostedService<SettingsSeedingService>();

    return base.OnConfigureAsync(context);
  }
}
