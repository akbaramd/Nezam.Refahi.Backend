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
/// Handler for GetQuestionByIdQuery
/// </summary>
public class GetQuestionByIdQueryHandler : IRequestHandler<GetQuestionByIdQuery, ApplicationResult<QuestionByIdResponse>>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ILogger<GetQuestionByIdQueryHandler> _logger;

    public GetQuestionByIdQueryHandler(
        ISurveyRepository surveyRepository,
        ILogger<GetQuestionByIdQueryHandler> logger)
    {
        _surveyRepository = surveyRepository;
        _logger = logger;
    }

    public async Task<ApplicationResult<QuestionByIdResponse>> Handle(
        GetQuestionByIdQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting question {QuestionId} for response {ResponseId} in survey {SurveyId}", 
                request.QuestionId, request.ResponseId, request.SurveyId);

            // Get survey with response and questions
            var survey = await _surveyRepository.GetSurveyWithResponseAndQuestionsAsync(request.ResponseId, cancellationToken);
            if (survey == null)
            {
                _logger.LogWarning("Survey with response {ResponseId} not found", request.ResponseId);
                return ApplicationResult<QuestionByIdResponse>.Failure("Survey or response not found");
            }

            // Get response from survey aggregate
            var response = survey.Responses.FirstOrDefault(r => r.Id == request.ResponseId);
            if (response == null)
            {
                _logger.LogWarning("Response {ResponseId} not found in survey", request.ResponseId);
                return ApplicationResult<QuestionByIdResponse>.Failure("Response not found");
            }

            // Verify response belongs to survey
            if (response.SurveyId != request.SurveyId)
            {
                _logger.LogWarning("Response {ResponseId} does not belong to survey {SurveyId}", 
                    request.ResponseId, request.SurveyId);
                return ApplicationResult<QuestionByIdResponse>.Failure("Response does not belong to survey");
            }

            // Find specific question
            var question = survey.Questions.FirstOrDefault(q => q.Id == request.QuestionId);
            if (question == null)
            {
                _logger.LogWarning("Question {QuestionId} not found in survey {SurveyId}", 
                    request.QuestionId, request.SurveyId);
                return ApplicationResult<QuestionByIdResponse>.Failure("Question not found");
            }

            // Determine repeat index
            var repeatIndex = request.RepeatIndex ?? 1;
            
            // Validate repeat index for repeatable questions
            if (!question.ValidateRepeatIndex(repeatIndex))
            {
                _logger.LogWarning("Invalid repeat index {RepeatIndex} for question {QuestionId}", 
                    repeatIndex, request.QuestionId);
                return ApplicationResult<QuestionByIdResponse>.Failure($"Invalid repeat index {repeatIndex} for this question");
            }

            var orderedQuestions = survey.GetOrderedQuestions().ToList();
            var result = await MapToResponse(survey, response, question, orderedQuestions, repeatIndex);
            
            _logger.LogInformation("Successfully retrieved question {QuestionId} for response {ResponseId}", 
                request.QuestionId, request.ResponseId);
            return ApplicationResult<QuestionByIdResponse>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting question {QuestionId} for response {ResponseId} in survey {SurveyId}", 
                request.QuestionId, request.ResponseId, request.SurveyId);
            return ApplicationResult<QuestionByIdResponse>.Failure(ex, "An error occurred while retrieving question");
        }
    }

    private Task<QuestionByIdResponse> MapToResponse(
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

        return Task.FromResult(new QuestionByIdResponse
        {
            QuestionId = question.Id,
            QuestionText = question.Text,
            QuestionKind = question.Kind.ToString(),
            Order = question.Order,
            IsRequired = question.IsRequired,
            IsAnswered = questionAnswer?.HasAnswer() ?? false,
            IsComplete = questionAnswer?.HasAnswer() ?? false,
            
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
