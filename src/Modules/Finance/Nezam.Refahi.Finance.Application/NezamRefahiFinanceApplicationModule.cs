using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using System.Reflection;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using MediatR;

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
    return base.OnConfigureAsync(context);
  }
}