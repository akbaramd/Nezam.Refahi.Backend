using Bonyan.AspNetCore;
using Bonyan.Modularity;
using Nezam.Refahi.Recreation.Infrastructure;
using Nezam.Refahi.Recreation.Presentation.Endpoints;

namespace Nezam.Refahi.Recreation.Presentation;

public class NezamRefahiRecreationPresentationModule : BonWebModule
{
    public NezamRefahiRecreationPresentationModule()
    {
        DependOn<NezamRefahiRecreationInfrastructureModule>();
    }

    public override Task OnPostApplicationAsync(BonWebApplicationContext context)
    {
        var app = context.Application;

        // Map Recreation endpoints
        app.MapTourEndpoints();
        app.MapMeReservationEndpoints();

        return base.OnPostApplicationAsync(context);
    }
}