using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;
using Nezam.Refahi.Surveying.Contracts.Dtos;
using Nezam.Refahi.Surveying.Contracts.Queries;
using Nezam.Refahi.Surveying.Domain.Entities;
using Nezam.Refahi.Surveying.Domain.Enums;
using Nezam.Refahi.Surveying.Domain.Repositories;
using Nezam.Refahi.Surveying.Domain.ValueObjects;

namespace Nezam.Refahi.Surveying.Application.Queries;

/// <summary>
/// Handler for GetSurveyQuestionsDetailsQuery
/// </summary>
public class GetSurveyQuestionsDetailsQueryHandler : IRequestHandler<GetSurveyQuestionsDetailsQuery, ApplicationResult<SurveyQuestionsDetailsResponse>>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ILogger<GetSurveyQuestionsDetailsQueryHandler> _logger;
    private readonly IMemberInfoService _memberInfoService;

    public GetSurveyQuestionsDetailsQueryHandler(
        ISurveyRepository surveyRepository,
        ILogger<GetSurveyQuestionsDetailsQueryHandler> logger,
        IMemberInfoService memberInfoService)
    {
        _surveyRepository = surveyRepository;
        _logger = logger;
        _memberInfoService = memberInfoService;
    }

    public async Task<ApplicationResult<SurveyQuestionsDetailsResponse>> Handle(GetSurveyQuestionsDetailsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Get survey with all data
            var survey = await _surveyRepository.GetWithAllDataAsync(request.SurveyId, cancellationToken);
            if (survey == null)
                return ApplicationResult<SurveyQuestionsDetailsResponse>.Failure("نظرسنجی یافت نشد");

            var response = new SurveyQuestionsDetailsResponse
            {
                SurveyId = survey.Id,
                SurveyTitle = survey.Title,
                SurveyDescription = string.Empty,
                SurveyState = MapSurveyStateToDto(survey),
                Questions = new List<QuestionDetailsDto>(),
                TotalQuestions = survey.Questions.Count,
                RequiredQuestions = survey.Questions.Count(q => q.IsRequired)
            };

            // Get user's response if UserNationalNumber is provided
            Response? userResponse = null;
            Guid? memberId = null;
            if (!string.IsNullOrWhiteSpace(request.UserNationalNumber) && request.IncludeUserAnswers)
            {
                // Get member info from national number
                var nationalId = new NationalId(request.UserNationalNumber);
                var memberInfo = await _memberInfoService.GetMemberInfoAsync(nationalId);
                
                if (memberInfo != null)
                {
                    memberId = memberInfo.Id;
                    // Load survey with responses to get user's response
                    var surveyWithResponses = await _surveyRepository.GetWithResponsesAsync(request.SurveyId, cancellationToken);
                    if (surveyWithResponses != null)
                    {
                        var participant = ParticipantInfo.ForMember(memberInfo.Id);
                        // Get the latest valid response (submitted responses only)
                        userResponse = surveyWithResponses.GetLatestValidResponse(participant);
                    }
                }
            }

            // Map questions with user answers
            var questionsWithDetails = new List<QuestionDetailsDto>();
            foreach (var question in survey.Questions.OrderBy(q => q.Order))
            {
                var questionDto = MapQuestionToDto(question);
                
                // Add user answer if available
                if (userResponse != null)
                {
                    var userAnswer = userResponse.GetQuestionAnswer(question.Id, 1); // Default to repeat index 1
                    
                    if (userAnswer != null)
                    {
                        questionDto.LatestUserAnswer = MapUserAnswerToDto(userAnswer, question, userResponse.AttemptNumber);
                        questionDto.IsAnswered = true;
                    }
                }
                
                questionsWithDetails.Add(questionDto);
            }

            response.Questions = questionsWithDetails;

            // Add user answer status
            if (userResponse != null)
            {
                response.UserAnswerStatus = MapUserAnswerStatusToDto(userResponse, survey, cancellationToken);
            }

            // Add statistics if requested
            if (request.IncludeStatistics)
            {
                response.Statistics = await MapSurveyStatisticsToDto(survey, cancellationToken);
            }

            return ApplicationResult<SurveyQuestionsDetailsResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting survey questions details for SurveyId {SurveyId}", request.SurveyId);
            return ApplicationResult<SurveyQuestionsDetailsResponse>.Failure("خطا در دریافت جزئیات سوالات نظرسنجی");
        }
    }

    private static SurveyStateInfoDto MapSurveyStateToDto(Survey survey)
    {
        return new SurveyStateInfoDto
        {
            State = survey.State.ToString(),
            StateText = GetSurveyStateText(survey.State),
            IsActive = survey.State == SurveyState.Active,
            IsAcceptingResponses = survey.State == SurveyState.Active,
            StartAt = survey.StartAt?.DateTime,
            EndAt = survey.EndAt?.DateTime,
            IsAnonymous = survey.IsAnonymous,
            ParticipationPolicy = new ParticipationPolicyDto
            {
                MaxAttemptsPerMember = survey.ParticipationPolicy.MaxAttemptsPerMember,
                AllowMultipleSubmissions = survey.ParticipationPolicy.AllowMultipleSubmissions,
                CoolDownSeconds = survey.ParticipationPolicy.CoolDownSeconds
            }
        };
    }

    private static QuestionDetailsDto MapQuestionToDto(Question question)
    {
        return new QuestionDetailsDto
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
                .Select(o => MapOptionToDto(o))
                .ToList()
        };
    }

    private static Contracts.Dtos.QuestionOptionDto MapOptionToDto(QuestionOption? option)
    {
        if (option == null)
            throw new ArgumentNullException(nameof(option));
            
        return new Contracts.Dtos.QuestionOptionDto
        {
            Id = option.Id,
            QuestionId = option.QuestionId,
            Text = option.Text,
            Order = option.Order,
            IsActive = option.IsActive
        };
    }

    private static QuestionAnswerDetailsDto MapUserAnswerToDto(Domain.Entities.QuestionAnswer questionAnswer, Question question, int attemptNumber)
    {
        var selectedOptions = questionAnswer.SelectedOptions
            .Select(so => question.Options.FirstOrDefault(o => o.Id == so.OptionId))
            .Where(o => o != null)
            .Select(MapOptionToDto)
            .ToList();

        return new QuestionAnswerDetailsDto
        {
            Id = questionAnswer.Id,
            QuestionId = questionAnswer.QuestionId,
            ResponseId = questionAnswer.ResponseId,
            TextAnswer = questionAnswer.TextAnswer,
            SelectedOptionIds = questionAnswer.SelectedOptions.Select(so => so.OptionId).ToList(),
            SelectedOptions = selectedOptions,
            IsAnswered = questionAnswer.HasAnswer(),
            IsComplete = questionAnswer.HasAnswer()
        };
    }

    private static UserAnswerStatusDto MapUserAnswerStatusToDto(Response userResponse, Survey survey, CancellationToken cancellationToken)
    {
        var questionAnswers = userResponse.QuestionAnswers.ToList();
        var answeredQuestions = questionAnswers.Count(qa => qa.HasAnswer());
        var totalQuestions = survey.Questions.Count;
        var completionPercentage = totalQuestions > 0 ? (decimal)answeredQuestions / totalQuestions * 100 : 0;

        return new UserAnswerStatusDto
        {
            ResponseId = userResponse.Id,
            AttemptNumber = userResponse.AttemptNumber,
            LastAnsweredAt = DateTime.MinValue, // QuestionAnswer doesn't have CreatedAt
            AnsweredQuestions = answeredQuestions,
            TotalQuestions = totalQuestions,
            CompletionPercentage = completionPercentage,
            IsComplete = answeredQuestions == totalQuestions,
            CanContinue = CanUserContinueSurvey(survey, userResponse),
            StatusMessage = GetUserStatusMessage(survey, userResponse, answeredQuestions, totalQuestions)
        };
    }

    private Task<SurveyStatisticsDto> MapSurveyStatisticsToDto(Survey survey, CancellationToken cancellationToken)
    {
        var responses = survey.Responses.ToList();

        return Task.FromResult(new SurveyStatisticsDto
        {
            TotalQuestions = survey.Questions.Count,
            RequiredQuestions = survey.Questions.Count(q => q.IsRequired),
            ResponseCount = responses.Count,
            UniqueParticipantCount = responses.Select(r => r.Participant.MemberId).Distinct().Count(),
            SubmittedResponseCount = responses.Count(r => r.SubmittedAt.HasValue),
            ActiveResponseCount = responses.Count(r => r.AttemptStatus == AttemptStatus.Active),
            CanceledResponseCount = responses.Count(r => r.AttemptStatus == AttemptStatus.Canceled),
            ExpiredResponseCount = responses.Count(r => r.AttemptStatus == AttemptStatus.Expired),
            AverageCompletionPercentage = responses.Any() ? 
                responses.Average(r => (decimal)r.QuestionAnswers.Count(qa => qa.HasAnswer()) / survey.Questions.Count * 100) : 0,
            AverageResponseTime = 0, // Not available without CreatedAt
            IsAcceptingResponses = survey.State == SurveyState.Active,
            IsAcceptingResponsesText = survey.State == SurveyState.Active ? "در حال پذیرش پاسخ" : "پذیرش پاسخ متوقف شده"
        });
    }

    private static bool CanUserContinueSurvey(Survey survey, Response userResponse)
    {
        if (survey.State != SurveyState.Active)
            return false;

        if (survey.StartAt.HasValue && DateTimeOffset.UtcNow < survey.StartAt.Value)
            return false;

        if (survey.EndAt.HasValue && DateTimeOffset.UtcNow > survey.EndAt.Value)
            return false;

        if (!survey.ParticipationPolicy.AllowMultipleSubmissions)
            return false;

        if (userResponse.AttemptNumber >= survey.ParticipationPolicy.MaxAttemptsPerMember)
            return false;

        if (survey.ParticipationPolicy.CoolDownSeconds.HasValue)
        {
            var cooldownEnd = userResponse.SubmittedAt?.AddSeconds(survey.ParticipationPolicy.CoolDownSeconds.Value);
            if (cooldownEnd.HasValue && DateTimeOffset.UtcNow < cooldownEnd.Value)
                return false;
        }

        return true;
    }

    private static string GetUserStatusMessage(Survey survey, Response userResponse, int answeredQuestions, int totalQuestions)
    {
        if (answeredQuestions == totalQuestions)
            return "نظرسنجی تکمیل شده است";

        if (answeredQuestions == 0)
            return "هنوز شروع نشده است";

        return $"در حال تکمیل ({answeredQuestions}/{totalQuestions})";
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
