using Bonyan.AspNetCore;
using Bonyan.Modularity;
using Nezam.Refahi.Settings.Infrastructure;
using Nezam.Refahi.Settings.Presentation.Endpoints;

namespace Nezam.Refahi.Settings.Presentation;

public class NezamRefahiSettingsPresentationModule : BonWebModule
{
    public NezamRefahiSettingsPresentationModule()
    {
        DependOn<NezamRefahiSettingsInfrastructureModule>();
    }

    public override Task OnPostApplicationAsync(BonWebApplicationContext context)
    {
        var app = context.Application;
        
        // Map Settings endpoints
        app.MapSettingsEndpoints();
        
        return base.OnPostApplicationAsync(context);
    }
}
