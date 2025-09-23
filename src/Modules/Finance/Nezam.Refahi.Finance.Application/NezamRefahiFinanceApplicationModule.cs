using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using System.Reflection;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using MediatR;
using Nezam.Refahi.Finance.Application.Configuration;
using Microsoft.Extensions.Configuration;

namespace Nezam.Refahi.Finance.Application;

/// <summary>
/// Finance Application Module for dependency injection configuration
/// </summary>
public  class NezamRefahiFinanceApplicationModule : BonModule
{
  public override Task OnConfigureAsync(BonConfigurationContext context)
  {
    var assembly = Assembly.GetExecutingAssembly();

    // Add MediatR
    context.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

    // Add FluentValidation
    context.Services.AddValidatorsFromAssembly(assembly);

    // Register configuration
    var configuration = context.GetRequireService<IConfiguration>();
    context.Services.Configure<FrontendSettings>(configuration.GetSection(FrontendSettings.SectionName));

    return base.OnConfigureAsync(context);
  }
}