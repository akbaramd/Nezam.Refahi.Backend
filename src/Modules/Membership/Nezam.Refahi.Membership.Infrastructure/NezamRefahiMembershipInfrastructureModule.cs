using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nezam.Refahi.Membership.Application;
using Nezam.Refahi.Membership.Application.Services;
using Nezam.Refahi.Membership.Domain.Repositories;
using Nezam.Refahi.Membership.Infrastructure.Persistence;
using Nezam.Refahi.Membership.Infrastructure.Providers;
using Nezam.Refahi.Membership.Infrastructure.Persistence.Repositories;
using Nezam.Refahi.Shared.Infrastructure;
using Nezam.Refahi.Shared.Infrastructure.Providers;

namespace Nezam.Refahi.Membership.Infrastructure;

public class NezamRefahiMembershipInfrastructureModule : BonModule
{

  public NezamRefahiMembershipInfrastructureModule()
  {
    DependOn<NezamRefahiMembershipApplicationModule>();
    DependOn<NezamRefahiSharedInfrastructureModule>();
  }
  
  public override Task OnConfigureAsync(BonConfigurationContext context)
  {
    // Configure DbContext
    context.Services.AddDbContext<MembershipDbContext>(options =>
    {
        var configuration = context.GetRequireService<IConfiguration>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Database connection string 'DefaultConnection' is not configured.");
        }

        options.EnableSensitiveDataLogging();
        options.UseSqlServer(connectionString, sqlOptions =>
        {
          sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory__Membership","membership");
        });
    });

    // Register repositories
    context.Services.AddScoped<IMemberRepository, MemberRepository>();
    context.Services.AddScoped<IRoleRepository, RoleRepository>();
    context.Services.AddScoped<IMemberRoleRepository, MemberRoleRepository>();
    context.Services.AddScoped<IMemberCapabilityRepository, MemberCapabilityRepository>();
    context.Services.AddScoped<IMemberFeatureRepository, MemberFeatureRepository>();
    context.Services.AddScoped<IMemberAgencyRepository, MemberAgencyRepository>();
    
    // Register Unit of Work
    context.Services.AddScoped<IMembershipUnitOfWork, MembershipUnitOfWork>();
    // Register Identity Claims Provider
    context.Services.AddScoped<IIdentityClaimsPermissionProvider, MembershipClaimsProvider>();
    return base.OnConfigureAsync(context);
  }
}