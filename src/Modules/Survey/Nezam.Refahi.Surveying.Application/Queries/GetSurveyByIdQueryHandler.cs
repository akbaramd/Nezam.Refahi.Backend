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
/// Handler for GetSurveyByIdQuery
/// </summary>
public class GetSurveyByIdQueryHandler : IRequestHandler<GetSurveyByIdQuery, ApplicationResult<SurveyDto>>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ILogger<GetSurveyByIdQueryHandler> _logger;

    public GetSurveyByIdQueryHandler(
        ISurveyRepository surveyRepository,
        ILogger<GetSurveyByIdQueryHandler> logger)
    {
        _surveyRepository = surveyRepository;
        _logger = logger;
    }

    public async Task<ApplicationResult<SurveyDto>> Handle(GetSurveyByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Get survey with responses if user ID is provided
            Survey? survey;
            if (request.UserId.HasValue)
            {
                survey = await _surveyRepository.GetWithResponsesAsync(request.SurveyId, cancellationToken);
            }
            else
            {
                survey = await _surveyRepository.GetByIdAsync(request.SurveyId, cancellationToken);
            }
            
            if (survey == null)
                return ApplicationResult<SurveyDto>.Failure("نظرسنجی یافت نشد");

            var surveyDto = MapToDto(survey);

            // Include questions if requested
            if (request.IncludeQuestions)
            {
                surveyDto.Questions = survey.Questions
                    .OrderBy(q => q.Order)
                    .Select(MapQuestionToDto)
                    .ToList();

                surveyDto.TotalQuestions = surveyDto.Questions.Count;
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
                var userResponses = survey.Responses
                    .Where(r => r.Participant.MemberId == request.UserId.Value)
                    .OrderByDescending(r => r.AttemptNumber)
                    .ToList();

                surveyDto.ResponseCount = survey.Responses.Count;
                surveyDto.CanParticipate = CanUserParticipate(survey, userResponses.FirstOrDefault(), request.UserId.Value);
                surveyDto.CanUserParticipate = CanUserParticipate(survey, userResponses.FirstOrDefault(), request.UserId.Value);
                surveyDto.IsAcceptingResponses = survey.State == SurveyState.Active;
                
                if (!surveyDto.CanParticipate)
                {
                    surveyDto.ParticipationMessage = GetParticipationMessage(survey, userResponses.FirstOrDefault());
                }
            }

            return ApplicationResult<SurveyDto>.Success(surveyDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting survey by ID {SurveyId}", request.SurveyId);
            return ApplicationResult<SurveyDto>.Failure("خطا در دریافت نظرسنجی");
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
            AudienceFilter = survey.AudienceFilter?.FilterExpression
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

        if (survey.StartAt.HasValue && DateTime.UtcNow < survey.StartAt.Value)
            return "نظرسنجی هنوز شروع نشده است";

        if (survey.EndAt.HasValue && DateTime.UtcNow > survey.EndAt.Value)
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
