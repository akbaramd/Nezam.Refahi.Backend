using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Surveying.Application.Services;
using Nezam.Refahi.Surveying.Contracts.Commands;
using Nezam.Refahi.Surveying.Domain.Entities;
using Nezam.Refahi.Surveying.Domain.Repositories;

namespace Nezam.Refahi.Surveying.Application.Features.Navigation.Commands;

/// <summary>
/// Handler for JumpToQuestionCommand (C5)
/// </summary>
public class JumpToQuestionCommandHandler : IRequestHandler<JumpToQuestionCommand, ApplicationResult<JumpToQuestionResponse>>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ISurveyUnitOfWork _unitOfWork;
    private readonly ILogger<JumpToQuestionCommandHandler> _logger;

    public JumpToQuestionCommandHandler(
        ISurveyRepository surveyRepository,
        ISurveyUnitOfWork unitOfWork,
        ILogger<JumpToQuestionCommandHandler> logger)
    {
        _surveyRepository = surveyRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApplicationResult<JumpToQuestionResponse>> Handle(JumpToQuestionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get survey with responses and questions
            var survey = await _surveyRepository.GetWithResponsesAsync(request.SurveyId, cancellationToken);
            if (survey == null)
            {
                return ApplicationResult<JumpToQuestionResponse>.Failure("نظرسنجی یافت نشد");
            }

            // Get response from survey aggregate
            var response = survey.Responses.FirstOrDefault(r => r.Id == request.ResponseId);
            if (response == null)
            {
                return ApplicationResult<JumpToQuestionResponse>.Failure("پاسخ یافت نشد");
            }

            // Check if response is submitted - allow read-only navigation
            if (response.SubmittedAt.HasValue)
            {
                _logger.LogInformation("Jump navigation requested for submitted response {ResponseId}", request.ResponseId);
                // Allow read-only navigation for submitted responses (but don't modify state)
            }

            // Check if back navigation is allowed (only if jumping backwards)
            var currentQuestion = survey.GetCurrentQuestionForResponse(request.ResponseId);
            var targetQuestion = survey.Questions.FirstOrDefault(q => q.Id == request.TargetQuestionId);
            
            if (targetQuestion == null)
            {
                return ApplicationResult<JumpToQuestionResponse>.Failure("سوال مورد نظر یافت نشد");
            }

            // Check if jumping backwards and back navigation is not allowed
            if (currentQuestion != null && targetQuestion.Order < currentQuestion.Order)
            {
                if (!survey.ParticipationPolicy.AllowBackNavigation)
                {
                    return ApplicationResult<JumpToQuestionResponse>.Failure("BACK_NOT_ALLOWED: ناوبری به عقب مجاز نیست");
                }
            }

            // Validate repeat index if provided
            if (request.TargetRepeatIndex.HasValue && !targetQuestion.ValidateRepeatIndex(request.TargetRepeatIndex.Value))
            {
                return ApplicationResult<JumpToQuestionResponse>.Failure($"تکرار {request.TargetRepeatIndex.Value} برای این سوال مجاز نیست");
            }

            // Use domain behavior to navigate to specific question
            // If TargetQuestionId is Guid.Empty or null, navigate to first question
            Guid? questionId = request.TargetQuestionId == Guid.Empty ? null : request.TargetQuestionId;
            survey.NavigateResponseToQuestion(request.ResponseId, questionId, request.TargetRepeatIndex, isFirst: true);

            // Save navigation state changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Get updated navigation state
            var (finalQuestionId, finalRepeatIndex) = survey.GetCurrentNavigationState(request.ResponseId);
            var finalQuestion = finalQuestionId.HasValue 
                ? survey.Questions.FirstOrDefault(q => q.Id == finalQuestionId.Value) 
                : null;

            var result = new JumpToQuestionResponse
            {
                CurrentQuestionId = finalQuestionId,
                CurrentRepeatIndex = finalRepeatIndex,
                BackAllowed = survey.ParticipationPolicy.AllowBackNavigation,
                Message = "پرش به سوال مورد نظر موفقیت‌آمیز بود"
            };

            _logger.LogInformation("Successfully jumped to question {QuestionId} (repeat {RepeatIndex}) for response {ResponseId}", 
                finalQuestionId, finalRepeatIndex, request.ResponseId);

            return ApplicationResult<JumpToQuestionResponse>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error jumping to question {QuestionId} for response {ResponseId}", 
                request.TargetQuestionId, request.ResponseId);
            return ApplicationResult<JumpToQuestionResponse>.Failure(ex, "خطا در پرش به سوال مورد نظر");
        }
    }
}
