using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;

namespace Nezam.Refahi.Surveying.Contracts;

public class NezamRefahiSurveyingContractsModule : BonModule
{
    public NezamRefahiSurveyingContractsModule()
    {
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        return Task.CompletedTask;
    }
}
