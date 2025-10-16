using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Surveying.Application.Services;
using Nezam.Refahi.Surveying.Contracts.Commands;
using Nezam.Refahi.Surveying.Contracts.Dtos;
using Nezam.Refahi.Surveying.Domain.Entities;
using Nezam.Refahi.Surveying.Domain.Enums;
using Nezam.Refahi.Surveying.Domain.Repositories;

namespace Nezam.Refahi.Surveying.Application.Commands;

/// <summary>
/// Handler for GoNextQuestionCommand (C3)
/// </summary>
public class GoNextQuestionCommandHandler : IRequestHandler<GoNextQuestionCommand, ApplicationResult<GoNextQuestionResponse>>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ISurveyUnitOfWork _unitOfWork;
    private readonly ILogger<GoNextQuestionCommandHandler> _logger;

    public GoNextQuestionCommandHandler(
        ISurveyRepository surveyRepository,
        ISurveyUnitOfWork unitOfWork,
        ILogger<GoNextQuestionCommandHandler> logger)
    {
        _surveyRepository = surveyRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApplicationResult<GoNextQuestionResponse>> Handle(GoNextQuestionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get survey with responses
            var survey = await _surveyRepository.GetWithResponsesAsync(request.SurveyId, cancellationToken);
            if (survey == null)
            {
                return ApplicationResult<GoNextQuestionResponse>.Failure("نظرسنجی یافت نشد");
            }

            // Get response from survey aggregate
            var response = survey.Responses.FirstOrDefault(r => r.Id == request.ResponseId);
            if (response == null)
            {
                return ApplicationResult<GoNextQuestionResponse>.Failure("پاسخ یافت نشد");
            }

            // Check if response is submitted - allow read-only navigation
            if (response.SubmittedAt.HasValue)
            {
                _logger.LogInformation("Navigation requested for submitted response {ResponseId}", request.ResponseId);
                // Allow read-only navigation for submitted responses
            }

            // Get current question (first unanswered or first question for new attempts)
            var currentQuestion = GetCurrentQuestion(survey, response);
            if (currentQuestion == null)
            {
                // If no current question found, check if this is a new attempt with no answers
                if (!response.QuestionAnswers.Any(qa => qa.HasAnswer()))
                {
                    // This is a new attempt, return the first question
                    currentQuestion = survey.Questions.OrderBy(q => q.Order).FirstOrDefault();
                    if (currentQuestion == null)
                    {
                        return ApplicationResult<GoNextQuestionResponse>.Failure("نظرسنجی سوالی ندارد");
                    }
                }
                else
                {
                    return ApplicationResult<GoNextQuestionResponse>.Failure("تمام سوالات پاسخ داده شده‌اند");
                }
            }

            // For navigation, we allow moving to next question even if current is not answered
            // The validation will happen during submission
            // Only block if this is a submitted response (read-only mode)
            if (response.SubmittedAt.HasValue)
            {
                _logger.LogInformation("Navigation for submitted response {ResponseId} - allowing read-only navigation", request.ResponseId);
            }

            // Get next question
            var nextQuestion = survey.Questions
                .Where(q => q.Order > currentQuestion.Order)
                .OrderBy(q => q.Order)
                .FirstOrDefault();

            // Calculate progress (count unique answered questions, not repeats)
            var answeredQuestionIds = response.QuestionAnswers
                .Where(qa => qa.HasAnswer())
                .Select(qa => qa.QuestionId)
                .Distinct()
                .Count();
            var totalCount = survey.Questions.Count;
            var completionPercentage = totalCount > 0 ? (double)answeredQuestionIds / totalCount * 100 : 0;

            var responseDto = new GoNextQuestionResponse
            {
                CurrentQuestionId = nextQuestion?.Id,
                CurrentRepeatIndex = 1, // Default to 1, will be enhanced later
                Progress = new CommandProgressDto
                {
                    Answered = answeredQuestionIds,
                    Total = totalCount,
                    CompletionPercentage = completionPercentage
                }
            };

            return ApplicationResult<GoNextQuestionResponse>.Success(responseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error navigating to next question for response {ResponseId}", request.ResponseId);
            return ApplicationResult<GoNextQuestionResponse>.Failure("خطا در ناوبری به سوال بعدی");
        }
    }

    private static Question? GetCurrentQuestion(Survey survey, Response response)
    {
        // For submitted responses, return the first question for review
        if (response.SubmittedAt.HasValue)
        {
            return survey.Questions.OrderBy(q => q.Order).FirstOrDefault();
        }

        // For active responses, find the first unanswered question
        return survey.Questions
            .Where(q => !IsQuestionAnswered(response, q.Id))
            .OrderBy(q => q.Order)
            .FirstOrDefault();
    }

    private static bool IsQuestionAnswered(Response response, Guid questionId)
    {
        // Check if there's at least one answered repeat for this question
        return response.QuestionAnswers
            .Any(qa => qa.QuestionId == questionId && qa.HasAnswer());
    }
}
