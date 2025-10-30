using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using System.Reflection;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using MCA.SharedKernel.Application.Mappers.Extensions;
using MediatR;
using Nezam.Refahi.Finance.Application.Configuration;
using Microsoft.Extensions.Configuration;
using Nezam.Refahi.Finance.Domain;

namespace Nezam.Refahi.Finance.Application;

/// <summary>
/// Finance Application Module for dependency injection configuration
/// </summary>
public  class NezamRefahiFinanceApplicationModule : BonModule
{

  public NezamRefahiFinanceApplicationModule()
  {
    DependOn<NezamRefahiFinanceDomainModule>();
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

    return base.OnConfigureAsync(context);
  }
}
