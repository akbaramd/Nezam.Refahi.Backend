using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nezam.Refahi.Recreation.Application;
using Nezam.Refahi.Recreation.Application.Services;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Recreation.Infrastructure.Persistence;
using Nezam.Refahi.Recreation.Infrastructure.Persistence.Repositories;
using Nezam.Refahi.Recreation.Infrastructure.Services;
using Nezam.Refahi.Recreation.Infrastructure.HostedServices;
using Nezam.Refahi.Shared.Infrastructure;

namespace Nezam.Refahi.Recreation.Infrastructure;

public class NezamRefahiRecreationInfrastructureModule : BonModule
{
    public NezamRefahiRecreationInfrastructureModule()
    {
        DependOn<NezamRefahiRecreationApplicationModule>();
        DependOn<NezamRefahiSharedInfrastructureModule>();
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        // Configure DbContext
        context.Services.AddDbContext<RecreationDbContext>(options =>
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
                sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory__Recreation", "recreation");
            });
        });

        // Register repositories
        context.Services.AddScoped<ITourRepository, TourRepository>();
        context.Services.AddScoped<ITourCapacityRepository, TourCapacityRepository>();
        context.Services.AddScoped<IParticipantRepository, ParticipantRepository>();
        context.Services.AddScoped<ITourReservationRepository, TourReservationRepository>();
        context.Services.AddScoped<IApiIdempotencyRepository, ApiIdempotencyRepository>();
        context.Services.AddScoped<ITourPricingRepository, TourPricingRepository>();
        context.Services.AddScoped<ITourPhotoRepository, TourPhotoRepository>();
        context.Services.AddScoped<ITourFeatureRepository, TourFeatureRepository>();
        context.Services.AddScoped<IFeatureRepository, FeatureRepository>();
        context.Services.AddScoped<IFeatureCategoryRepository, FeatureCategoryRepository>();
        context.Services.AddScoped<ITourMemberCapabilityRepository, TourMemberCapabilityRepository>();
        context.Services.AddScoped<ITourMemberFeatureRepository, TourMemberFeatureRepository>();
        context.Services.AddScoped<ITourRestrictedTourRepository, TourRestrictedTourRepository>();

        // Register Unit of Work
        context.Services.AddScoped<IRecreationUnitOfWork, RecreationUnitOfWork>();

        // Register seeding services
        context.Services.AddScoped<ITourSeedingService, TourSeedingService>();
        context.Services.AddHostedService<TourSeedingHostedService>();

        // Register cleanup configuration and service
        context.Services.Configure<ReservationCleanupOptions>(
            context.GetRequireService<IConfiguration>().GetSection(ReservationCleanupOptions.SectionName));
        context.Services.AddHostedService<ReservationCleanupService>();

        return base.OnConfigureAsync(context);
    }
}