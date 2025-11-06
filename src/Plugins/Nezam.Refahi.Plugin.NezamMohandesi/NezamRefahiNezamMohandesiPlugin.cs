using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nezam.Refahi.BasicDefinitions.Contracts.Services;
using Nezam.Refahi.Identity.Infrastructure.ACL.Contracts;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Plugin.NezamMohandesi.Adapters;
using Nezam.Refahi.Plugin.NezamMohandesi.Cedo;
using Nezam.Refahi.Plugin.NezamMohandesi.Seeding;
using Nezam.Refahi.Plugin.NezamMohandesi.Services;

namespace Nezam.Refahi.Plugin.NezamMohandesi;

public class NezamRefahiNezamMohandesiPlugin : BonModule
{
  public override Task OnConfigureAsync(BonConfigurationContext context)
  {
    var configuration = context.GetRequireService<IConfiguration>();
    
    // Register DbContext for scoped usage
    context.Services
      .AddDbContext<CedoContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("CedoConnection")));
    
    // Register DbContextFactory for creating contexts when needed (e.g., in background services)
    context.Services.AddDbContextFactory<CedoContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("CedoConnection")));
    
    // Register repositories

    // Register services
    context.Services.AddScoped<IExternalMemberStorage, ExternalMemberStorage>();
    
    // Register user seed sources
    context.Services.AddScoped<IUserSeedSource, CedoUserSeedSource>();
    
    // Register seed contributors
    context.Services.AddScoped<IMembershipSeedContributor, NezamMohandesiSeedContributor>();
    context.Services.AddScoped<IBasicDefinitionsSeedContributor, NezamMohandesiBasicDefinitionsSeedContributor>();
    
    // Register hosted services
    
    return base.OnConfigureAsync(context);
  }
}
