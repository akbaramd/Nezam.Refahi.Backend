using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using System.Reflection;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using MassTransit;
using MCA.SharedKernel.Application.Mappers.Extensions;
using MediatR;
using Nezam.Refahi.Finance.Application.Configuration;
using Microsoft.Extensions.Configuration;
using Nezam.Refahi.Contracts.Finance.v1.Messages;
using Nezam.Refahi.Finance.Application.Consumers;
using Nezam.Refahi.Finance.Domain;

namespace Nezam.Refahi.Finance.Application;

/// <summary>
/// Finance Application Module for dependency injection configuration
/// </summary>
public class NezamRefahiFinanceApplicationModule : BonModule
{
  public NezamRefahiFinanceApplicationModule()
  {
    DependOn<NezamRefahiFinanceDomainModule>();
  }

  public override Task OnPreConfigureAsync(BonConfigurationContext context)
  {
    context.Services.PreConfigure<Tuple<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>>((c) =>
    {

      // یک صف برای همه‌ی Commandهای فایننس
      c.Item2.ReceiveEndpoint(FinanceMessagingRoutes.FinanceMessagesQueueName, e =>
      {
        // QoS
        e.PrefetchCount = 32;
        e.ConcurrentMessageLimit = 16;

        // Retry کوتاه → سپس DLQ
        e.UseMessageRetry(r => { r.Interval(3, TimeSpan.FromSeconds(2)); });

        // Outbox مصرف‌کننده (در همین هاست)
        e.ConfigureConsumer<IssueBillMessageConsumer>(c.Item1);
        e.ConfigureConsumer<FailWalletDepositMessageConsumer>(c.Item1);
        e.ConfigureConsumer<CompleteWalletDepositMessageConsumer>(c.Item1);
        e.ConfigureConsumer<MarkWalletDepositAwaitingPaymentMessageConsumer>(c.Item1);
      });
    });
    return base.OnPreConfigureAsync(context);
  }

  public override Task OnConfigureAsync(BonConfigurationContext context)
  {
    var assembly = Assembly.GetExecutingAssembly();

    // Add MediatR
    context.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
    context.Services.AddMappers(cfg => cfg.AddAssembly(assembly));

    // Add FluentValidation
    context.Services.AddValidatorsFromAssembly(assembly);

    // Register configuration
    var configuration = context.GetRequireService<IConfiguration>();
    context.Services.Configure<FrontendSettings>(configuration.GetSection(FrontendSettings.SectionName));

    // Register BillService
    context.Services.AddScoped<Nezam.Refahi.Finance.Contracts.Services.IBillService, Nezam.Refahi.Finance.Application.Services.BillService>();

    return base.OnConfigureAsync(context);
  }
}
