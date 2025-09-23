using Bonyan.Modularity;
using Nezam.Refahi.Finance.Infrastructure;
using Nezam.Refahi.Finance.Presentation.Endpoints;

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
        
        // Register Payment endpoints
        app.MapPaymentEndpoints();
        app.MapBillEndpoints();
        app.MapWalletEndpoints();

        return base.OnPostApplicationAsync(context);
    }
}
