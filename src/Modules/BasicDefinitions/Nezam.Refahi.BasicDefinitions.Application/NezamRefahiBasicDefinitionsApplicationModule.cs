using System.Reflection;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nezam.Refahi.BasicDefinitions.Application.HostedServices;
using Nezam.Refahi.BasicDefinitions.Application.Services;
using Nezam.Refahi.BasicDefinitions.Contracts;
using Nezam.Refahi.BasicDefinitions.Contracts.Services;
using Nezam.Refahi.BasicDefinitions.Domain;
using Nezam.Refahi.Shared.Application;

namespace Nezam.Refahi.BasicDefinitions.Application;

public class NezamRefahiBasicDefinitionsApplicationModule : BonModule
{
  public NezamRefahiBasicDefinitionsApplicationModule()
  {
    DependOn<NezamRefahiSharedApplicationModule>();
    DependOn<NezamRefahiBasicDefinitionsContractsModule>();
    DependOn<NezamRefahiBasicDefinitionsDomainModule>();
  }
  
  public override Task OnConfigureAsync(BonConfigurationContext context)
  {
    var assembly = typeof(NezamRefahiBasicDefinitionsApplicationModule).Assembly;
    
    context.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
    context.Services.AddValidatorsFromAssembly(assembly);

    // Register RepresentativeOffice service for inter-context communication
    context.Services.AddScoped<IRepresentativeOfficeService, RepresentativeOfficeService>();

    // Register cache service
    context.Services.AddMemoryCache();
    context.Services.AddScoped<IBasicDefinitionsCacheService, BasicDefinitionsCacheService>();

    // Register background service for cache refresh
    context.Services.AddHostedService<BasicDefinitionsCacheRefreshService>();

    // Register background service for seeding
    context.Services.AddHostedService<BasicDefinitionsSeedingHostedService>();

    return base.OnConfigureAsync(context);
  }
}
