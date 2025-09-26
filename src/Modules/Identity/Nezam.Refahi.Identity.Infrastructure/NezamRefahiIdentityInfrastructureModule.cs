using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Identity.Application;
using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Identity.Application.Services.Contracts;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Identity.Infrastructure.Persistence;
using Nezam.Refahi.Identity.Infrastructure.Persistence.Repositories;
using Nezam.Refahi.Identity.Infrastructure.Persistence.Seeding;
using Nezam.Refahi.Identity.Infrastructure.Providers;
using Nezam.Refahi.Identity.Infrastructure.Services;
using Nezam.Refahi.Shared.Infrastructure;
using Nezam.Refahi.Shared.Infrastructure.Providers;

namespace Nezam.Refahi.Identity.Infrastructure;

public class NezamRefahiIdentityInfrastructureModule : BonModule
{

  public NezamRefahiIdentityInfrastructureModule()
  {
    DependOn<NezamRefahiIdentityApplicationModule>();
    DependOn<NezamRefahiSharedInfrastructureModule>();
  }
  
  public override Task OnConfigureAsync(BonConfigurationContext context)
  {
    // ========================================================================
    // Production-Grade Token Services
    // ========================================================================
    
    // Register the enhanced TokenService with all dependencies
    context.Services.AddScoped<ITokenService>(provider =>
    {
      var configuration = provider.GetRequiredService<IConfiguration>();
      var userTokenRepository = provider.GetRequiredService<IUserTokenRepository>();
      var userRepository = provider.GetRequiredService<IUserRepository>();
      var distributedCache = provider.GetService<IDistributedCache>(); // Optional
      var logger = provider.GetService<ILogger<TokenService>>();
      
      return new TokenService(configuration, userTokenRepository, userRepository, distributedCache, logger);
    });
    
    // Legacy OTP service (if still needed)
    context.Services.AddScoped<IOtpService, OtpService>();
    
    // OTP hashing and generation services
    context.Services.AddSingleton<IOtpHasherService, OtpHasherService>();
    context.Services.AddSingleton<IOtpGeneratorService, OtpGeneratorService>();
    
    // OTP cleanup service for big data management
    context.Services.AddScoped<IOtpCleanupService, OtpCleanupService>();
    
    // Claims aggregator service
    context.Services.AddScoped<IClaimsAggregatorService, ClaimsAggregatorService>();
    
    // Register Identity Claims Provider
    context.Services.AddScoped<IIdentityClaimsPermissionProvider, IdentityClaimsProvider>();
    
    // ========================================================================
    // Background Services
    // ========================================================================
    
    // Token cleanup background service
    // TokenCleanupService moved to Hangfire jobs - runs at 4:30 AM daily
    
     context.Services.AddDbContext<IdentityDbContext>(options =>
     {
       options.EnableSensitiveDataLogging(true);
            var configuration = context.GetRequireService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string 'DefaultConnection' is not configured.");
            }
            options.UseSqlServer(connectionString, sqlOptions =>
            {
              sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory__Identity","identity");
            });
        });

        context.Services.AddScoped<IUserRepository, UserRepository>();
        context.Services.AddScoped<IUserTokenRepository, UserTokenRepository>();
        context.Services.AddScoped<IRefreshSessionRepository, RefreshSessionRepository>();  
        context.Services.AddScoped<IOtpChallengeRepository, OtpChallengeRepository>();
        context.Services.AddScoped<IUserPreferenceRepository, UserPreferenceRepository>();
        
        // Role and Claims repositories
        context.Services.AddScoped<IRoleRepository, RoleRepository>();
        context.Services.AddScoped<IRoleClaimRepository, RoleClaimRepository>();
        context.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        context.Services.AddScoped<IUserClaimRepository, UserClaimRepository>();
        
        // Register Unit of Work
        context.Services.AddScoped<IIdentityUnitOfWork, IdentityUnitOfWork>();
         context.Services.AddScoped<IScopeAuthorizationService,ScopeAuthorizationService>();
        // Register seeding services
        context.Services.AddScoped<RoleSeeder>();
        context.Services.AddScoped<UserSeeder>();
        context.Services.AddScoped<IdentityDataSeeder>();
        
        // Register hosted service for automatic seeding
        // IdentitySeedingService moved to Hangfire jobs - runs at 1:00 AM daily
        
        // Register user validation service
        context.Services.AddScoped<IUserValidationService, UserValidationService>();
        
    return base.OnConfigureAsync(context);
  }
}
