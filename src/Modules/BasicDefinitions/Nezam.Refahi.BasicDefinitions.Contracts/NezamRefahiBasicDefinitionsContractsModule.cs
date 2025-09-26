using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Nezam.Refahi.BasicDefinitions.Domain;

namespace Nezam.Refahi.BasicDefinitions.Contracts;

public class NezamRefahiBasicDefinitionsContractsModule : BonModule
{
    public NezamRefahiBasicDefinitionsContractsModule()
    {
        DependOn<NezamRefahiBasicDefinitionsDomainModule>();
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        return base.OnConfigureAsync(context);
    }
}
