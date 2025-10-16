using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nezam.Refahi.BasicDefinitions.Application;
using Nezam.Refahi.BasicDefinitions.Application.Services;
using Nezam.Refahi.BasicDefinitions.Domain.Repositories;
using Nezam.Refahi.BasicDefinitions.Infrastructure.Persistence;
using Nezam.Refahi.BasicDefinitions.Infrastructure.Persistence.Repositories;
using Nezam.Refahi.Shared.Infrastructure;

namespace Nezam.Refahi.BasicDefinitions.Infrastructure;

public class NezamRefahiBasicDefinitionsInfrastructureModule : BonModule
{
    public NezamRefahiBasicDefinitionsInfrastructureModule()
    {
      DependOn<NezamRefahiBasicDefinitionsApplicationModule>();
        DependOn<NezamRefahiSharedInfrastructureModule>();
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        var configuration = context.GetRequireService<IConfiguration>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        // Register DbContext
        context.Services.AddDbContext<BasicDefinitionsDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(BasicDefinitionsDbContext).Assembly.FullName);
            });
        });

        // Register repositories
        context.Services.AddScoped<IAgencyRepository, AgencyRepository>();
        context.Services.AddScoped<IFeaturesRepository, FeaturesRepository>();
        context.Services.AddScoped<ICapabilityRepository, CapabilityRepository>();

        // Register Unit of Work
        context.Services.AddScoped<IBasicDefinitionsUnitOfWork, BasicDefinitionsUnitOfWork>();

        return base.OnConfigureAsync(context);
    }
}
