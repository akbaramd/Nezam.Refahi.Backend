using System.Reflection;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Nezam.Refahi.Notifications.Application.EventHandlers.Finance;
using Nezam.Refahi.Notifications.Application.EventHandlers.Recreation;
using Nezam.Refahi.Notifications.Application.Services;
using Nezam.Refahi.Notifications.Domain.Services;
using Nezam.Refahi.Shared.Application;

namespace Nezam.Refahi.Notifications.Application;

/// <summary>
/// Notification Application Module
/// </summary>
public class NezamRefahiNotificationApplicationModule : BonModule
{
    public NezamRefahiNotificationApplicationModule()
    {
        DependOn<NezamRefahiSharedApplicationModule>();
    }
    
    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        // Register application services
        context.Services.AddScoped<INotificationService, NotificationService>();
        
        // Register domain services
        context.Services.AddScoped<INotificationDomainService, NotificationDomainService>();
        
        // Register event handlers
        var assembly = Assembly.GetExecutingAssembly();

        // Add MediatR
        context.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // Add FluentValidation
        context.Services.AddValidatorsFromAssembly(assembly);
        
        return base.OnConfigureAsync(context);
    }
}
