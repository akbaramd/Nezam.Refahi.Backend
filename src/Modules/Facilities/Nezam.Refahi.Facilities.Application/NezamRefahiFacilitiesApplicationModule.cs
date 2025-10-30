using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using System.Reflection;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using MediatR;
using Nezam.Refahi.Facilities.Domain;
using MCA.SharedKernel.Application.Mappers.Extensions;

namespace Nezam.Refahi.Facilities.Application;

/// <summary>
/// Facilities Application Module for dependency injection configuration
/// </summary>
public class NezamRefahiFacilitiesApplicationModule : BonModule
{

  public NezamRefahiFacilitiesApplicationModule()
  {
    DependOn<NezamRefahiFacilitiesDomainModule>();
  }
  public override Task OnConfigureAsync(BonConfigurationContext context)
  {
    var assembly = Assembly.GetExecutingAssembly();

    context.Services.AddMappers(c => c.AddAssembly(assembly));
    // Add MediatR
    context.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

    // Add FluentValidation
    context.Services.AddValidatorsFromAssembly(assembly);

    return base.OnConfigureAsync(context);
  }
}
