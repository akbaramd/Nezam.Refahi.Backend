using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nezam.Refahi.Facilities.Application;
using Nezam.Refahi.Facilities.Domain.Repositories;
using Nezam.Refahi.Facilities.Domain.Services;
using Nezam.Refahi.Facilities.Infrastructure.Persistence;
using Nezam.Refahi.Facilities.Infrastructure.Persistence.Repositories;
using Nezam.Refahi.Facilities.Infrastructure.Seeding;
using Nezam.Refahi.Facilities.Infrastructure.Services;
using Nezam.Refahi.Shared.Infrastructure;

namespace Nezam.Refahi.Facilities.Infrastructure;

/// <summary>
/// Infrastructure module for Facilities bounded context
/// Configures database, repositories, and other infrastructure services
/// </summary>
public class NezamRefahiFacilitiesInfrastructureModule : BonWebModule
{
    public NezamRefahiFacilitiesInfrastructureModule()
    {
        DependOn<NezamRefahiFacilitiesApplicationModule>();
        DependOn<NezamRefahiSharedInfrastructureModule>();
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        // Configure DbContext
        context.Services.AddDbContext<FacilitiesDbContext>(options =>
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
                sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory__Facilities", "facilities");
            });
        });

        // Register repositories
        context.Services.AddScoped<IFacilityRepository, FacilityRepository>();
        context.Services.AddScoped<IFacilityCycleRepository, FacilityCycleRepository>();
        context.Services.AddScoped<IFacilityRequestRepository, FacilityRequestRepository>();

        // Register Unit of Work
        context.Services.AddScoped<IFacilitiesUnitOfWork, FacilitiesUnitOfWork>();


        // Register Domain Services
        context.Services.AddScoped<IUniqueConstraintManager, Infrastructure.Services.UniqueConstraintManager>();
        context.Services.AddScoped<Domain.Services.IFacilityCycleEligibilityService, Domain.Services.FacilityCycleEligibilityService>();
        context.Services.AddScoped<Domain.Services.IFacilityCycleValidationService, Domain.Services.FacilityCycleValidationService>();
        context.Services.AddScoped<Domain.Services.IFacilityQuotaService, Domain.Services.FacilityQuotaService>();
        context.Services.AddScoped<Domain.Services.IFacilityDependencyService, Domain.Services.FacilityDependencyService>();

        // Register Seeding Hosted Service
        context.Services.AddHostedService<FacilitiesSeedingHostedService>();

        return base.OnConfigureAsync(context);
    }
}