using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Surveying.Application.Services;
using Nezam.Refahi.Surveying.Contracts.Dtos;
using Nezam.Refahi.Surveying.Contracts.Queries;
using Nezam.Refahi.Surveying.Domain.Entities;
using Nezam.Refahi.Surveying.Domain.Enums;
using Nezam.Refahi.Surveying.Domain.Repositories;

namespace Nezam.Refahi.Surveying.Application.Queries;

/// <summary>
/// Handler for GetResponseProgressQuery
/// </summary>
public class GetResponseProgressQueryHandler : IRequestHandler<GetResponseProgressQuery, ApplicationResult<ResponseProgressResponse>>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ILogger<GetResponseProgressQueryHandler> _logger;

    public GetResponseProgressQueryHandler(
        ISurveyRepository surveyRepository,
        ILogger<GetResponseProgressQueryHandler> logger)
    {
        _surveyRepository = surveyRepository;
        _logger = logger;
    }

    public async Task<ApplicationResult<ResponseProgressResponse>> Handle(
        GetResponseProgressQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting progress for response {ResponseId} in survey {SurveyId}", 
                request.ResponseId, request.SurveyId);

            // Get survey with response
            var survey = await _surveyRepository.GetSurveyWithResponseAndQuestionsAsync(request.ResponseId, cancellationToken);
            if (survey == null)
            {
                _logger.LogWarning("Survey with response {ResponseId} not found", request.ResponseId);
                return ApplicationResult<ResponseProgressResponse>.Failure("Survey or response not found");
            }

            // Get response from survey aggregate
            var response = survey.Responses.FirstOrDefault(r => r.Id == request.ResponseId);
            if (response == null)
            {
                _logger.LogWarning("Response {ResponseId} not found in survey", request.ResponseId);
                return ApplicationResult<ResponseProgressResponse>.Failure("Response not found");
            }

            // Verify response belongs to survey
            if (response.SurveyId != request.SurveyId)
            {
                _logger.LogWarning("Response {ResponseId} does not belong to survey {SurveyId}", 
                    request.ResponseId, request.SurveyId);
                return ApplicationResult<ResponseProgressResponse>.Failure("Response does not belong to survey");
            }

            var result = await MapToResponse(survey, response);
            
            _logger.LogInformation("Successfully retrieved progress for response {ResponseId}", request.ResponseId);
            return ApplicationResult<ResponseProgressResponse>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting progress for response {ResponseId} in survey {SurveyId}", 
                request.ResponseId, request.SurveyId);
            return ApplicationResult<ResponseProgressResponse>.Failure("An error occurred while retrieving progress");
        }
    }

    private Task<ResponseProgressResponse> MapToResponse(Survey survey, Response response)
    {
        var orderedQuestions = survey.GetOrderedQuestions().ToList();
        var answeredQuestionIds = response.GetAnsweredQuestionIds().ToHashSet();
        
        var answeredQuestions = orderedQuestions.Count(q => answeredQuestionIds.Contains(q.Id));
        var requiredQuestions = orderedQuestions.Count(q => q.IsRequired);
        var answeredRequiredQuestions = orderedQuestions.Count(q => q.IsRequired && answeredQuestionIds.Contains(q.Id));
        
        var completionPercentage = orderedQuestions.Count > 0 ? (double)answeredQuestions / orderedQuestions.Count * 100 : 0;
        var requiredCompletionPercentage = requiredQuestions > 0 ? (double)answeredRequiredQuestions / requiredQuestions * 100 : 0;
        
        var isComplete = answeredQuestions == orderedQuestions.Count;
        var isSubmitted = response.SubmittedAt.HasValue;
        
        var timeSpent = isSubmitted && response.SubmittedAt.HasValue 
            ? response.SubmittedAt.Value - response.SubmittedAt 
            : (TimeSpan?)null;

        var questionsProgress = orderedQuestions.Select(q => new QuestionProgressDto
        {
            QuestionId = q.Id,
            QuestionText = q.Text,
            Order = q.Order,
            IsRequired = q.IsRequired,
            IsAnswered = answeredQuestionIds.Contains(q.Id),
            IsComplete = answeredQuestionIds.Contains(q.Id),
            LastAnsweredAt = DateTime.MinValue // QuestionAnswer doesn't have CreatedAt
        }).ToList();

        // Calculate repeatable questions progress
        var repeatables = orderedQuestions
            .Where(q => q.RepeatPolicy.Kind != Domain.ValueObjects.RepeatPolicyKind.None)
            .Select(q => new RepeatableQuestionProgressDto
            {
                QuestionId = q.Id,
                QuestionText = q.Text,
                RepeatPolicy = new RepeatPolicyDto
                {
                    Kind = q.RepeatPolicy.Kind.ToString(),
                    MaxRepeats = q.RepeatPolicy.MaxRepeats
                },
                AnsweredRepeats = response.GetAnsweredRepeatCount(q.Id),
                RequiredRepeats = q.IsRequired ? 1 : null, // At least 1 repeat required for required questions
                CanAddMoreRepeats = q.CanAddMoreRepeats(response.GetAnsweredRepeatCount(q.Id)),
                AnsweredRepeatIndices = response.GetAnsweredRepeatIndices(q.Id).ToList()
            })
            .ToList();

        return Task.FromResult(new ResponseProgressResponse
        {
            ResponseId = response.Id,
            SurveyId = survey.Id,
            AttemptNumber = response.AttemptNumber,
            AttemptStatus = response.AttemptStatus.ToString(),
            
            TotalQuestions = orderedQuestions.Count,
            AnsweredQuestions = answeredQuestions,
            RequiredQuestions = requiredQuestions,
            AnsweredRequiredQuestions = answeredRequiredQuestions,
            CompletionPercentage = completionPercentage,
            RequiredCompletionPercentage = requiredCompletionPercentage,
            
            IsComplete = isComplete,
            IsSubmitted = isSubmitted,
            SubmittedAt = response.SubmittedAt?.DateTime,
            StartedAt = response.SubmittedAt?.DateTime,
            TimeSpent = timeSpent,
            
            AllowBackNavigation = survey.ParticipationPolicy.AllowBackNavigation,
            QuestionsProgress = questionsProgress,
            Repeatables = repeatables
        });
    }
}
