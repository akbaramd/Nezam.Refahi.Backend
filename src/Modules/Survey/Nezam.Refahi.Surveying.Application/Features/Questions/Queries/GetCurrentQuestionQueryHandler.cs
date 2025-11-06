using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Surveying.Application.Services;
using Nezam.Refahi.Surveying.Contracts.Dtos;
using Nezam.Refahi.Surveying.Contracts.Queries;
using Nezam.Refahi.Surveying.Domain.Entities;
using Nezam.Refahi.Surveying.Domain.Enums;
using Nezam.Refahi.Surveying.Domain.Repositories;

namespace Nezam.Refahi.Surveying.Application.Features.Questions.Queries;

/// <summary>
/// Handler for GetCurrentQuestionQuery
/// </summary>
public class GetCurrentQuestionQueryHandler : IRequestHandler<GetCurrentQuestionQuery, ApplicationResult<CurrentQuestionResponse>>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ILogger<GetCurrentQuestionQueryHandler> _logger;

    public GetCurrentQuestionQueryHandler(
        ISurveyRepository surveyRepository,
        ILogger<GetCurrentQuestionQueryHandler> logger)
    {
        _surveyRepository = surveyRepository;
        _logger = logger;
    }

    public async Task<ApplicationResult<CurrentQuestionResponse>> Handle(
        GetCurrentQuestionQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting current question for response {ResponseId} in survey {SurveyId}", 
                request.ResponseId, request.SurveyId);

            // Get survey with responses
            var survey = await _surveyRepository.GetSurveyWithResponseAndQuestionsAsync(request.ResponseId, cancellationToken);
            if (survey == null)
            {
                _logger.LogWarning("Survey with response {ResponseId} not found", request.ResponseId);
                return ApplicationResult<CurrentQuestionResponse>.Failure("Survey or response not found");
            }

            // Get response from survey aggregate
            var response = survey.Responses.FirstOrDefault(r => r.Id == request.ResponseId);
            if (response == null)
            {
                _logger.LogWarning("Response {ResponseId} not found in survey", request.ResponseId);
                return ApplicationResult<CurrentQuestionResponse>.Failure("Response not found");
            }

            // Verify response belongs to survey
            if (response.SurveyId != request.SurveyId)
            {
                _logger.LogWarning("Response {ResponseId} does not belong to survey {SurveyId}", 
                    request.ResponseId, request.SurveyId);
                return ApplicationResult<CurrentQuestionResponse>.Failure("Response does not belong to survey");
            }

            // Find current question (first unanswered question)
            var orderedQuestions = survey.GetOrderedQuestions().ToList();
            var currentQuestion = FindCurrentQuestion(orderedQuestions, response);

            if (currentQuestion == null)
            {
                _logger.LogInformation("No current question found for response {ResponseId}", request.ResponseId);
                return ApplicationResult<CurrentQuestionResponse>.Failure("No current question found");
            }

            // Determine repeat index
            var repeatIndex = request.RepeatIndex ?? 1;
            
            // Validate repeat index for repeatable questions
            if (!currentQuestion.ValidateRepeatIndex(repeatIndex))
            {
                _logger.LogWarning("Invalid repeat index {RepeatIndex} for question {QuestionId}", 
                    repeatIndex, currentQuestion.Id);
                return ApplicationResult<CurrentQuestionResponse>.Failure($"Invalid repeat index {repeatIndex} for this question");
            }

            var result = await MapToResponse(survey, response, currentQuestion, orderedQuestions, repeatIndex);
            
            _logger.LogInformation("Successfully retrieved current question {QuestionId} for response {ResponseId}", 
                currentQuestion.Id, request.ResponseId);
            return ApplicationResult<CurrentQuestionResponse>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting current question for response {ResponseId} in survey {SurveyId}", 
                request.ResponseId, request.SurveyId);
            return ApplicationResult<CurrentQuestionResponse>.Failure(ex, "An error occurred while retrieving current question");
        }
    }

    private static Question? FindCurrentQuestion(List<Question> orderedQuestions, Response response)
    {
        var answeredQuestionIds = response.GetAnsweredQuestionIds().ToHashSet();
        
        // If response is submitted, return the first question to show completion status
        if (response.SubmittedAt.HasValue)
        {
            return orderedQuestions.FirstOrDefault();
        }
        
        // Find first unanswered question for active responses
        return orderedQuestions.FirstOrDefault(q => !answeredQuestionIds.Contains(q.Id));
    }

    private Task<CurrentQuestionResponse> MapToResponse(
        Survey survey, 
        Response response, 
        Question question, 
        List<Question> orderedQuestions,
        int repeatIndex)
    {
        // Get answer for specific repeat index
        var questionAnswer = response.GetQuestionAnswer(question.Id, repeatIndex);
        var currentIndex = orderedQuestions.IndexOf(question);

        // Calculate repeatable question statistics
        var answeredRepeats = response.GetAnsweredRepeatCount(question.Id);
        var maxRepeats = question.GetMaxRepeatIndex();
        var canAddMoreRepeats = question.CanAddMoreRepeats(answeredRepeats);
        var isLastRepeat = maxRepeats.HasValue && repeatIndex >= maxRepeats.Value;

        return Task.FromResult(new CurrentQuestionResponse
        {
            QuestionId = question.Id,
            QuestionText = question.Text,
            QuestionKind = question.Kind.ToString(),
            Order = question.Order,
            IsRequired = question.IsRequired,
            IsAnswered = questionAnswer?.HasAnswer() ?? false,
            IsComplete = response.SubmittedAt.HasValue || (questionAnswer?.HasAnswer() ?? false),
            
            // Repeatable question info
            RepeatPolicy = new RepeatPolicyDto
            {
                Kind = question.RepeatPolicy.Kind.ToString(),
                MaxRepeats = question.RepeatPolicy.MaxRepeats
            },
            RepeatIndex = repeatIndex,
            IsRepeatAnswered = questionAnswer?.HasAnswer() ?? false,
            IsLastRepeat = isLastRepeat,
            AnsweredRepeats = answeredRepeats,
            MaxRepeats = maxRepeats,
            CanAddMoreRepeats = canAddMoreRepeats,
            
            Options = question.Options.Select(o => new Contracts.Dtos.QuestionOptionDto
            {
                Id = o.Id,
                QuestionId = o.QuestionId,
                Text = o.Text,
                Order = o.Order,
                IsActive = true,
                IsSelected = questionAnswer?.SelectedOptions.Any(so => so.OptionId == o.Id) ?? false
            }).ToList(),
            
            TextAnswer = questionAnswer?.TextAnswer,
            SelectedOptionIds = questionAnswer?.SelectedOptions.Select(so => so.OptionId).ToList() ?? new List<Guid>(),
            
            IsFirstQuestion = currentIndex == 0,
            IsLastQuestion = currentIndex == orderedQuestions.Count - 1,
            CurrentQuestionNumber = currentIndex + 1,
            TotalQuestions = orderedQuestions.Count,
            ProgressPercentage = orderedQuestions.Count > 0 ? (double)(currentIndex + 1) / orderedQuestions.Count * 100 : 0,
            
            ResponseId = response.Id,
            AttemptNumber = response.AttemptNumber,
            AllowBackNavigation = survey.ParticipationPolicy.AllowBackNavigation
        });
    }
}
