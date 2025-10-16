using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Nezam.Refahi.Surveying.Domain.Services;

namespace Nezam.Refahi.Surveying.Domain;

public class NezamRefahiSurveyingDomainModule : BonModule
{
    public NezamRefahiSurveyingDomainModule()
    {
    }

    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        // Register domain services
        context.Services.AddScoped<ParticipationRulesDomainService>();
        context.Services.AddScoped<SurveyAnalyticsService>();
        context.Services.AddScoped<SurveyValidationService>();
        context.Services.AddScoped<ParticipantService>();

        return Task.CompletedTask;
    }
}
