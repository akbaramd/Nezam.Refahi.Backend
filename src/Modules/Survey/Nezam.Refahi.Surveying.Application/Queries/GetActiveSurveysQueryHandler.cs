using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Surveying.Contracts.Dtos;
using Nezam.Refahi.Surveying.Contracts.Queries;
using Nezam.Refahi.Surveying.Domain.Entities;
using Nezam.Refahi.Surveying.Domain.Enums;
using Nezam.Refahi.Surveying.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Surveying.Application.Queries;

/// <summary>
/// Handler for GetActiveSurveysQuery
/// </summary>
public class GetActiveSurveysQueryHandler : IRequestHandler<GetActiveSurveysQuery, ApplicationResult<ActiveSurveysResponse>>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ILogger<GetActiveSurveysQueryHandler> _logger;

    public GetActiveSurveysQueryHandler(
        ISurveyRepository surveyRepository,
        ILogger<GetActiveSurveysQueryHandler> logger)
    {
        _surveyRepository = surveyRepository;
        _logger = logger;
    }

    public async Task<ApplicationResult<ActiveSurveysResponse>> Handle(GetActiveSurveysQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Get active surveys
            var surveys = await _surveyRepository.GetActiveSurveysAsync(cancellationToken);

            // Apply filters
            if (!string.IsNullOrEmpty(request.FeatureKey))
            {
                surveys = surveys.Where(s => s.SurveyFeatures.Any(sf => sf.FeatureCode == request.FeatureKey));
            }

            if (!string.IsNullOrEmpty(request.CapabilityKey))
            {
                surveys = surveys.Where(s => s.SurveyCapabilities.Any(sc => sc.CapabilityCode == request.CapabilityKey));
            }

            // Apply pagination
            var pagedSurveys = surveys
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var surveyDtos = new List<SurveyDto>();

            foreach (var survey in pagedSurveys)
            {
                var surveyDto = MapToDto(survey);

                // Include questions if requested
                if (request.IncludeQuestions)
                {
                    surveyDto.Questions = survey.Questions
                        .OrderBy(q => q.Order)
                        .Select(MapQuestionToDto)
                        .ToList();
                }

                // Include features and capabilities
                surveyDto.Features = survey.SurveyFeatures
                    .Select(sf => new SurveyFeatureDto
                    {
                        Id = sf.Id,
                        SurveyId = sf.SurveyId,
                    FeatureCode = sf.FeatureCode,
                    FeatureTitleSnapshot = sf.FeatureTitleSnapshot
                    })
                    .ToList();

                surveyDto.Capabilities = survey.SurveyCapabilities
                    .Select(sc => new SurveyCapabilityDto
                    {
                        Id = sc.Id,
                        SurveyId = sc.SurveyId,
                    CapabilityCode = sc.CapabilityCode,
                    CapabilityTitleSnapshot = sc.CapabilityTitleSnapshot
                    })
                    .ToList();

                // Check participation eligibility
                if (request.UserId.HasValue)
                {
                    // Load survey with responses to get user's response
                    var surveyWithResponses = await _surveyRepository.GetWithResponsesAsync(survey.Id, cancellationToken);
                    var userResponse = surveyWithResponses?.Responses
                        .Where(r => r.Participant.MemberId == request.UserId.Value)
                        .OrderByDescending(r => r.AttemptNumber)
                        .FirstOrDefault();

                    surveyDto.ResponseCount = surveyWithResponses?.Responses.Count ?? 0;
                    surveyDto.CanParticipate = CanUserParticipate(survey, userResponse, request.UserId.Value);
                    surveyDto.IsAcceptingResponses = survey.State == SurveyState.Active;
                    
                    if (!surveyDto.CanParticipate)
                    {
                        surveyDto.ParticipationMessage = GetParticipationMessage(survey, userResponse);
                    }
                }

                surveyDtos.Add(surveyDto);
            }

            var response = new ActiveSurveysResponse
            {
                Surveys = surveyDtos,
                TotalCount = surveyDtos.Count,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling((double)surveyDtos.Count / request.PageSize),
                HasNextPage = request.PageNumber * request.PageSize < surveyDtos.Count,
                HasPreviousPage = request.PageNumber > 1
            };

            return ApplicationResult<ActiveSurveysResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active surveys");
            return ApplicationResult<ActiveSurveysResponse>.Failure("خطا در دریافت نظرسنجی‌های فعال");
        }
    }

    private static SurveyDto MapToDto(Survey survey)
    {
        return new SurveyDto
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
            MaxAttemptsPerMember = survey.ParticipationPolicy.MaxAttemptsPerMember,
            AllowMultipleSubmissions = survey.ParticipationPolicy.AllowMultipleSubmissions,
            CoolDownSeconds = survey.ParticipationPolicy.CoolDownSeconds,
            AudienceFilter = survey.AudienceFilter?.FilterExpression,
            TotalQuestions = survey.Questions.Count
        };
    }

    private static QuestionDto MapQuestionToDto(Question question)
    {
        return new QuestionDto
        {
            Id = question.Id,
            SurveyId = question.SurveyId,
            Kind = question.Kind.ToString(),
            KindText = GetQuestionKindText(question.Kind),
            Text = question.Text,
            Order = question.Order,
            IsRequired = question.IsRequired,
            Options = question.Options
                .Where(o => o.IsActive)
                .OrderBy(o => o.Order)
                .Select(MapOptionToDto)
                .ToList()
        };
    }

    private static Contracts.Dtos.QuestionOptionDto MapOptionToDto(QuestionOption option)
    {
        return new Contracts.Dtos.QuestionOptionDto
        {
            Id = option.Id,
            QuestionId = option.QuestionId,
            Text = option.Text,
            Order = option.Order,
            IsActive = option.IsActive
        };
    }

    private static bool CanUserParticipate(Survey survey, Response? existingResponse, Guid userId)
    {
        // Check if survey is active
        if (survey.State != SurveyState.Active)
            return false;

        // Check time constraints
        if (survey.StartAt.HasValue && DateTimeOffset.UtcNow < survey.StartAt.Value)
            return false;

        if (survey.EndAt.HasValue && DateTimeOffset.UtcNow > survey.EndAt.Value)
            return false;

        // Check attempt limits
        if (existingResponse != null)
        {
            if (!survey.ParticipationPolicy.AllowMultipleSubmissions)
                return false;

            if (existingResponse.AttemptNumber >= survey.ParticipationPolicy.MaxAttemptsPerMember)
                return false;

            // Check cooldown
            if (survey.ParticipationPolicy.CoolDownSeconds.HasValue)
            {
                var cooldownEnd = existingResponse.SubmittedAt?.AddSeconds(survey.ParticipationPolicy.CoolDownSeconds.Value);
                if (cooldownEnd.HasValue && DateTimeOffset.UtcNow < cooldownEnd.Value)
                    return false;
            }
        }

        return true;
    }

    private static string GetParticipationMessage(Survey survey, Response? existingResponse)
    {
        if (survey.State != SurveyState.Active)
            return "نظرسنجی در حال حاضر فعال نیست";

        if (survey.StartAt.HasValue && DateTimeOffset.UtcNow < survey.StartAt.Value)
            return "نظرسنجی هنوز شروع نشده است";

        if (survey.EndAt.HasValue && DateTimeOffset.UtcNow > survey.EndAt.Value)
            return "نظرسنجی به پایان رسیده است";

        if (existingResponse != null)
        {
            if (!survey.ParticipationPolicy.AllowMultipleSubmissions)
                return "شما قبلاً در این نظرسنجی شرکت کرده‌اید";

            if (existingResponse.AttemptNumber >= survey.ParticipationPolicy.MaxAttemptsPerMember)
                return "حداکثر تعداد تلاش‌های مجاز را انجام داده‌اید";

            if (survey.ParticipationPolicy.CoolDownSeconds.HasValue)
            {
                var cooldownEnd = existingResponse.SubmittedAt?.AddSeconds(survey.ParticipationPolicy.CoolDownSeconds.Value);
                if (cooldownEnd.HasValue && DateTimeOffset.UtcNow < cooldownEnd.Value)
                    return $"لطفاً {survey.ParticipationPolicy.CoolDownSeconds} ثانیه صبر کنید";
            }
        }

        return "شما می‌توانید در این نظرسنجی شرکت کنید";
    }

    private static string GetSurveyStateText(SurveyState state)
    {
        return state switch
        {
            SurveyState.Draft => "پیش‌نویس",
            SurveyState.Scheduled => "زمان‌بندی شده",
            SurveyState.Active => "فعال",
            SurveyState.Closed => "بسته شده",
            SurveyState.Archived => "آرشیو شده",
            _ => "نامشخص"
        };
    }

    private static string GetQuestionKindText(QuestionKind kind)
    {
        return kind switch
        {
            QuestionKind.FixedMCQ4 => "چهار گزینه‌ای ثابت",
            QuestionKind.ChoiceSingle => "انتخاب تک",
            QuestionKind.ChoiceMulti => "انتخاب چندگانه",
            QuestionKind.Textual => "متنی",
            _ => "نامشخص"
        };
    }
}
