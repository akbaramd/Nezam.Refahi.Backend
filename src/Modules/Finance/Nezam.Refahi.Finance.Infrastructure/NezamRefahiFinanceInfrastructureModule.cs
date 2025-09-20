using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nezam.Refahi.Finance.Application;
using Nezam.Refahi.Finance.Application.Services;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Finance.Infrastructure.Persistence;
using Nezam.Refahi.Finance.Infrastructure.Persistence.Repositories;
using Nezam.Refahi.Shared.Infrastructure;

namespace Nezam.Refahi.Finance.Infrastructure;

/// <summary>
/// Infrastructure module for Finance bounded context
/// Configures database, repositories, and other infrastructure services
/// </summary>
public class NezamRefahiFinanceInfrastructureModule : BonModule
{
    public NezamRefahiFinanceInfrastructureModule()
    {
        DependOn<NezamRefahiFinanceApplicationModule>();
        DependOn<NezamRefahiSharedInfrastructureModule>();
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
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

        // Register Unit of Work
        context.Services.AddScoped<IFinanceUnitOfWork, FinanceUnitOfWork>();

        return base.OnConfigureAsync(context);
    }
}