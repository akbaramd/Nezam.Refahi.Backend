using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Nezam.Refahi.Application.Common.Interfaces;
using Nezam.Refahi.Infrastructure.Services;
using System.Text;

namespace Nezam.Refahi.Infrastructure.IoC;

/// <summary>
/// Extension methods for registering infrastructure services in the DI container
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Adds all infrastructure services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register infrastructure services by bounded context
        
        // Identity bounded context infrastructure services
        // These follow the Dependency Inversion Principle from SOLID
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IOtpService, OtpService>();
        services.AddScoped<INotificationService, NotificationService>();
        
        // Add memory cache for OTP service
        services.AddMemoryCache();
        
        // Add HTTP context accessor for CurrentUserService
        services.AddHttpContextAccessor();
        
        // Configure JWT authentication
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
                    configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is not configured"))),
                ValidateIssuer = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });
        
        return services;
    }
}
