using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nezam.Refahi.Application.Common.Behaviors;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Repositories;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Services;
using Nezam.Refahi.Domain.BoundedContexts.Surveis.Repositories;

namespace Nezam.Refahi.Infrastructure.IoC;

/// <summary>
/// Extension methods for registering application services in the DI container
/// </summary>
public static class ApplicationExtensions
{
    /// <summary>
    /// Adds all application and domain services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Get the application assembly
        var applicationAssembly = Assembly.Load("Nezam.Refahi.Application");
        
        // Add MediatR
        services.AddMediatR(cfg => 
        {
            cfg.RegisterServicesFromAssembly(applicationAssembly);
            
            // Add pipeline behaviors
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });
        
        // Add FluentValidation
        services.AddValidatorsFromAssembly(applicationAssembly);
        
        // Register domain services by bounded context
        RegisterDomainServices(services);
        
        // Register repositories
        RegisterRepositories(services);
        
        return services;
    }
    
    /// <summary>
    /// Registers all domain services following DDD principles
    /// </summary>
    /// <param name="services">The service collection</param>
    private static void RegisterDomainServices(IServiceCollection services)
    {
        // Identity bounded context domain services
        services.AddScoped<UserDomainService>();
        
        // Survey bounded context domain services
        // Add survey domain services here
        
        // Add other domain services here as they are created
        // Each bounded context should have its own section
    }
    
    /// <summary>
    /// Registers all repositories following DDD principles
    /// </summary>
    /// <param name="services">The service collection</param>
    private static void RegisterRepositories(IServiceCollection services)
    {
        // Identity bounded context repositories
        // services.AddScoped<IUserRepository, UserRepository>();
        
        // Survey bounded context repositories
        // services.AddScoped<ISurveyRepository, SurveyRepository>();
        // services.AddScoped<ISurveyResponseRepository, SurveyResponseRepository>();
        // services.AddScoped<ISurveyQuestionRepository, SurveyQuestionRepository>();
        // services.AddScoped<ISurveyAnswerRepository, SurveyAnswerRepository>();
        // services.AddScoped<ISurveyOptionsRepository, SurveyOptionsRepository>();
    }
}
