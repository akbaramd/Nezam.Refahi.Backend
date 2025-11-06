using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Surveying.Application.Services;
using Nezam.Refahi.Surveying.Contracts.Commands;
using Nezam.Refahi.Surveying.Contracts.Dtos;
using Nezam.Refahi.Surveying.Domain.Entities;
using Nezam.Refahi.Surveying.Domain.Enums;
using Nezam.Refahi.Surveying.Domain.Repositories;

namespace Nezam.Refahi.Surveying.Application.Features.Navigation.Commands;

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
            // Get survey with responses and questions
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
                // Allow read-only navigation for submitted responses (but don't modify state)
            }

            // Check if back navigation is allowed
            if (!survey.ParticipationPolicy.AllowBackNavigation)
            {
                return ApplicationResult<GoPreviousQuestionResponse>.Failure("BACK_NOT_ALLOWED: ناوبری به عقب مجاز نیست");
            }

            // Check if survey has questions
            if (!survey.Questions.Any())
            {
                return ApplicationResult<GoPreviousQuestionResponse>.Failure("نظرسنجی سوالی ندارد");
            }

            // Use domain behavior to navigate to previous question
            var navigated = survey.NavigateResponseToPrevious(request.ResponseId);
            
            if (!navigated)
            {
                // Already at first question and first repeat
                var (currentQuestionId, currentRepeatIndex) = survey.GetCurrentNavigationState(request.ResponseId);
                var currentQuestion = currentQuestionId.HasValue 
                    ? survey.Questions.FirstOrDefault(q => q.Id == currentQuestionId.Value) 
                    : null;

                if (currentQuestion == null)
                {
                    return ApplicationResult<GoPreviousQuestionResponse>.Failure("در حال حاضر در اولین سوال هستید");
                }

                // Calculate progress
                var progress = CalculateProgress(survey, response);

                var responseDto = new GoPreviousQuestionResponse
                {
                    CurrentQuestionId = currentQuestionId,
                    CurrentRepeatIndex = currentRepeatIndex,
                    ResponseStatus = response.Status.ToString(),
                    ResponseStatusText = GetResponseStatusText(response.Status),
                    Progress = progress,
                    BackAllowed = false,
                    Message = "در حال حاضر در اولین سوال هستید"
                };

                return ApplicationResult<GoPreviousQuestionResponse>.Success(responseDto);
            }

            // Save navigation state changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Get updated navigation state
            var (questionId, repeatIndex) = survey.GetCurrentNavigationState(request.ResponseId);
            var question = questionId.HasValue 
                ? survey.Questions.FirstOrDefault(q => q.Id == questionId.Value) 
                : null;

            // Calculate progress
            var progressResult = CalculateProgress(survey, response);

            var result = new GoPreviousQuestionResponse
            {
                CurrentQuestionId = questionId,
                CurrentRepeatIndex = repeatIndex,
                ResponseStatus = response.Status.ToString(),
                ResponseStatusText = GetResponseStatusText(response.Status),
                Progress = progressResult,
                BackAllowed = true,
                Message = "ناوبری به سوال قبلی موفقیت‌آمیز بود"
            };

            _logger.LogInformation("Successfully navigated to previous question for response {ResponseId}. Current: {QuestionId}, RepeatIndex: {RepeatIndex}", 
                request.ResponseId, questionId, repeatIndex);

            return ApplicationResult<GoPreviousQuestionResponse>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error navigating to previous question for response {ResponseId}", request.ResponseId);
            return ApplicationResult<GoPreviousQuestionResponse>.Failure(ex, "خطا در ناوبری به سوال قبلی");
        }
    }

    private static CommandProgressDto CalculateProgress(Survey survey, Response response)
    {
        // Calculate progress (count unique answered questions, not repeats)
        var answeredQuestionIds = response.QuestionAnswers
            .Where(qa => qa.HasAnswer())
            .Select(qa => qa.QuestionId)
            .Distinct()
            .Count();
        var totalCount = survey.Questions.Count;
        var completionPercentage = totalCount > 0 ? (double)answeredQuestionIds / totalCount * 100 : 0;

        return new CommandProgressDto
        {
            Answered = answeredQuestionIds,
            Total = totalCount,
            CompletionPercentage = Math.Round(completionPercentage, 2)
        };
    }

    private static string GetResponseStatusText(ResponseStatus status)
    {
        return status switch
        {
            ResponseStatus.Answering => "در حال پاسخ‌دهی",
            ResponseStatus.Reviewing => "در حال بررسی",
            ResponseStatus.Completed => "تکمیل شده",
            ResponseStatus.Cancelled => "لغو شده",
            ResponseStatus.Expired => "منقضی شده",
            _ => status.ToString()
        };
    }
}
