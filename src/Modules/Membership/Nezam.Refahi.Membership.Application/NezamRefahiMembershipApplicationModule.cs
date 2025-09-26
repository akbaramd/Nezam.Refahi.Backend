using System.Reflection;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Nezam.Refahi.Membership.Application.HostedServices;
using Nezam.Refahi.Membership.Application.Services;
using Nezam.Refahi.Membership.Contracts;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Membership.Domain;
using Nezam.Refahi.Shared.Application;

namespace Nezam.Refahi.Membership.Application;

public class NezamRefahiMembershipApplicationModule : BonModule
{
  public NezamRefahiMembershipApplicationModule()
  {
    DependOn<NezamRefahiSharedApplicationModule>();
    DependOn<NezamRefahiMembershipContractsModule>();
    DependOn<NezamRefahiMembershipDomainModule>();
     
    
  }
  public override Task OnConfigureAsync(BonConfigurationContext context)
  {
    context.Services.AddMediatR(v =>
    {
      v.RegisterServicesFromAssembly(typeof(NezamRefahiMembershipApplicationModule).Assembly);
    });

    context.Services.AddValidatorsFromAssembly(typeof(NezamRefahiMembershipApplicationModule).Assembly);

    // Register Membership service for inter-context communication
    context.Services.AddScoped<IMemberService, MemberService>();

    // Register membership seeding hosted service
    // MembershipSeedingHostedService moved to Hangfire jobs - runs at 1:30 AM daily
    return base.OnConfigureAsync(context);
  }
}