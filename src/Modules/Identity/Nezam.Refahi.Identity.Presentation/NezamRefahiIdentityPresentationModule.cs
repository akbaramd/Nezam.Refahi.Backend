using Bonyan.AspNetCore;
using Bonyan.Modularity;
using Nezam.Refahi.Identity.Infrastructure;
using Nezam.Refahi.Identity.Presentation.Endpoints;

namespace Nezam.Refahi.Identity.Presentation;

public class NezamRefahiIdentityPresentationModule : BonWebModule
{
    public NezamRefahiIdentityPresentationModule()
    {
        DependOn<NezamRefahiIdentityInfrastructureModule>();
    }

    public override Task OnPostApplicationAsync(BonWebApplicationContext context)
    {
        var app = context.Application;
        
        // Map Identity endpoints
        app.MapIdentityEndpoints();
        
        return base.OnPostApplicationAsync(context);
    }
}
