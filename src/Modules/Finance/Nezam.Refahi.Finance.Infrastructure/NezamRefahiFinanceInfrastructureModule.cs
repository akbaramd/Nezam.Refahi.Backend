using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nezam.Refahi.Finance.Application;
using Nezam.Refahi.Finance.Application.Services;
using Nezam.Refahi.Finance.Contracts.Services;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Finance.Infrastructure.Consumers;
using Nezam.Refahi.Finance.Infrastructure.Persistence;
using Nezam.Refahi.Finance.Infrastructure.Persistence.Repositories;
using Nezam.Refahi.Finance.Infrastructure.Services;
using Nezam.Refahi.Shared.Infrastructure;
using Parbad.Builder;
using Parbad.Gateway.ParbadVirtual;
using Parbad.Gateway.Parsian;

namespace Nezam.Refahi.Finance.Infrastructure;

/// <summary>
/// Infrastructure module for Finance bounded context
/// Configures database, repositories, and other infrastructure services
/// </summary>
public class NezamRefahiFinanceInfrastructureModule : BonWebModule
{
    public NezamRefahiFinanceInfrastructureModule()
    {
        DependOn<NezamRefahiFinanceApplicationModule>();
        DependOn<NezamRefahiSharedInfrastructureModule>();
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
      #region Parbad
      context.Services.AddParbad()
        .ConfigureGateways(g =>
        {
          g.AddParsian().WithAccounts(c =>
          {
            c.AddInMemory(account =>
            {
              account.LoginAccount = "njHfBu23DkQ4S0Vy2m50";
              account.Name = "Urmia";
            });
          });
          g.AddParbadVirtual()
            .WithOptions(o => o.GatewayPath = "/MyVirtualGateway");
        })
        .ConfigureHttpContext(h => h.UseDefaultAspNetCore())
        .ConfigureStorage(s => s.UseMemoryCache());
      #endregion
      
        // Configure DbContext
        context.Services.AddDbContext<FinanceDbContext>(options =>
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
                sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory__Finance", "finance");
            });
        });

        // Register repositories
        context.Services.AddScoped<IBillRepository, BillRepository>();
        context.Services.AddScoped<IBillItemRepository, BillItemRepository>();
        context.Services.AddScoped<IPaymentRepository, PaymentRepository>();
        context.Services.AddScoped<IRefundRepository, RefundRepository>();
        context.Services.AddScoped<IWalletRepository, WalletRepository>();
        context.Services.AddScoped<IWalletTransactionRepository, WalletTransactionRepository>();
        context.Services.AddScoped<IWalletDepositRepository, WalletDepositRepository>();

        // Register Unit of Work
        context.Services.AddScoped<IFinanceUnitOfWork, FinanceUnitOfWork>();

        // Register Services
        context.Services.AddScoped<IPaymentService, PaymentService>();

        // Register Event Consumers
        context.Services.AddScoped<WalletChargePaymentCompletedConsumer>();

        return base.OnConfigureAsync(context);
    }

    
}