using System.Reflection;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Nezam.Refahi.Orchestrator.Persistence;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.Extensions.DependencyInjection;
using Nezam.Refahi.Contracts.Finance.v1.Messages;
using Nezam.Refahi.Orchestrator.Sagas.ReservationPayment;
using Nezam.Refahi.Orchestrator.Sagas.WalletDeposit;

namespace Nezam.Refahi.Orchestrator;

/// <summary>
/// Orchestrator Module - Manages and orchestrates events between modules using MassTransit Mediator
/// </summary>
public class NezamRefahiOrchestratorModule : BonModule
{
    public NezamRefahiOrchestratorModule()
    {
        // No dependencies - Orchestrator is a standalone module
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        var services = context.Services;
        var assembly = AppDomain.CurrentDomain.GetAssemblies();

        // Configure MassTransit with EF Core saga repository and EF Outbox (SQL Server)
        services.AddMassTransit(x =>
        {
            // Register all consumers in this assembly
            x.AddConsumers(assembly);

            // Register saga with EF Core repository (SQL Server)
            x.AddSagaStateMachine<ReservationPaymentSagaStateMachine, ReservationPaymentSagaState>()
                .EntityFrameworkRepository(r =>
                {
                    r.ConcurrencyMode = ConcurrencyMode.Optimistic;
                    r.AddDbContext<Microsoft.EntityFrameworkCore.DbContext, OrchestratorDbContext>((provider, builder) =>
                    {
                        var configuration = provider.GetRequiredService<IConfiguration>();
                        var connectionString = configuration.GetConnectionString("DefaultConnection")
                            ?? throw new InvalidOperationException("DefaultConnection not configured");
                        builder.UseSqlServer(connectionString, sql =>
                        {
                            sql.MigrationsHistoryTable("__EFMigrationsHistory__Orchestrator", "orchestrator");
                        });
                    });
                });

            // Wallet Deposit Saga
            x.AddSagaStateMachine<WalletDepositSagaStateMachine, WalletDepositSagaState>()
                .EntityFrameworkRepository(r =>
                {
                    r.ConcurrencyMode = ConcurrencyMode.Optimistic;
                    r.AddDbContext<Microsoft.EntityFrameworkCore.DbContext, OrchestratorDbContext>((provider, builder) =>
                    {
                        var configuration = provider.GetRequiredService<IConfiguration>();
                        var connectionString = configuration.GetConnectionString("DefaultConnection")
                            ?? throw new InvalidOperationException("DefaultConnection not configured");
                        builder.UseSqlServer(connectionString, sql =>
                        {
                            sql.MigrationsHistoryTable("__EFMigrationsHistory__Orchestrator", "orchestrator");
                        });
                    });
                    
                });

            // Configure EF Outbox for orchestrator
            x.AddEntityFrameworkOutbox<OrchestratorDbContext>(o =>
            {
         
                o.UseSqlServer();
                o.UseBusOutbox();
            });

            // Outbox برای انتشار مطمئن پیام‌ها (اختیاری اما توصیه‌شده)

            // Use RabbitMQ transport (adjust host as needed)
            x.UsingRabbitMq((ctx, cfg) =>
            {

              var configuration = ctx.GetRequiredService<IConfiguration>();
              cfg.Host("localhost", h =>
              {
                h.Username("guest");
                h.Password("guest");
              });

            

              context.Services.ExecutePreConfiguredActions(new Tuple<IBusRegistrationContext,IRabbitMqBusFactoryConfigurator>(ctx,cfg));
              
              cfg.ConfigureEndpoints(ctx);
             
            });
        });

        // Register OrchestratorDbContext for application use (Autofac resolve)
        services.AddDbContext<OrchestratorDbContext>((provider, builder) =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection not configured");
            builder.UseSqlServer(connectionString, sql =>
            {
                sql.MigrationsHistoryTable("__EFMigrationsHistory__Orchestrator", "orchestrator");
            });
        });


        return base.OnConfigureAsync(context);
    }
}

