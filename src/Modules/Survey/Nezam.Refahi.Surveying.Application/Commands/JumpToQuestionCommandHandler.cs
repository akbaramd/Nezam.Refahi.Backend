using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Surveying.Application.Services;
using Nezam.Refahi.Surveying.Contracts.Commands;
using Nezam.Refahi.Surveying.Domain.Entities;
using Nezam.Refahi.Surveying.Domain.Repositories;

namespace Nezam.Refahi.Surveying.Application.Commands;

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
            // Get survey with responses
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
                // Allow read-only navigation for submitted responses
            }

            // Check if back navigation is allowed
            if (!survey.ParticipationPolicy.AllowBackNavigation)
            {
                return ApplicationResult<JumpToQuestionResponse>.Failure("BACK_NOT_ALLOWED: ناوبری به عقب مجاز نیست");
            }

            // Get target question
            var targetQuestion = survey.Questions.FirstOrDefault(q => q.Id == request.TargetQuestionId);
            if (targetQuestion == null)
            {
                return ApplicationResult<JumpToQuestionResponse>.Failure("سوال مورد نظر یافت نشد");
            }

            var responseDto = new JumpToQuestionResponse
            {
                CurrentQuestionId = targetQuestion.Id,
                CurrentRepeatIndex = request.TargetRepeatIndex ?? 1, // Default to 1 if not specified
                BackAllowed = true,
                Message = "پرش به سوال مورد نظر موفقیت‌آمیز بود"
            };

            return ApplicationResult<JumpToQuestionResponse>.Success(responseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error jumping to question {QuestionId} for response {ResponseId}", 
                request.TargetQuestionId, request.ResponseId);
            return ApplicationResult<JumpToQuestionResponse>.Failure("خطا در پرش به سوال مورد نظر");
        }
    }
}
