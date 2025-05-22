using Microsoft.Extensions.DependencyInjection;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Services;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Services;
using Nezam.Refahi.Domain.BoundedContexts.Payment.Services;

namespace Nezam.Refahi.Infrastructure.IoC;

/// <summary>
/// Extension methods for registering domain services in the DI container
/// </summary>
public static class DomainServiceExtensions
{
    /// <summary>
    /// Adds all domain services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        // Identity bounded context domain services
        services.AddScoped<TokenDomainService>();
        services.AddScoped<UserDomainService>();
        services.AddScoped<PaymentDomainService>();
        services.AddScoped<PaymentAccommodationIntegrationService>();
        services.AddScoped<ReservationDomainService>();
        
        // Survey bounded context domain services
        // Add survey domain services when implemented
        
        return services;
    }
}
