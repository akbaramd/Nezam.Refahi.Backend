using System.Reflection;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Nezam.Refahi.Identity.Application.Pool;
using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Identity.Contracts;
using Nezam.Refahi.Identity.Contracts.Pool;
using Nezam.Refahi.Identity.Domain;
using Nezam.Refahi.Shared.Application;

namespace Nezam.Refahi.Identity.Application;

public class NezamRefahiIdentityApplicationModule : BonModule
{
  public NezamRefahiIdentityApplicationModule()
  {
    DependOn<NezamRefahiSharedApplicationModule>();
    DependOn<NezamRefahiIdentityContractsModule>();
    DependOn<NezamRefahiIdentityDomainModule>();
  }
  public override Task OnConfigureAsync(BonConfigurationContext context)
  {
    // This module will be configured by the shared infrastructure
    // which has access to all the required packages

    context.Services.AddMediatR(v =>
    {
      v.RegisterServicesFromAssembly(typeof(NezamRefahiIdentityApplicationModule).Assembly);
    });

    // Register FluentValidation
    context.Services.AddValidatorsFromAssembly(typeof(NezamRefahiIdentityApplicationModule).Assembly);

    // Register Anti-Corruption Layer for Membership context access
    // Implementation can be easily swapped (HTTP client, message broker, etc.) without affecting Identity domain
    context.Services.AddScoped<IUserIntegrationPool, UserIntegrationPoolService>();

    return base.OnConfigureAsync(context);
  }
}
