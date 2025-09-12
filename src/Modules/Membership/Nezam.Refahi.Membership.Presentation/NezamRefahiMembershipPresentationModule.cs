using Bonyan.AspNetCore;
using Bonyan.Modularity;
using Nezam.Refahi.Membership.Infrastructure;

namespace Nezam.Refahi.Membership.Presentation;

public class NezamRefahiMembershipPresentationModule : BonWebModule
{
    public NezamRefahiMembershipPresentationModule()
    {
        DependOn<NezamRefahiMembershipInfrastructureModule>();
    }

    public override Task OnPostApplicationAsync(BonWebApplicationContext context)
    {
        var app = context.Application;
        
        // TODO: Map Membership endpoints when created
        // app.MapMembershipEndpoints();
        
        return base.OnPostApplicationAsync(context);
    }
}