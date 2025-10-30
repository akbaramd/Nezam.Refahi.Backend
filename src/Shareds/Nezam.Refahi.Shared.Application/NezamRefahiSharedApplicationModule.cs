using System.Reflection;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.DependencyInjection;

using Nezam.Refahi.Shared.Domain;

namespace Nezam.Refahi.Shared.Application;

public class NezamRefahiSharedApplicationModule : BonModule
{
  public NezamRefahiSharedApplicationModule()
  {
    DependOn<NezamRefahiSharedDomainModule>();
  }

  public override Task OnConfigureAsync(BonConfigurationContext context)
  {
    context.Services.AddMediatR(v =>
    {
      v.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    });


    return base.OnConfigureAsync(context);
  }
}
