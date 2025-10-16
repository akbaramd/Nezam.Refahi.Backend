using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Identity.Worker;
using Serilog;
using Serilog.Events;
using Bonyan.Modularity;
using Bonyan.Plugins;

namespace Nezam.Refahi.Identity.Worker;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Warning()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Error)
            .MinimumLevel.Override("Nezam.Refahi", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("logs/identity-worker-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try
        {
            Log.Warning("Starting Identity Worker Service");

            var builder = BonyanApplication.CreateModularBuilder<NezamRefahiIdentityWorkerModule>("web-api", c =>
            {
                c.PlugInSources.AddFolder("./Plugins");
            });

            var app = await builder.BuildAsync();

            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Identity Worker Service terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
