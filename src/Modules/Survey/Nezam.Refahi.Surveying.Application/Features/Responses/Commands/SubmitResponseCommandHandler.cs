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

namespace Nezam.Refahi.Surveying.Application.Features.Responses.Commands;

/// <summary>
/// Handler for SubmitResponseCommand (C6)
/// </summary>
public class SubmitResponseCommandHandler : IRequestHandler<SubmitResponseCommand, ApplicationResult<SubmitResponseResponse>>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ISurveyUnitOfWork _unitOfWork;
    private readonly ILogger<SubmitResponseCommandHandler> _logger;
    private readonly IMemberService _memberService;
    private readonly ParticipationRulesDomainService _participationRulesService;

    public SubmitResponseCommandHandler(
        ISurveyRepository surveyRepository,
        ISurveyUnitOfWork unitOfWork,
        ILogger<SubmitResponseCommandHandler> logger,
        IMemberService memberService,
        ParticipationRulesDomainService participationRulesService)
    {
        _surveyRepository = surveyRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _memberService = memberService;
        _participationRulesService = participationRulesService;
    }

    public async Task<ApplicationResult<SubmitResponseResponse>> Handle(SubmitResponseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Efficiently load survey with response and questions
            var survey = await _surveyRepository.GetSurveyWithResponseAndQuestionsAsync(request.ResponseId, cancellationToken);
            if (survey == null)
            {
                return ApplicationResult<SubmitResponseResponse>.Failure("نظرسنجی یافت نشد");
            }

            // Check member authorization if NationalNumber is provided
            if (!string.IsNullOrWhiteSpace(request.NationalNumber))
            {
                var nationalId = new NationalId(request.NationalNumber);
                var memberDetail = await _memberService.GetMemberDetailByNationalCodeAsync(nationalId);
                if (memberDetail == null)
                {
                    return ApplicationResult<SubmitResponseResponse>.Failure("عضو یافت نشد");
                }

                // Validate member authorization against survey features/capabilities
                var authorizationResult = _participationRulesService.ValidateMemberAuthorizationWithDetails(
                    survey, 
                    memberDetail.Features ?? new List<string>(), 
                    memberDetail.Capabilities ?? new List<string>());
                if (!authorizationResult.IsAuthorized)
                {
                    return ApplicationResult<SubmitResponseResponse>.Failure(
                        authorizationResult.ErrorMessage ?? "شما مجاز به شرکت در این نظرسنجی نیستید");
                }
            }

            var response = survey.Responses.FirstOrDefault(r => r.Id == request.ResponseId);
            if (response == null)
            {
                return ApplicationResult<SubmitResponseResponse>.Failure("پاسخ یافت نشد");
            }

            // Check if response is already submitted
            if (response.SubmittedAt.HasValue)
            {
                return ApplicationResult<SubmitResponseResponse>.Failure("RESPONSE_ALREADY_SUBMITTED: پاسخ قبلاً ارسال شده است");
            }

            // Check survey status
            if (!survey.IsAcceptingResponses())
            {
                return ApplicationResult<SubmitResponseResponse>.Failure("SURVEY_NOT_ACTIVE: نظرسنجی در حال حاضر فعال نیست");
            }

            // Check time window
            if (survey.StartAt.HasValue && DateTimeOffset.UtcNow < survey.StartAt.Value)
            {
                return ApplicationResult<SubmitResponseResponse>.Failure("WINDOW_CLOSED: نظرسنجی هنوز شروع نشده است");
            }

            if (survey.EndAt.HasValue && DateTimeOffset.UtcNow > survey.EndAt.Value)
            {
                return ApplicationResult<SubmitResponseResponse>.Failure("WINDOW_CLOSED: نظرسنجی به پایان رسیده است");
            }

            // Check required questions
            var unansweredRequiredQuestions = survey.Questions
                .Where(q => q.IsRequired && !IsQuestionAnswered(response, q.Id))
                .Select(q => q.Id)
                .ToList();

            if (unansweredRequiredQuestions.Any())
            {
                return ApplicationResult<SubmitResponseResponse>.Failure("REQUIRED_NOT_ANSWERED: سوالات اجباری پاسخ داده نشده‌اند");
            }

            // Submit response using domain aggregate
            survey.SubmitResponse(request.ResponseId);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Calculate summary
            var answeredCount = response.QuestionAnswers.Count(qa => qa.HasAnswer());
            var totalCount = survey.Questions.Count;
            var completionPercentage = totalCount > 0 ? (double)answeredCount / totalCount * 100 : 0;

            var responseDto = new SubmitResponseResponse
            {
                Submitted = true,
                SubmittedAt = response.SubmittedAt,
                Summary = new ResponseSummaryDto
                {
                    Answered = answeredCount,
                    Total = totalCount,
                    Completion = completionPercentage,
                    UnansweredRequiredQuestions = unansweredRequiredQuestions
                }
            };

            return ApplicationResult<SubmitResponseResponse>.Success(responseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting response {ResponseId}", request.ResponseId);
            return ApplicationResult<SubmitResponseResponse>.Failure(ex, "خطا در ارسال پاسخ");
        }
    }

    private static bool IsQuestionAnswered(Response response, Guid questionId)
    {
        var questionAnswer = response.QuestionAnswers.FirstOrDefault(qa => qa.QuestionId == questionId);
        return questionAnswer?.HasAnswer() == true;
    }
}
