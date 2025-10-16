using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Membership.Contracts.Dtos;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;
using Nezam.Refahi.Surveying.Application.Services;
using Nezam.Refahi.Surveying.Contracts.Commands;
using Nezam.Refahi.Surveying.Domain.Entities;
using Nezam.Refahi.Surveying.Domain.Enums;
using Nezam.Refahi.Surveying.Domain.Repositories;
using Nezam.Refahi.Surveying.Domain.Services;

namespace Nezam.Refahi.Surveying.Application.Commands;

/// <summary>
/// Handler for SubmitSurveyResponseCommand
/// </summary>
public class SubmitSurveyResponseCommandHandler : IRequestHandler<SubmitSurveyResponseCommand, ApplicationResult<SubmitSurveyResponseResponse>>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ISurveyUnitOfWork _unitOfWork;
    private readonly ILogger<SubmitSurveyResponseCommandHandler> _logger;
    private readonly IMemberInfoService _memberInfoService;
    private readonly ParticipationRulesDomainService _participationRulesService;

    public SubmitSurveyResponseCommandHandler(
        ISurveyRepository surveyRepository,
        ISurveyUnitOfWork unitOfWork,
        ILogger<SubmitSurveyResponseCommandHandler> logger,
        IMemberInfoService memberInfoService,
        ParticipationRulesDomainService participationRulesService)
    {
        _surveyRepository = surveyRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _memberInfoService = memberInfoService;
        _participationRulesService = participationRulesService;
    }

    public async Task<ApplicationResult<SubmitSurveyResponseResponse>> Handle(SubmitSurveyResponseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Efficiently load survey with response and questions
            var survey = await _surveyRepository.GetSurveyWithResponseAndQuestionsAsync(request.ResponseId, cancellationToken);
            if (survey == null)
            {
                return ApplicationResult<SubmitSurveyResponseResponse>.Failure("نظرسنجی یافت نشد");
            }

            // Check member authorization if NationalNumber is provided
            if (!string.IsNullOrWhiteSpace(request.NationalNumber))
            {
                var nationalId = new NationalId(request.NationalNumber);
                var memberInfo = await _memberInfoService.GetMemberInfoAsync(nationalId);
                if (memberInfo == null)
                {
                    return ApplicationResult<SubmitSurveyResponseResponse>.Failure("عضو یافت نشد");
                }

                // Validate member authorization against survey features/capabilities
                var authorizationResult = _participationRulesService.ValidateMemberAuthorizationWithDetails(
                    survey, 
                    memberInfo.Features ?? new List<string>(), 
                    memberInfo.Capabilities ?? new List<string>());
                if (!authorizationResult.IsAuthorized)
                {
                    return ApplicationResult<SubmitSurveyResponseResponse>.Failure(
                        authorizationResult.ErrorMessage ?? "شما مجاز به شرکت در این نظرسنجی نیستید");
                }
            }

            var response = survey.Responses.FirstOrDefault(r => r.Id == request.ResponseId);
            if (response == null)
            {
                return ApplicationResult<SubmitSurveyResponseResponse>.Failure("پاسخ یافت نشد");
            }

            // Check if survey is still active
            if (survey.State != SurveyState.Active)
            {
                return ApplicationResult<SubmitSurveyResponseResponse>.Failure("نظرسنجی در حال حاضر فعال نیست");
            }

            // Process all answers using domain aggregate
            foreach (var answerDto in request.Answers)
            {
                var question = survey.Questions.FirstOrDefault(q => q.Id == answerDto.QuestionId);
                if (question == null)
                    continue;

                // Use domain aggregate to set the answer
                survey.SetResponseAnswer(request.ResponseId, answerDto.QuestionId, answerDto.TextAnswer, answerDto.SelectedOptionIds);
            }

            // Submit the response using domain aggregate
            survey.SubmitResponse(request.ResponseId);

            // Save all changes using Unit of Work
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Calculate completion statistics
            var totalQuestions = survey.Questions.Count;
            var answeredQuestions = response.QuestionAnswers.Count(qa => qa.HasAnswer());
            var requiredQuestions = survey.Questions.Count(q => q.IsRequired);
            var requiredAnsweredQuestions = survey.Questions
                .Where(q => q.IsRequired)
                .Count(q => response.QuestionAnswers.Any(qa => qa.QuestionId == q.Id && qa.HasAnswer()));

            var completionPercentage = totalQuestions > 0 ? (decimal)answeredQuestions / totalQuestions * 100 : 0;
            var isComplete = answeredQuestions == totalQuestions && requiredAnsweredQuestions == requiredQuestions;

            var responseDto = new SubmitSurveyResponseResponse
            {
                ResponseId = request.ResponseId,
                SurveyId = survey.Id,
                IsSubmitted = true,
                IsComplete = isComplete,
                AnsweredQuestions = answeredQuestions,
                TotalQuestions = totalQuestions,
                CompletionPercentage = completionPercentage,
                Message = isComplete ? "نظرسنجی با موفقیت تکمیل شد" : "پاسخ‌ها با موفقیت ثبت شدند"
            };

            return ApplicationResult<SubmitSurveyResponseResponse>.Success(responseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting survey response {ResponseId}", request.ResponseId);
            return ApplicationResult<SubmitSurveyResponseResponse>.Failure("خطا در ارسال نظرسنجی");
        }
    }
}
