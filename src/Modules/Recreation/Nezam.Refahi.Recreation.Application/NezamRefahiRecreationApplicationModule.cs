using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Nezam.Refahi.Identity.Contracts.Pool;
using Nezam.Refahi.Recreation.Application.Services;
using Nezam.Refahi.Recreation.Contracts;
using Nezam.Refahi.Shared.Application;

namespace Nezam.Refahi.Recreation.Application;

public class NezamRefahiRecreationApplicationModule : BonModule
{
  public NezamRefahiRecreationApplicationModule()
  {
    DependOn<NezamRefahiSharedApplicationModule>();
    DependOn<NezamRefahiRecreationContractsModule>();
  }
  
  
  public override Task OnConfigureAsync(BonConfigurationContext context)
  {
    // This module will be configured by the shared infrastructure
    // which has access to all the required packages

    context.Services.AddMediatR(v =>
    {
      v.RegisterServicesFromAssembly(typeof(NezamRefahiRecreationApplicationModule).Assembly);
    });

    // Register FluentValidation
    context.Services.AddValidatorsFromAssembly(typeof(NezamRefahiRecreationApplicationModule).Assembly);

    // Register application services
    context.Services.AddScoped<ParticipantValidationService>();

    return base.OnConfigureAsync(context);
  }
}