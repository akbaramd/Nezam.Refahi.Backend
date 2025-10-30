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
/// Handler for GetSpecificQuestionQuery - gets a specific question by index with navigation capabilities
/// </summary>
public class GetSpecificQuestionQueryHandler : IRequestHandler<GetSpecificQuestionQuery, ApplicationResult<GetSpecificQuestionResponse>>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ILogger<GetSpecificQuestionQueryHandler> _logger;
    private readonly IMemberInfoService _memberInfoService;

    public GetSpecificQuestionQueryHandler(
        ISurveyRepository surveyRepository,
        ILogger<GetSpecificQuestionQueryHandler> logger,
        IMemberInfoService memberInfoService)
    {
        _surveyRepository = surveyRepository ?? throw new ArgumentNullException(nameof(surveyRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _memberInfoService = memberInfoService ?? throw new ArgumentNullException(nameof(memberInfoService));
    }

    public async Task<ApplicationResult<GetSpecificQuestionResponse>> Handle(
        GetSpecificQuestionQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting specific question {QuestionIndex} for survey {SurveyId}", 
                request.QuestionIndex, request.SurveyId);

            // Get survey with all data
            var survey = await _surveyRepository.GetWithAllDataAsync(request.SurveyId, cancellationToken);
            if (survey == null)
            {
                _logger.LogWarning("Survey {SurveyId} not found", request.SurveyId);
                return ApplicationResult<GetSpecificQuestionResponse>.Failure("نظرسنجی یافت نشد");
            }

            // Validate question index
            if (request.QuestionIndex < 0 || request.QuestionIndex >= survey.Questions.Count)
            {
                _logger.LogWarning("Invalid question index {QuestionIndex} for survey {SurveyId} with {TotalQuestions} questions", 
                    request.QuestionIndex, request.SurveyId, survey.Questions.Count);
                return ApplicationResult<GetSpecificQuestionResponse>.Failure("شماره سوال نامعتبر است");
            }

            // Get the specific question
            var orderedQuestions = survey.Questions.OrderBy(q => q.Order).ToList();
            var currentQuestion = orderedQuestions[request.QuestionIndex];

            // Build response
            var response = new GetSpecificQuestionResponse
            {
                Survey = MapSurveyToBasicInfo(survey),
                CurrentQuestion = MapQuestionToDetailsDto(currentQuestion),
                Navigation = CalculateNavigation(orderedQuestions, request.QuestionIndex),
                UserAnswer = null,
                UserResponseStatus = null,
                Statistics = null
            };

            // Get user-specific data if requested
            if (!string.IsNullOrWhiteSpace(request.UserNationalNumber))
            {
                var nationalId = new NationalId(request.UserNationalNumber);
                var memberInfo = await _memberInfoService.GetMemberInfoAsync(nationalId);
                
                if (memberInfo != null)
                {
                    _logger.LogInformation("Found member {MemberId} for national number {UserNationalNumber}", 
                        memberInfo.Id, request.UserNationalNumber);

                    // Get user's response
                    Response? userResponse = null;
                    if (request.ResponseId.HasValue)
                    {
                        // Get specific response
                        userResponse = survey.Responses.FirstOrDefault(r => r.Id == request.ResponseId.Value);
                    }
                    else
                    {
                        // Get latest response for this member
                        userResponse = survey.Responses
                            .Where(r => r.Participant.MemberId == memberInfo.Id)
                            .OrderByDescending(r => r.AttemptNumber)
                            .FirstOrDefault();
                    }

                    if (userResponse != null && request.IncludeUserAnswers)
                    {
                        // Get user's answer for current question
                        var questionAnswer = userResponse.QuestionAnswers
                            .FirstOrDefault(qa => qa.QuestionId == currentQuestion.Id);
                        
                        if (questionAnswer != null)
                        {
                            response.UserAnswer = MapQuestionAnswerToDto(questionAnswer, currentQuestion);
                        }

                        // Build user response status
                        response.UserResponseStatus = MapUserResponseStatusToDto(userResponse, survey);
                    }
                }
                else
                {
                    _logger.LogWarning("Member not found for national number {UserNationalNumber}", request.UserNationalNumber);
                }
            }

            // Include statistics if requested
            if (request.IncludeStatistics)
            {
                response.Statistics = await CalculateQuestionStatistics(survey, currentQuestion.Id, cancellationToken);
            }

            _logger.LogInformation("Successfully retrieved question {QuestionIndex} for survey {SurveyId}", 
                request.QuestionIndex, request.SurveyId);

            return ApplicationResult<GetSpecificQuestionResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting specific question {QuestionIndex} for survey {SurveyId}", 
                request.QuestionIndex, request.SurveyId);
            return ApplicationResult<GetSpecificQuestionResponse>.Failure("خطا در دریافت سوال");
        }
    }

    private static SurveyBasicInfoDto MapSurveyToBasicInfo(Survey survey)
    {
        return new SurveyBasicInfoDto
        {
            Id = survey.Id,
            Title = survey.Title,
            State = survey.State.ToString(),
            StateText = GetSurveyStateText(survey.State),
            IsActive = survey.State == SurveyState.Active,
            TotalQuestions = survey.Questions.Count,
            RequiredQuestions = survey.Questions.Count(q => q.IsRequired),
            IsAnonymous = survey.IsAnonymous,
            StartAt = survey.StartAt?.DateTime,
            EndAt = survey.EndAt?.DateTime
        };
    }

    private static QuestionDetailsDto MapQuestionToDetailsDto(Question question)
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
            IsRequiredText = question.IsRequired ? "الزامی" : "اختیاری",
            Options = question.Options
                .Where(o => o.IsActive)
                .OrderBy(o => o.Order)
                .Select(MapOptionToDto)
                .ToList()
        };
    }

    private static QuestionOptionDto MapOptionToDto(QuestionOption option)
    {
        return new QuestionOptionDto
        {
            Id = option.Id,
            QuestionId = option.QuestionId,
            Text = option.Text,
            Order = option.Order,
            IsActive = option.IsActive
        };
    }

    private static QuestionAnswerDto MapQuestionAnswerToDto(Domain.Entities.QuestionAnswer questionAnswer, Question question)
    {
        return new QuestionAnswerDto
        {
            Id = questionAnswer.Id,
            QuestionId = questionAnswer.QuestionId,
            TextAnswer = questionAnswer.TextAnswer,
            SelectedOptions = questionAnswer.SelectedOptions.Select(so => new QuestionAnswerOptionDto
            {
                OptionId = so.OptionId,
                OptionText = so.OptionText
            }).ToList()
        };
    }

    private static UserResponseStatusDto MapUserResponseStatusToDto(Response userResponse, Survey survey)
    {
        var answeredQuestions = userResponse.QuestionAnswers.Count(qa => qa.HasAnswer());
        var totalQuestions = survey.Questions.Count;
        var completionPercentage = totalQuestions > 0 ? (int)((decimal)answeredQuestions / totalQuestions * 100) : 0;

        return new UserResponseStatusDto
        {
            ResponseId = userResponse.Id,
            AttemptNumber = userResponse.AttemptNumber,
            AttemptStatus = userResponse.AttemptStatus.ToString(),
            AttemptStatusTextFa = GetAttemptStatusText(userResponse.AttemptStatus),
            ResponseStatus = userResponse.Status.ToString(),
            ResponseStatusTextFa = ResponseStatusHelper.GetPersianText(userResponse.Status),
            StartedAt = null, // Response doesn't have StartedAt property
            StartedAtLocal = null,
            SubmittedAt = userResponse.SubmittedAt,
            SubmittedAtLocal = userResponse.SubmittedAt,
            QuestionsAnswered = answeredQuestions,
            QuestionsTotal = totalQuestions,
            CompletionPercentage = completionPercentage,
            IsActive = userResponse.AttemptStatus == AttemptStatus.Active,
            IsSubmitted = userResponse.AttemptStatus == AttemptStatus.Submitted,
            CanContinue = userResponse.AttemptStatus == AttemptStatus.Active,
            NextActionText = GetNextActionText(userResponse, survey)
        };
    }

    private static QuestionNavigationDto CalculateNavigation(List<Question> orderedQuestions, int currentIndex)
    {
        return new QuestionNavigationDto
        {
            PreviousQuestionId = currentIndex > 0 ? orderedQuestions[currentIndex - 1].Id : null,
            NextQuestionId = currentIndex < orderedQuestions.Count - 1 ? orderedQuestions[currentIndex + 1].Id : null,
            CanGoBack = currentIndex > 0,
            CanGoForward = currentIndex < orderedQuestions.Count - 1,
            CanSkip = !orderedQuestions[currentIndex].IsRequired,
            IsRequired = orderedQuestions[currentIndex].IsRequired
        };
    }

    private Task<QuestionStatisticsDto?> CalculateQuestionStatistics(Survey survey, Guid questionId, CancellationToken cancellationToken)
    {
        try
        {
            // Get all responses for this survey
            var allResponses = survey.Responses.Where(r => r.AttemptStatus == AttemptStatus.Submitted).ToList();
            
            if (!allResponses.Any())
            {
                return Task.FromResult<QuestionStatisticsDto?>(new QuestionStatisticsDto
                {
                    TotalAnswers = 0,
                    RequiredAnswers = 0,
                    OptionalAnswers = 0,
                    AnswerRate = 0,
                    CompletionRate = 0,
                    OptionSelectionCounts = new Dictionary<string, int>(),
                    CommonTextAnswers = new List<string>()
                });
            }

            var questionAnswers = allResponses
                .SelectMany(r => r.QuestionAnswers)
                .Where(qa => qa.QuestionId == questionId)
                .ToList();

            var answeredCount = questionAnswers.Count(qa => qa.HasAnswer());
            var skippedCount = allResponses.Count - answeredCount;
            var completionRate = allResponses.Count > 0 ? (decimal)answeredCount / allResponses.Count * 100 : 0;

            // Calculate answer distribution for multiple choice questions
            var optionSelectionCounts = new Dictionary<string, int>();
            var commonTextAnswers = new List<string>();
            
            foreach (var answer in questionAnswers.Where(qa => qa.HasAnswer()))
            {
                if (answer.SelectedOptions.Any())
                {
                    foreach (var option in answer.SelectedOptions)
                    {
                        var key = option.OptionText;
                        optionSelectionCounts[key] = optionSelectionCounts.GetValueOrDefault(key, 0) + 1;
                    }
                }
                else if (!string.IsNullOrWhiteSpace(answer.TextAnswer))
                {
                    // For text answers, collect them for analysis
                    commonTextAnswers.Add(answer.TextAnswer);
                }
            }

            return Task.FromResult<QuestionStatisticsDto?>(new QuestionStatisticsDto
            {
                TotalAnswers = allResponses.Count,
                RequiredAnswers = answeredCount,
                OptionalAnswers = skippedCount,
                AnswerRate = completionRate,
                CompletionRate = completionRate,
                OptionSelectionCounts = optionSelectionCounts,
                CommonTextAnswers = commonTextAnswers.Take(10).ToList() // Limit to top 10 text answers
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating statistics for question {QuestionId}", questionId);
            return Task.FromResult<QuestionStatisticsDto?>(null);
        }
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

    private static string GetAttemptStatusText(AttemptStatus status)
    {
        return status switch
        {
            AttemptStatus.Active => "فعال",
            AttemptStatus.Submitted => "ارسال شده",
            AttemptStatus.Canceled => "لغو شده",
            AttemptStatus.Expired => "منقضی شده",
            _ => "نامشخص"
        };
    }

    private static string GetNextActionText(Response response, Survey survey)
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

    private static bool IsResponseComplete(Survey survey, Response response)
    {
        // Check if all required questions are answered
        var requiredQuestions = survey.Questions.Where(q => q.IsRequired).ToList();
        return requiredQuestions.All(q => response.HasAnswerForQuestion(q.Id));
    }
}
