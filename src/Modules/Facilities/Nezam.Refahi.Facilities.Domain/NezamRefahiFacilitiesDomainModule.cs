using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Nezam.Refahi.Facilities.Domain.Services;
using Nezam.Refahi.Shared.Domain;

namespace Nezam.Refahi.Facilities.Domain;

public class NezamRefahiFacilitiesDomainModule : BonModule
{
  public NezamRefahiFacilitiesDomainModule()
  {
    DependOn<NezamRefahiSharedDomainModule>();
  }

  public override Task OnConfigureAsync(BonConfigurationContext context)
  {
    // Domain module - no services to register
    context.Services.AddScoped<FacilityEligibilityDomainService>();
    return base.OnConfigureAsync(context);
  }
}
