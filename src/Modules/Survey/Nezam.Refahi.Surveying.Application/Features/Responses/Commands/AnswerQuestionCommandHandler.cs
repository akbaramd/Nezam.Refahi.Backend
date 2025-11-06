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
using Nezam.Refahi.Surveying.Domain.ValueObjects;

namespace Nezam.Refahi.Surveying.Application.Features.Responses.Commands;

/// <summary>
/// Handler for AnswerQuestionCommand
/// </summary>
public class AnswerQuestionCommandHandler : IRequestHandler<AnswerQuestionCommand, ApplicationResult<AnswerQuestionResponse>>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ISurveyUnitOfWork _unitOfWork;
    private readonly ILogger<AnswerQuestionCommandHandler> _logger;
    private readonly IMemberService _memberService;
    private readonly ParticipationRulesDomainService _participationRulesService;

    public AnswerQuestionCommandHandler(
        ISurveyRepository surveyRepository,
        ISurveyUnitOfWork unitOfWork,
        ILogger<AnswerQuestionCommandHandler> logger,
        IMemberService memberService,
        ParticipationRulesDomainService participationRulesService)
    {
        _surveyRepository = surveyRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _memberService = memberService;
        _participationRulesService = participationRulesService;
    }

    public async Task<ApplicationResult<AnswerQuestionResponse>> Handle(AnswerQuestionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Efficiently load survey with response and questions
            var survey = await _surveyRepository.GetSurveyWithResponseAndQuestionsAsync(request.ResponseId, cancellationToken);
            if (survey == null)
            {
                return ApplicationResult<AnswerQuestionResponse>.Failure("پاسخ یافت نشد");
            }

            // Check member authorization if NationalNumber is provided
            if (!string.IsNullOrWhiteSpace(request.NationalNumber))
            {
                var nationalId = new NationalId(request.NationalNumber);
                var memberDetail = await _memberService.GetMemberDetailByNationalCodeAsync(nationalId);
                if (memberDetail == null)
                {
                    return ApplicationResult<AnswerQuestionResponse>.Failure("عضو یافت نشد");
                }

                // Validate member authorization against survey features/capabilities
                var authorizationResult = _participationRulesService.ValidateMemberAuthorizationWithDetails(
                    survey, 
                    memberDetail.Features ?? new List<string>(), 
                    memberDetail.Capabilities ?? new List<string>());
                if (!authorizationResult.IsAuthorized)
                {
                    return ApplicationResult<AnswerQuestionResponse>.Failure(
                        authorizationResult.ErrorMessage ?? "شما مجاز به شرکت در این نظرسنجی نیستید");
                }
            }

            var response = survey.Responses.FirstOrDefault(r => r.Id == request.ResponseId);
            if (response == null)
            {
                return ApplicationResult<AnswerQuestionResponse>.Failure("پاسخ یافت نشد");
            }

            // Check if survey is still active
            if (!survey.IsAcceptingResponses())
            {
                return ApplicationResult<AnswerQuestionResponse>.Failure("نظرسنجی در حال حاضر فعال نیست");
            }

            // Get question
            var question = survey.Questions.FirstOrDefault(q => q.Id == request.QuestionId);
            if (question == null)
            {
                return ApplicationResult<AnswerQuestionResponse>.Failure("سوال یافت نشد");
            }

            // Validate repeat index
            if (!question.ValidateRepeatIndex(request.RepeatIndex))
            {
                return ApplicationResult<AnswerQuestionResponse>.Failure($"INVALID_REPEAT_INDEX: تکرار {request.RepeatIndex} برای این سوال مجاز نیست");
            }

            // Check repeat limits
            if (question.RepeatPolicy.Kind == RepeatPolicyKind.Bounded && request.RepeatIndex > question.RepeatPolicy.MaxRepeats!.Value)
            {
                return ApplicationResult<AnswerQuestionResponse>.Failure($"REPEAT_LIMIT_REACHED: از سقف تکرار مجاز ({question.RepeatPolicy.MaxRepeats}) عبور کرده‌اید");
            }

            // Validate answer based on question type and enforce rules
            var validationResult = ValidateAnswer(question, request.TextAnswer, request.SelectedOptionIds);
            if (!validationResult.IsValid)
            {
                return ApplicationResult<AnswerQuestionResponse>.Failure(validationResult.ErrorMessage ?? "خطا در اعتبارسنجی پاسخ");
            }

            // Check back navigation rules
            if (!request.AllowBackNavigation && !survey.ParticipationPolicy.AllowBackNavigation)
            {
                var currentQuestionOrder = question.Order;
                var answeredQuestionOrders = response.QuestionAnswers
                    .Where(qa => qa.QuestionId != request.QuestionId)
                    .Select(qa => survey.Questions.FirstOrDefault(q => q.Id == qa.QuestionId))
                    .Where(q => q != null)
                    .Select(q => q!.Order)
                    .ToList();

                if (answeredQuestionOrders.Any(order => order > currentQuestionOrder))
                {
                    return ApplicationResult<AnswerQuestionResponse>.Failure("RESPONSE_IMMUTABLE: نمی‌توانید به سوالات قبلی برگردید");
                }
            }

            // Get question text and convert selected option IDs to dictionary with option texts
            var questionText = question.Text;
            var selectedOptionsWithTexts = new Dictionary<Guid, string>();
            
            if (request.SelectedOptionIds != null && request.SelectedOptionIds.Any())
            {
                foreach (var optionId in request.SelectedOptionIds)
                {
                    var optionText = survey.GetOptionText(request.QuestionId, optionId);
                    if (!string.IsNullOrEmpty(optionText))
                    {
                        selectedOptionsWithTexts[optionId] = optionText;
                    }
                }
            }

            // Use domain aggregate to set the answer
            survey.SetResponseAnswer(request.ResponseId, request.QuestionId, questionText, request.TextAnswer, selectedOptionsWithTexts);

            // Manage response status based on completion
            var totalQuestions = survey.Questions.Count;
            var answeredQuestionIds = response.QuestionAnswers
                .Where(qa => qa.HasAnswer())
                .Select(qa => qa.QuestionId)
                .Distinct()
                .Count();
            
            var completionPercentage = totalQuestions > 0 ? (double)answeredQuestionIds / totalQuestions * 100 : 0;
            
            // If response is nearly complete (80% or more), move to reviewing status
            if (completionPercentage >= 80 && response.Status == ResponseStatus.Answering)
            {
                response.StartReviewing();
                _logger.LogInformation("Response {ResponseId} moved to reviewing status due to {CompletionPercentage}% completion", 
                    request.ResponseId, completionPercentage);
            }
            // If response is less than 80% complete and in reviewing, move back to answering
            else if (completionPercentage < 80 && response.Status == ResponseStatus.Reviewing)
            {
                response.ResumeAnswering();
                _logger.LogInformation("Response {ResponseId} moved back to answering status due to {CompletionPercentage}% completion", 
                    request.ResponseId, completionPercentage);
            }

            // Save all changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Calculate repeatable question statistics
            var answeredRepeatsForThisQuestion = response.QuestionAnswers
                .Count(qa => qa.QuestionId == request.QuestionId && qa.HasAnswer());
            
            var totalRepeatsAllowed = question.GetMaxRepeatIndex();

            var res = new AnswerQuestionResponse
            {
                ResponseId = request.ResponseId,
                QuestionId = request.QuestionId,
                RepeatIndex = request.RepeatIndex,
                IsAnswered = true,
                IsOverwritten = false, // Domain aggregate handles this
                AnsweredQuestions = answeredQuestionIds,
                AnsweredRepeatsForThisQuestion = answeredRepeatsForThisQuestion,
                TotalRepeatsAllowed = totalRepeatsAllowed,
                CompletionPercentage = completionPercentage,
                ResponseStatus = response.Status.ToString(),
                ResponseStatusText = ResponseStatusHelper.GetPersianText(response.Status),
                Message = "پاسخ با موفقیت ثبت شد",
                ValidationErrors = validationResult.ValidationErrors ?? new List<string>()
            };

            return ApplicationResult<AnswerQuestionResponse>.Success(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error answering question {QuestionId} for response {ResponseId}", 
                request.QuestionId, request.ResponseId);
            return ApplicationResult<AnswerQuestionResponse>.Failure(ex, "خطا در ثبت پاسخ");
        }
    }

    private static AnswerValidationResult ValidateAnswer(Question question, string? textAnswer, IEnumerable<Guid>? selectedOptionIds)
    {
        var errors = new List<string>();

        if (question.IsRequired && string.IsNullOrWhiteSpace(textAnswer) && (selectedOptionIds == null || !selectedOptionIds.Any()))
        {
            errors.Add("پاسخ به این سوال اجباری است");
        }

        if (question.Kind == QuestionKind.Textual)
        {
            if (selectedOptionIds != null && selectedOptionIds.Any())
            {
                errors.Add("سوال متنی نمی‌تواند گزینه انتخابی داشته باشد");
            }
            // Note: Text length validation would need to be added to QuestionSpecification
            // For now, we'll skip this validation
        }
        else // Choice questions
        {
            if (!string.IsNullOrWhiteSpace(textAnswer))
            {
                errors.Add("سوال انتخابی نمی‌تواند پاسخ متنی داشته باشد");
            }

            if (selectedOptionIds != null && selectedOptionIds.Any())
            {
                foreach (var optionId in selectedOptionIds)
                {
                    if (!question.Options.Any(o => o.Id == optionId && o.IsActive))
                    {
                        errors.Add($"گزینه انتخابی {optionId} معتبر نیست");
                    }
                }

                if (question.Kind == QuestionKind.ChoiceSingle && selectedOptionIds.Count() > 1)
                {
                    errors.Add("سوال تک انتخابی نمی‌تواند بیش از یک گزینه داشته باشد");
                }
            }
            else if (question.IsRequired)
            {
                errors.Add("انتخاب حداقل یک گزینه برای این سوال اجباری است");
            }
        }

        return new AnswerValidationResult(errors.Count == 0, errors.FirstOrDefault(), errors);
    }

    private record AnswerValidationResult(bool IsValid, string? ErrorMessage, List<string>? ValidationErrors);
}
