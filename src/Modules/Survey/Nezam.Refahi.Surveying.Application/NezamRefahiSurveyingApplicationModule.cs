using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Nezam.Refahi.Surveying.Contracts;
using Nezam.Refahi.Surveying.Domain;
using Nezam.Refahi.Shared.Application;
using FluentValidation;

namespace Nezam.Refahi.Surveying.Application;

public class NezamRefahiSurveyingApplicationModule : BonModule
{
    public NezamRefahiSurveyingApplicationModule()
    {
        DependOn<NezamRefahiSharedApplicationModule>();
        DependOn<NezamRefahiSurveyingContractsModule>();
        DependOn<NezamRefahiSurveyingDomainModule>();
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        context.Services.AddMediatR(v =>
        {
            v.RegisterServicesFromAssembly(typeof(NezamRefahiSurveyingApplicationModule).Assembly);
        });

        context.Services.AddValidatorsFromAssembly(typeof(NezamRefahiSurveyingApplicationModule).Assembly);

        return base.OnConfigureAsync(context);
    }
}
