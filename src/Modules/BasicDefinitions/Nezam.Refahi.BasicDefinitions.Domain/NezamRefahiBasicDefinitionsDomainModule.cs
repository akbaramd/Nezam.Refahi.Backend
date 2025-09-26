using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Nezam.Refahi.Shared.Domain;

namespace Nezam.Refahi.BasicDefinitions.Domain;

public class NezamRefahiBasicDefinitionsDomainModule : BonModule
{
    public NezamRefahiBasicDefinitionsDomainModule()
    {
        DependOn<NezamRefahiSharedDomainModule>();
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        // Domain services registration will be done here if needed
        return base.OnConfigureAsync(context);
    }
}
