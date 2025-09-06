using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Nezam.Refahi.Settings.Application.Services;
using Nezam.Refahi.Settings.Contracts;
using Nezam.Refahi.Settings.Domain;
using Nezam.Refahi.Shared.Application;

namespace Nezam.Refahi.Settings.Application;

public class NezamRefahiSettingsApplicationModule : BonModule
{
    public override async Task OnConfigureAsync(BonConfigurationContext context)
    {
        var services = context.Services;

        // Register application services
        services.AddScoped<ISettingsService, SettingService>();
        
        
        context.Services.AddMediatR(v =>
        {
          v.RegisterServicesFromAssembly(typeof(NezamRefahiSettingsApplicationModule).Assembly);
        });

        // Register FluentValidation
        context.Services.AddValidatorsFromAssembly(typeof(NezamRefahiSettingsApplicationModule).Assembly);
        await base.OnConfigureAsync(context);
    }

    public NezamRefahiSettingsApplicationModule()
    {
        DependOn<NezamRefahiSettingsContractsModule>();
        DependOn<NezamRefahiSharedApplicationModule>();
        DependOn<NezamRefahiSettingsDomainModule>();
    }
}