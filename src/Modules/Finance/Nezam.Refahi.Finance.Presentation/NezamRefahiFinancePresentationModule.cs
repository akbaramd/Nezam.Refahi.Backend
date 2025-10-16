using Bonyan.Modularity;
using Nezam.Refahi.Finance.Application;
using Nezam.Refahi.Finance.Infrastructure;
using Nezam.Refahi.Finance.Presentation.Endpoints;
using Nezam.Refahi.Finance.Presentation.Services;

namespace Nezam.Refahi.Finance.Presentation;

public class NezamRefahiFinancePresentationModule : BonWebModule
{
    public NezamRefahiFinancePresentationModule()
    {
        DependOn<NezamRefahiFinanceInfrastructureModule>();
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
      context.Services.AddHostedService<WalletSnapshotHostedService>();
      context.Services.AddScoped<WalletSnapshotJob>();
      return base.OnConfigureAsync(context);
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
