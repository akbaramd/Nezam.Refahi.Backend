using Bonyan.AspNetCore;
using Bonyan.Modularity;
using Nezam.Refahi.BasicDefinitions.Infrastructure;
using Nezam.Refahi.BasicDefinitions.Presentation.Endpoints;

namespace Nezam.Refahi.BasicDefinitions.Presentation;

public class NezamRefahiBasicDefinitionsPresentationModule : BonWebModule
{
    public NezamRefahiBasicDefinitionsPresentationModule()
    {
        DependOn<NezamRefahiBasicDefinitionsInfrastructureModule>();
    }

    public override Task OnPostApplicationAsync(BonWebApplicationContext context)
    {
        var app = context.Application;
        
        // Map endpoints
        app.MapAgencyEndpoints();
        
        return base.OnPostApplicationAsync(context);
    }
}