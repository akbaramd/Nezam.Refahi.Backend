using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Nezam.Refahi.Notifications.Infrastructure;
using Nezam.Refahi.Notifications.Presentation.Endpoints;

namespace Nezam.Refahi.Notifications.Presentation;

/// <summary>
/// Notification Presentation Module
/// </summary>
public class NezamRefahiNotificationPresentationModule : BonWebModule
{
    public NezamRefahiNotificationPresentationModule()
    {
        DependOn<NezamRefahiNotificationInfrastructureModule>();
    }
    
 

    public override Task OnApplicationAsync(BonWebApplicationContext context)
    {
      var app = context.Application;
        
      // Map endpoints
      app.MapNotificationEndpoints();
      return base.OnApplicationAsync(context);
    }
}
