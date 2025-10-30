using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Nezam.Refahi.Recreation.Domain.Services;
using Nezam.Refahi.Shared.Domain;

namespace Nezam.Refahi.Recreation.Domain;

public class NezamRefahiRecreationDomainModule : BonModule
{
  public NezamRefahiRecreationDomainModule()
  {
    DependOn<NezamRefahiSharedDomainModule>();
  }
  public override Task OnConfigureAsync(BonConfigurationContext context)
  {
    context.Services.AddScoped<SpecialCapacityService>();
    return base.OnConfigureAsync(context);
  }
}