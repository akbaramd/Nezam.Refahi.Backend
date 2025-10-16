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
using Nezam.Refahi.Surveying.Domain.Services;
using Nezam.Refahi.Surveying.Domain.ValueObjects;

namespace Nezam.Refahi.Surveying.Application.Queries;

/// <summary>
/// Handler for getting user's responses to a specific survey
/// </summary>
public class GetUserSurveyResponsesQueryHandler : IRequestHandler<GetUserSurveyResponsesQuery, ApplicationResult<UserSurveyResponsesResponse>>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ILogger<GetUserSurveyResponsesQueryHandler> _logger;
    private readonly IMemberInfoService _memberInfoService;

    public GetUserSurveyResponsesQueryHandler(
        ISurveyRepository surveyRepository,
        ILogger<GetUserSurveyResponsesQueryHandler> logger,
        IMemberInfoService memberInfoService)
    {
        _surveyRepository = surveyRepository;
        _logger = logger;
        _memberInfoService = memberInfoService;
    }

    public async Task<ApplicationResult<UserSurveyResponsesResponse>> Handle(
        GetUserSurveyResponsesQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            // Get member info using national number
            if (string.IsNullOrWhiteSpace(request.NationalNumber))
            {
                return ApplicationResult<UserSurveyResponsesResponse>.Failure("شماره ملی الزامی است");
            }

            var nationalId = new NationalId(request.NationalNumber);
            var memberInfo = await _memberInfoService.GetMemberInfoAsync(nationalId);
            if (memberInfo == null)
            {
                return ApplicationResult<UserSurveyResponsesResponse>.Failure("عضو یافت نشد");
            }

            // Get survey with responses to get user's responses
            var survey = await _surveyRepository.GetWithResponsesAsync(request.SurveyId, cancellationToken);
            if (survey == null)
            {
                return ApplicationResult<UserSurveyResponsesResponse>.Failure("نظرسنجی یافت نشد");
            }

            // Get user's responses from the survey aggregate using proper MemberId
            var responses = survey.Responses
                .Where(r => r.Participant.MemberId == memberInfo.Id)
                .ToList();

            // Map responses to DTOs
            var responseDtos = responses.Select(r => MapToResponseDto(r, survey, request.IncludeAnswers, request.IncludeLastAnswersOnly)).ToList();

            // Get latest response
            var latestResponse = responses.OrderByDescending(r => r.SubmittedAt ?? DateTimeOffset.MinValue).FirstOrDefault();
            var latestResponseDto = latestResponse != null ? MapToResponseDto(latestResponse, survey, request.IncludeAnswers, request.IncludeLastAnswersOnly) : null;

            // Calculate summary
            var summary = CalculateResponseSummary(responses);

            // Check if user can start new attempt
            var participant = ParticipantInfo.ForMember(memberInfo.Id);
            var canStartNewAttempt = CanUserStartNewAttempt(survey, participant, responses);

            var response = new UserSurveyResponsesResponse
            {
                SurveyId = survey.Id,
                SurveyTitle = survey.Title,
                SurveyDescription = survey.Description,
                Responses = responseDtos,
                TotalAttempts = responses.Count(),
                CompletedAttempts = summary.CompletedCount,
                ActiveAttempts = summary.ActiveCount,
                CanceledAttempts = summary.CanceledCount,
                ExpiredAttempts = summary.ExpiredCount,
                LatestResponse = latestResponseDto,
                HasActiveResponse = latestResponse?.AttemptStatus == AttemptStatus.Active,
                CanStartNewAttempt = canStartNewAttempt,
                NextActionText = GetNextActionText(latestResponse, survey, canStartNewAttempt),
                MaxAttemptsAllowed = survey.ParticipationPolicy.MaxAttemptsPerMember,
                RemainingAttempts = Math.Max(0, survey.ParticipationPolicy.MaxAttemptsPerMember - responses.Count()),
                IsSurveyActive = survey.IsAcceptingResponses(),
                SurveyStatusText = GetSurveyStatusText(survey)
            };

            return ApplicationResult<UserSurveyResponsesResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user survey responses for NationalNumber {NationalNumber} and survey {SurveyId}", 
                request.NationalNumber, request.SurveyId);
            return ApplicationResult<UserSurveyResponsesResponse>.Failure("خطا در دریافت پاسخ‌های کاربر");
        }
    }

    private ResponseDto MapToResponseDto(Response response, Survey survey, bool includeAnswers, bool includeLastAnswersOnly)
    {
        var dto = new ResponseDto
        {
            Id = response.Id,
            SurveyId = response.SurveyId,
            AttemptNumber = response.AttemptNumber,
            SubmittedAt = response.SubmittedAt?.DateTime,
            ExpiredAt = response.ExpiredAt?.DateTime,
            CanceledAt = response.CanceledAt?.DateTime,
            ParticipantDisplayName = response.Participant.GetDisplayName(),
            ParticipantShortIdentifier = response.Participant.GetShortIdentifier(),
            IsAnonymous = response.Participant.IsAnonymous,
            IsAnonymousText = response.Participant.IsAnonymous ? "ناشناس" : "شناخته شده",
            AttemptStatus = response.AttemptStatus.ToString(),
            AttemptStatusText = GetAttemptStatusText(response.AttemptStatus),
            IsActive = response.AttemptStatus == AttemptStatus.Active,
            IsSubmitted = response.AttemptStatus == AttemptStatus.Submitted,
            IsExpired = response.AttemptStatus == AttemptStatus.Expired,
            IsCanceled = response.AttemptStatus == AttemptStatus.Canceled,
            TotalQuestions = survey.Questions.Count,
            AnsweredQuestions = response.QuestionAnswers.Count(qa => qa.HasAnswer()),
            RequiredQuestions = survey.Questions.Count(q => q.IsRequired),
            RequiredAnsweredQuestions = survey.Questions.Count(q => q.IsRequired && response.HasAnswerForQuestion(q.Id)),
            IsComplete = IsResponseComplete(survey, response),
            IsCompleteText = IsResponseComplete(survey, response) ? "کامل" : "ناقص",
            CompletionPercentage = CalculateCompletionPercentage(response, survey),
            CompletionPercentageText = $"{CalculateCompletionPercentage(response, survey):F1}%",
            SurveyTitle = survey.Title,
            SurveyDescription = survey.Description,
            CanContinue = response.AttemptStatus == AttemptStatus.Active,
            CanSubmit = response.AttemptStatus == AttemptStatus.Active && IsResponseComplete(survey, response),
            CanCancel = response.AttemptStatus == AttemptStatus.Active,
            NextActionText = GetResponseNextActionText(response, survey)
        };

        // Include answers if requested
        if (includeAnswers)
        {
            var questionAnswers = response.QuestionAnswers.ToList();
            
            if (includeLastAnswersOnly)
            {
                // Only include the latest answer for each question
                questionAnswers = questionAnswers
                    .GroupBy(qa => qa.QuestionId)
                    .Select(g => g.OrderByDescending(qa => qa.RepeatIndex).First())
                    .ToList();
            }

            dto.QuestionAnswers = questionAnswers.Select(qa => new QuestionAnswerDto
            {
                Id = qa.Id,
                QuestionId = qa.QuestionId,
                TextAnswer = qa.TextAnswer,
                SelectedOptions = qa.SelectedOptions.Select(so => new QuestionAnswerOptionDto
                {
                    OptionId = so.OptionId,
                    OptionText = so.OptionText
                }).ToList()
            }).ToList();

            // Set last answers (latest answer for each question)
            dto.LastAnswers = response.QuestionAnswers
                .GroupBy(qa => qa.QuestionId)
                .Select(g => g.OrderByDescending(qa => qa.RepeatIndex).First())
                .Select(qa => new QuestionAnswerDto
                {
                    Id = qa.Id,
                    QuestionId = qa.QuestionId,
                    TextAnswer = qa.TextAnswer,
                    SelectedOptions = qa.SelectedOptions.Select(so => new QuestionAnswerOptionDto
                    {
                        OptionId = so.OptionId,
                        OptionText = so.OptionText
                    }).ToList()
                }).ToList();
        }

        return dto;
    }

    private (int CompletedCount, int ActiveCount, int CanceledCount, int ExpiredCount) CalculateResponseSummary(List<Response> responses)
    {
        var completedCount = responses.Count(r => r.AttemptStatus == AttemptStatus.Submitted);
        var activeCount = responses.Count(r => r.AttemptStatus == AttemptStatus.Active);
        var canceledCount = responses.Count(r => r.AttemptStatus == AttemptStatus.Canceled);
        var expiredCount = responses.Count(r => r.AttemptStatus == AttemptStatus.Expired);

        return (completedCount, activeCount, canceledCount, expiredCount);
    }

    private string GetAttemptStatusText(AttemptStatus status) => status switch
    {
        AttemptStatus.Active => "فعال",
        AttemptStatus.Submitted => "ارسال شده",
        AttemptStatus.Canceled => "لغو شده",
        AttemptStatus.Expired => "منقضی شده",
        _ => "نامشخص"
    };

    private string GetSurveyStatusText(Survey survey) => survey.State switch
    {
        SurveyState.Draft => "پیش‌نویس",
        SurveyState.Scheduled => "زمان‌بندی شده",
        SurveyState.Active => "فعال",
        SurveyState.Closed => "بسته شده",
        SurveyState.Archived => "آرشیو شده",
        _ => "نامشخص"
    };

    private string GetNextActionText(Response? latestResponse, Survey survey, bool canStartNewAttempt)
    {
        if (!survey.IsAcceptingResponses())
            return "نظرسنجی در حال حاضر فعال نیست";

        if (latestResponse == null)
            return "شروع نظرسنجی";

        return latestResponse.AttemptStatus switch
        {
            AttemptStatus.Active => IsResponseComplete(survey, latestResponse) ? "ارسال پاسخ" : "ادامه پاسخ‌دهی",
            AttemptStatus.Submitted => canStartNewAttempt ? "شروع تلاش جدید" : "حداکثر تلاش‌ها رسیده است",
            AttemptStatus.Canceled => canStartNewAttempt ? "شروع تلاش جدید" : "حداکثر تلاش‌ها رسیده است",
            AttemptStatus.Expired => canStartNewAttempt ? "شروع تلاش جدید" : "حداکثر تلاش‌ها رسیده است",
            _ => canStartNewAttempt ? "شروع نظرسنجی" : "حداکثر تلاش‌ها رسیده است"
        };
    }

    private string GetResponseNextActionText(Response response, Survey survey)
    {
        return response.AttemptStatus switch
        {
            AttemptStatus.Active => IsResponseComplete(survey, response) ? "ارسال پاسخ" : "ادامه پاسخ‌دهی",
            AttemptStatus.Submitted => "پاسخ ارسال شده است",
            AttemptStatus.Canceled => "پاسخ لغو شده است",
            AttemptStatus.Expired => "پاسخ منقضی شده است",
            _ => "نامشخص"
        };
    }

    private decimal CalculateCompletionPercentage(Response response, Survey survey)
    {
        if (survey.Questions.Count == 0) return 100;
        var answeredQuestions = response.QuestionAnswers.Count(qa => qa.HasAnswer());
        return Math.Round((decimal)answeredQuestions / survey.Questions.Count * 100, 1);
    }

    private static bool IsResponseComplete(Survey survey, Response response)
    {
        // Check if all required questions are answered
        var requiredQuestions = survey.Questions.Where(q => q.IsRequired).ToList();
        return requiredQuestions.All(q => response.HasAnswerForQuestion(q.Id));
    }

    private static bool CanUserStartNewAttempt(Survey survey, ParticipantInfo participant, List<Response> responses)
    {
        if (!survey.IsAcceptingResponses())
            return false;

        if (responses.Count >= survey.ParticipationPolicy.MaxAttemptsPerMember)
            return false;

        var lastResponse = responses.OrderByDescending(r => r.SubmittedAt ?? DateTimeOffset.MinValue).FirstOrDefault();
        if (lastResponse != null && survey.ParticipationPolicy.CoolDownSeconds.HasValue)
        {
            var timeSinceLastResponse = DateTimeOffset.UtcNow - (lastResponse.SubmittedAt ?? DateTimeOffset.MinValue);
            if (timeSinceLastResponse.TotalSeconds < survey.ParticipationPolicy.CoolDownSeconds.Value)
                return false;
        }

        return true;
    }
}