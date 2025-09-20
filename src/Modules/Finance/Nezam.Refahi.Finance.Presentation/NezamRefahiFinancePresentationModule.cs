using Bonyan.Modularity;
using Nezam.Refahi.Finance.Infrastructure;

namespace Nezam.Refahi.Finance.Presentation;

public class NezamRefahiFinancePresentationModule : BonWebModule
{
    public NezamRefahiFinancePresentationModule()
    {
        DependOn<NezamRefahiFinanceInfrastructureModule>();
    }

    public override Task OnPostApplicationAsync(BonWebApplicationContext context)
    {
        var app = context.Application;
        

        
        return base.OnPostApplicationAsync(context);
    }
}
