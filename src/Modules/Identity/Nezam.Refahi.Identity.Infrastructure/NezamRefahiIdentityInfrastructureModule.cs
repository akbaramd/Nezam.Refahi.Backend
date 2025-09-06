using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Identity.Application;
using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Identity.Infrastructure.Persistence;
using Nezam.Refahi.Identity.Infrastructure.Persistence.Repositories;
using Nezam.Refahi.Identity.Infrastructure.Persistence.Seeding;
using Nezam.Refahi.Identity.Infrastructure.Services;
using Nezam.Refahi.Shared.Application.Common.Services;
using Nezam.Refahi.Shared.Infrastructure;

namespace Nezam.Refahi.Identity.Infrastructure;

public class NezamRefahiIdentityInfrastructureModule : BonModule
{

  public NezamRefahiIdentityInfrastructureModule()
  {
    DependOn<NezamRefahiIdentityApplicationModule>();
    DependOn<NezamRefahiSharedInfrastructureModule<IdentityDbContext>>();
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
    
    // ========================================================================
    // Background Services
    // ========================================================================
    
    // Token cleanup background service
    context.Services.AddHostedService<TokenCleanupService>();
    
     context.Services.AddDbContext<IdentityDbContext>(options =>
        {
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
        
        // Register seeding services
        context.Services.AddScoped<RoleSeeder>();
        context.Services.AddScoped<UserSeeder>();
        context.Services.AddScoped<IdentityDataSeeder>();
        
        // Register hosted service for automatic seeding
        context.Services.AddHostedService<IdentitySeedingService>();
        
    return base.OnConfigureAsync(context);
  }
}
