using Microsoft.Extensions.DependencyInjection;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Repositories;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Repositories;
using Nezam.Refahi.Domain.BoundedContexts.Location.Repositories;
using Nezam.Refahi.Domain.BoundedContexts.Payment.Repositories;
using Nezam.Refahi.Domain.BoundedContexts.Shared.Repositories;
using Nezam.Refahi.Domain.BoundedContexts.Surveis.Repositories;
using Nezam.Refahi.Infrastructure.Persistence.Repositories;

namespace Nezam.Refahi.Infrastructure.IoC;

/// <summary>
/// Extension methods for registering repository implementations in the DI container
/// </summary>
public static class RepositoryExtensions
{
    /// <summary>
    /// Adds all repository implementations to the service collection manually
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Register generic repository interface and implementation
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        
        // Identity bounded context repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserTokenRepository, UserTokenRepository>();
        
        // Location bounded context repositories
        services.AddScoped<ICityRepository, CityRepository>();
        services.AddScoped<IProvinceRepository, ProvinceRepository>();
        
        // Accommodation bounded context repositories
        services.AddScoped<IHotelRepository, HotelRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        
        // Payment bounded context repositories
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        
        // Survey bounded context repositories
        services.AddScoped<ISurveyRepository, SurveyRepository>();
        services.AddScoped<ISurveyResponseRepository, SurveyResponseRepository>();
        services.AddScoped<ISurveyQuestionRepository, SurveyQuestionRepository>();
        services.AddScoped<ISurveyAnswerRepository, SurveyAnswerRepository>();
        services.AddScoped<ISurveyOptionsRepository, SurveyOptionsRepository>();
        
        return services;
    }
}
