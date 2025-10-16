using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Nezam.Refahi.Surveying.Application;
using Nezam.Refahi.Surveying.Contracts;
using Nezam.Refahi.Surveying.Infrastructure;
using Nezam.Refahi.Surveying.Presentation.Endpoints;

namespace Nezam.Refahi.Surveying.Presentation;

public class NezamRefahiSurveyingPresentationModule : BonWebModule
{
    public NezamRefahiSurveyingPresentationModule()
    {
        DependOn<NezamRefahiSurveyingApplicationModule>();
        DependOn<NezamRefahiSurveyingContractsModule>();
        DependOn<NezamRefahiSurveyingInfrastructureModule>();
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        // Register presentation services if needed
        return base.OnConfigureAsync(context);
    }

    public override Task OnPostApplicationAsync(BonWebApplicationContext context)
    {
        var app = context.Application;
        
        // Register Survey endpoints
        app.MapSurveyEndpoints();

        return base.OnPostApplicationAsync(context);
    }
}
