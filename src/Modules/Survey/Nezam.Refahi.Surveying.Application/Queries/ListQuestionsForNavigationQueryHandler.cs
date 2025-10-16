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
/// Handler for ListQuestionsForNavigationQuery
/// </summary>
public class ListQuestionsForNavigationQueryHandler : IRequestHandler<ListQuestionsForNavigationQuery, ApplicationResult<QuestionsNavigationResponse>>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ILogger<ListQuestionsForNavigationQueryHandler> _logger;

    public ListQuestionsForNavigationQueryHandler(
        ISurveyRepository surveyRepository,
        ILogger<ListQuestionsForNavigationQueryHandler> logger)
    {
        _surveyRepository = surveyRepository;
        _logger = logger;
    }

    public async Task<ApplicationResult<QuestionsNavigationResponse>> Handle(
        ListQuestionsForNavigationQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting questions navigation for response {ResponseId} in survey {SurveyId}", 
                request.ResponseId, request.SurveyId);

            // Get survey with responses to find the specific response
            var survey = await _surveyRepository.GetWithResponsesAsync(request.SurveyId, cancellationToken);
            if (survey == null)
            {
                _logger.LogWarning("Survey {SurveyId} not found", request.SurveyId);
                return ApplicationResult<QuestionsNavigationResponse>.Failure("Survey not found");
            }

            // Get response from the survey aggregate
            var response = survey.Responses.FirstOrDefault(r => r.Id == request.ResponseId);
            if (response == null)
            {
                _logger.LogWarning("Response {ResponseId} not found", request.ResponseId);
                return ApplicationResult<QuestionsNavigationResponse>.Failure("Response not found");
            }

            var result = await MapToResponse(survey, response, request.IncludeBackNavigation);
            
            _logger.LogInformation("Successfully retrieved questions navigation for response {ResponseId}", request.ResponseId);
            return ApplicationResult<QuestionsNavigationResponse>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting questions navigation for response {ResponseId} in survey {SurveyId}", 
                request.ResponseId, request.SurveyId);
            return ApplicationResult<QuestionsNavigationResponse>.Failure("An error occurred while retrieving questions navigation");
        }
    }

    private Task<QuestionsNavigationResponse> MapToResponse(
        Survey survey, 
        Response response, 
        bool includeBackNavigation)
    {
        var orderedQuestions = survey.GetOrderedQuestions().ToList();
        var answeredQuestionIds = response.GetAnsweredQuestionIds().ToHashSet();
        
        // Find current question (first unanswered question)
        var currentQuestion = orderedQuestions.FirstOrDefault(q => !answeredQuestionIds.Contains(q.Id));
        var currentIndex = currentQuestion != null ? orderedQuestions.IndexOf(currentQuestion) : orderedQuestions.Count - 1;
        
        var progressPercentage = orderedQuestions.Count > 0 ? (double)(currentIndex + 1) / orderedQuestions.Count * 100 : 0;

        var navigationQuestions = orderedQuestions.Select((q, index) => new NavigationQuestionDto
        {
            QuestionId = q.Id,
            QuestionText = q.Text,
            QuestionKind = q.Kind.ToString(),
            Order = q.Order,
            IsRequired = q.IsRequired,
            IsAnswered = answeredQuestionIds.Contains(q.Id),
            IsComplete = answeredQuestionIds.Contains(q.Id),
            IsCurrent = index == currentIndex,
            LastAnsweredAt = DateTime.MinValue // QuestionAnswer doesn't have CreatedAt
        }).ToList();

        // Filter based on back navigation setting
        if (!includeBackNavigation && !survey.ParticipationPolicy.AllowBackNavigation)
        {
            // Only show current and future questions
            navigationQuestions = navigationQuestions.Where(q => q.Order >= currentIndex + 1).ToList();
        }

        return Task.FromResult(new QuestionsNavigationResponse
        {
            ResponseId = response.Id,
            SurveyId = survey.Id,
            AttemptNumber = response.AttemptNumber,
            
            AllowBackNavigation = survey.ParticipationPolicy.AllowBackNavigation,
            IncludeBackNavigation = includeBackNavigation,
            
            Questions = navigationQuestions,
            
            CurrentQuestionId = currentQuestion?.Id,
            CurrentQuestionNumber = currentIndex + 1,
            TotalQuestions = orderedQuestions.Count,
            ProgressPercentage = progressPercentage
        });
    }
}
