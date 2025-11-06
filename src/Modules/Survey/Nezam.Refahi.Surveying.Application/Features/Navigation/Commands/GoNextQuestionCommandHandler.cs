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
            // Get survey with responses and questions
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
                // Allow read-only navigation for submitted responses (but don't modify state)
            }

            // Check if survey has questions
            if (!survey.Questions.Any())
            {
                return ApplicationResult<GoNextQuestionResponse>.Failure("نظرسنجی سوالی ندارد");
            }

            // Use domain behavior to navigate to next question
            var navigated = survey.NavigateResponseToNext(request.ResponseId);
            
            if (!navigated)
            {
                // Already at last question with no more repeats
                var (currentQuestionId, currentRepeatIndex) = survey.GetCurrentNavigationState(request.ResponseId);
                var currentQuestion = currentQuestionId.HasValue 
                    ? survey.Questions.FirstOrDefault(q => q.Id == currentQuestionId.Value) 
                    : null;

                if (currentQuestion == null)
                {
                    return ApplicationResult<GoNextQuestionResponse>.Failure("در حال حاضر در آخرین سوال هستید");
                }

                // Calculate progress
                var progress = CalculateProgress(survey, response);

                var responseDto = new GoNextQuestionResponse
                {
                    CurrentQuestionId = currentQuestionId,
                    CurrentRepeatIndex = currentRepeatIndex,
                    ResponseStatus = response.Status.ToString(),
                    ResponseStatusText = GetResponseStatusText(response.Status),
                    Progress = progress
                };

                return ApplicationResult<GoNextQuestionResponse>.Success(responseDto);
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

            var result = new GoNextQuestionResponse
            {
                CurrentQuestionId = questionId,
                CurrentRepeatIndex = repeatIndex,
                ResponseStatus = response.Status.ToString(),
                ResponseStatusText = GetResponseStatusText(response.Status),
                Progress = progressResult
            };

            _logger.LogInformation("Successfully navigated to next question for response {ResponseId}. Current: {QuestionId}, RepeatIndex: {RepeatIndex}", 
                request.ResponseId, questionId, repeatIndex);

            return ApplicationResult<GoNextQuestionResponse>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error navigating to next question for response {ResponseId}", request.ResponseId);
            return ApplicationResult<GoNextQuestionResponse>.Failure(ex, "خطا در ناوبری به سوال بعدی");
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
