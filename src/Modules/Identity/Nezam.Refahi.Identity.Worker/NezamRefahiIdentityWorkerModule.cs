using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nezam.Refahi.Identity.Application;
using Nezam.Refahi.Identity.Application.Services.Contracts;
using Nezam.Refahi.Identity.Infrastructure;
using Nezam.Refahi.Identity.Infrastructure.ACL.Contracts;
using Nezam.Refahi.Identity.Worker.Services;
using Nezam.Refahi.Shared.Infrastructure;

namespace Nezam.Refahi.Identity.Worker;

/// <summary>
/// Identity Worker Module for background processing and scheduled tasks
/// </summary>
public class NezamRefahiIdentityWorkerModule : BonWebModule
{
    public NezamRefahiIdentityWorkerModule()
    {
        DependOn<NezamRefahiIdentityApplicationModule>();
        DependOn<NezamRefahiIdentityInfrastructureModule>();
        DependOn<NezamRefahiSharedInfrastructureModule>();
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        var services = context.Services;

        
        // Register worker services
        services.AddHostedService<UserSeedingWorkerService>();

        // Register Swagger services
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "Identity Worker API",
                Version = "v1",
                Description = "Background worker service for Identity module operations"
            });
        });

        return base.OnConfigureAsync(context);
    }

    public override Task OnApplicationAsync(BonWebApplicationContext context)
    {
        var app = context.Application;

        // Configure Swagger/OpenAPI
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity Worker API v1");
            c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
        });

        // Configure minimal API endpoints
        app.MapGet("/health", () => new { 
            Status = "Healthy", 
            Service = "Identity Worker", 
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0"
        })
          .WithName("HealthCheck")
          .WithOpenApi();

        // Add endpoint to manually trigger user seeding (for testing/admin purposes)
        app.MapPost("/trigger-user-seeding", async (IServiceProvider serviceProvider) =>
        {
            using var scope = serviceProvider.CreateScope();
            var orchestrator = scope.ServiceProvider.GetRequiredService<IUserSeedOrchestrator>();
            
            try
            {
                var watermark = new UserSeedWatermark
                {
                    LastProcessedId = "0", // Start from offset 0
                    LastProcessedUpdatedAt = DateTime.UtcNow
                };

                var result = await orchestrator.RunIncrementalAsync(
                    watermark: watermark,
                    batchSize: 1000, // Use same batch size as main service
                    maxParallel: 4,
                    dryRun: false,
                    cancellationToken: CancellationToken.None);

                return Results.Ok(new { 
                    Success = true, 
                    Message = "User seeding triggered successfully",
                    Processed = result.TotalProcessed,
                    Skipped = result.Skipped,
                    Failed = result.Failed,
                    NextOffset = result.LastWatermark?.LastProcessedId
                });
            }
            catch (Exception ex)
            {
                return Results.Problem($"Failed to trigger user seeding: {ex.Message}");
            }
        })
        .WithName("TriggerUserSeeding")
        .WithOpenApi();

        return base.OnApplicationAsync(context);
    }
}