using Bonyan.Modularity;
using Nezam.Refahi.Facilities.Application;
using Nezam.Refahi.Facilities.Infrastructure;
using Nezam.Refahi.Facilities.Presentation.Endpoints;

namespace Nezam.Refahi.Facilities.Presentation;

public class NezamRefahiFacilitiesPresentationModule : BonWebModule
{
    public NezamRefahiFacilitiesPresentationModule()
    {
        DependOn<NezamRefahiFacilitiesInfrastructureModule>();
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        // Register presentation services here if needed
        return base.OnConfigureAsync(context);
    }

    public override Task OnPostApplicationAsync(BonWebApplicationContext context)
    {
        var app = context.Application;
        
        // Register endpoints here when they are created
        app.MapFacilityEndpoints();

        return base.OnPostApplicationAsync(context);
    }
}
