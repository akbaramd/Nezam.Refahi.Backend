using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Surveying.Application.Services;
using Nezam.Refahi.Surveying.Contracts.Queries;
using Nezam.Refahi.Surveying.Domain.Entities;
using Nezam.Refahi.Surveying.Domain.Enums;
using Nezam.Refahi.Surveying.Domain.Repositories;

namespace Nezam.Refahi.Surveying.Application.Queries;

/// <summary>
/// Handler for GetSurveyOverviewQuery
/// </summary>
public class GetSurveyOverviewQueryHandler : IRequestHandler<GetSurveyOverviewQuery, ApplicationResult<SurveyOverviewResponse>>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ILogger<GetSurveyOverviewQueryHandler> _logger;

    public GetSurveyOverviewQueryHandler(
        ISurveyRepository surveyRepository,
        ILogger<GetSurveyOverviewQueryHandler> logger)
    {
        _surveyRepository = surveyRepository;
        _logger = logger;
    }

    public async Task<ApplicationResult<SurveyOverviewResponse>> Handle(
        GetSurveyOverviewQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting survey overview for survey {SurveyId}", request.SurveyId);

            var survey = await _surveyRepository.GetByIdAsync(request.SurveyId, cancellationToken);
            if (survey == null)
            {
                _logger.LogWarning("Survey {SurveyId} not found", request.SurveyId);
                return ApplicationResult<SurveyOverviewResponse>.Failure("Survey not found");
            }

            var response = MapToResponse(survey);
            
            _logger.LogInformation("Successfully retrieved survey overview for survey {SurveyId}", request.SurveyId);
            return ApplicationResult<SurveyOverviewResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting survey overview for survey {SurveyId}", request.SurveyId);
            return ApplicationResult<SurveyOverviewResponse>.Failure("An error occurred while retrieving survey overview");
        }
    }

    private static SurveyOverviewResponse MapToResponse(Survey survey)
    {
        var orderedQuestions = survey.GetOrderedQuestions().ToList();
        var requiredQuestions = orderedQuestions.Count(q => q.IsRequired);
        
        // Calculate estimated duration (rough estimate: 30 seconds per question)
        var estimatedDuration = TimeSpan.FromSeconds(orderedQuestions.Count * 30);

        return new SurveyOverviewResponse
        {
            Id = survey.Id,
            Title = survey.Title,
            Description = survey.Description,
            State = survey.State.ToString(),
            StateText = GetSurveyStateText(survey.State),
            StartAt = survey.StartAt?.DateTime,
            EndAt = survey.EndAt?.DateTime,
            IsAnonymous = survey.IsAnonymous,
            CreatedAt = survey.CreatedAt,
            
            // Participation policy
            MaxAttemptsPerMember = survey.ParticipationPolicy.MaxAttemptsPerMember,
            AllowMultipleSubmissions = survey.ParticipationPolicy.AllowMultipleSubmissions,
            CoolDownSeconds = survey.ParticipationPolicy.CoolDownSeconds,
            AllowBackNavigation = survey.ParticipationPolicy.AllowBackNavigation,
            
            // Survey structure
            TotalQuestions = orderedQuestions.Count,
            RequiredQuestions = requiredQuestions,
            EstimatedDuration = estimatedDuration,
            
            // Features and capabilities
            FeatureCodes = survey.SurveyFeatures.Select(sf => sf.FeatureCode).ToList(),
            CapabilityCodes = survey.SurveyCapabilities.Select(sc => sc.CapabilityCode).ToList(),
            
            // Audience filter
            AudienceFilter = survey.AudienceFilter?.FilterExpression,
            
            // Structure versioning
            StructureVersion = survey.StructureVersion,
            IsStructureFrozen = survey.IsStructureFrozen
        };
    }

    private static string GetSurveyStateText(SurveyState state)
    {
        return state switch
        {
            SurveyState.Draft => "پیش‌نویس",
            SurveyState.Active => "فعال",
            SurveyState.Paused => "متوقف",
            SurveyState.Closed => "بسته",
            SurveyState.Archived => "آرشیو شده",
            _ => "نامشخص"
        };
    }
}
