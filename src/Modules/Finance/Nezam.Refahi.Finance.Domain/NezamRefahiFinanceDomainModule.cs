

using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Nezam.Refahi.Finance.Domain.Services;
using Nezam.Refahi.Shared.Domain;

namespace Nezam.Refahi.Finance.Domain;


public class NezamRefahiFinanceDomainModule : BonModule
{
  public NezamRefahiFinanceDomainModule()
  {
    DependOn<NezamRefahiSharedDomainModule>();
  }

  public override Task OnConfigureAsync(BonConfigurationContext context)
  {
    context.Services.AddScoped<WalletDomainService>();
    return base.OnConfigureAsync(context);
  }
}