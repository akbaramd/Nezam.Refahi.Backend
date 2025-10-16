using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Surveying.Application.Services;
using Nezam.Refahi.Surveying.Contracts.Commands;
using Nezam.Refahi.Surveying.Contracts.Dtos;
using Nezam.Refahi.Surveying.Domain.Entities;
using Nezam.Refahi.Surveying.Domain.Repositories;

namespace Nezam.Refahi.Surveying.Application.Commands;

/// <summary>
/// Handler for GoPreviousQuestionCommand (C4)
/// </summary>
public class GoPreviousQuestionCommandHandler : IRequestHandler<GoPreviousQuestionCommand, ApplicationResult<GoPreviousQuestionResponse>>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ISurveyUnitOfWork _unitOfWork;
    private readonly ILogger<GoPreviousQuestionCommandHandler> _logger;

    public GoPreviousQuestionCommandHandler(
        ISurveyRepository surveyRepository,
        ISurveyUnitOfWork unitOfWork,
        ILogger<GoPreviousQuestionCommandHandler> logger)
    {
        _surveyRepository = surveyRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApplicationResult<GoPreviousQuestionResponse>> Handle(GoPreviousQuestionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get survey with responses
            var survey = await _surveyRepository.GetWithResponsesAsync(request.SurveyId, cancellationToken);
            if (survey == null)
            {
                return ApplicationResult<GoPreviousQuestionResponse>.Failure("نظرسنجی یافت نشد");
            }

            // Get response from survey aggregate
            var response = survey.Responses.FirstOrDefault(r => r.Id == request.ResponseId);
            if (response == null)
            {
                return ApplicationResult<GoPreviousQuestionResponse>.Failure("پاسخ یافت نشد");
            }

            // Check if response is submitted - allow read-only navigation
            if (response.SubmittedAt.HasValue)
            {
                _logger.LogInformation("Previous navigation requested for submitted response {ResponseId}", request.ResponseId);
                // Allow read-only navigation for submitted responses
            }

            // Check if back navigation is allowed
            if (!survey.ParticipationPolicy.AllowBackNavigation)
            {
                return ApplicationResult<GoPreviousQuestionResponse>.Failure("BACK_NOT_ALLOWED: ناوبری به عقب مجاز نیست");
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
                        return ApplicationResult<GoPreviousQuestionResponse>.Failure("نظرسنجی سوالی ندارد");
                    }
                }
                else
                {
                    return ApplicationResult<GoPreviousQuestionResponse>.Failure("تمام سوالات پاسخ داده شده‌اند");
                }
            }

            // Get previous question
            var previousQuestion = survey.Questions
                .Where(q => q.Order < currentQuestion.Order)
                .OrderByDescending(q => q.Order)
                .FirstOrDefault();

            // Calculate progress (count unique answered questions, not repeats)
            var answeredQuestionIds = response.QuestionAnswers
                .Where(qa => qa.HasAnswer())
                .Select(qa => qa.QuestionId)
                .Distinct()
                .Count();
            var totalCount = survey.Questions.Count;
            var completionPercentage = totalCount > 0 ? (double)answeredQuestionIds / totalCount * 100 : 0;

            var responseDto = new GoPreviousQuestionResponse
            {
                CurrentQuestionId = previousQuestion?.Id,
                CurrentRepeatIndex = 1, // Default to 1, will be enhanced later
                Progress = new CommandProgressDto
                {
                    Answered = answeredQuestionIds,
                    Total = totalCount,
                    CompletionPercentage = completionPercentage
                },
                BackAllowed = true,
                Message = "ناوبری به سوال قبلی موفقیت‌آمیز بود"
            };

            return ApplicationResult<GoPreviousQuestionResponse>.Success(responseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error navigating to previous question for response {ResponseId}", request.ResponseId);
            return ApplicationResult<GoPreviousQuestionResponse>.Failure("خطا در ناوبری به سوال قبلی");
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
