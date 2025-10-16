using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nezam.Refahi.Surveying.Application;
using Nezam.Refahi.Surveying.Domain.Repositories;
using Nezam.Refahi.Surveying.Infrastructure.Persistence;
using Nezam.Refahi.Surveying.Infrastructure.Persistence.Repositories;
using Nezam.Refahi.Shared.Infrastructure;
using Nezam.Refahi.Surveying.Application.Services;
using Nezam.Refahi.Surveying.Infrastructure.Seeding;

namespace Nezam.Refahi.Surveying.Infrastructure;

public class NezamRefahiSurveyingInfrastructureModule : BonModule
{
    public NezamRefahiSurveyingInfrastructureModule()
    {
        DependOn<NezamRefahiSurveyingApplicationModule>();
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
        context.Services.AddDbContext<SurveyDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(SurveyDbContext).Assembly.FullName);
            });
        });

        // Register repositories
        context.Services.AddScoped<ISurveyRepository, SurveyRepository>();

        // Register Unit of Work
        context.Services.AddScoped<ISurveyUnitOfWork, SurveyUnitOfWork>();

        // Register seeding hosted service
        context.Services.AddHostedService<SurveySeedingHostedService>();

        return base.OnConfigureAsync(context);
    }
}
