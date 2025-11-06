using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Surveying.Contracts.Dtos;
using Nezam.Refahi.Surveying.Contracts.Queries;
using Nezam.Refahi.Surveying.Domain.Entities;
using Nezam.Refahi.Surveying.Domain.Enums;
using Nezam.Refahi.Surveying.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;
using Nezam.Refahi.Membership.Contracts.Services;

namespace Nezam.Refahi.Surveying.Application.Features.Questions.Queries;

/// <summary>
/// Handler for GetQuestionAnswerDetailsQuery
/// </summary>
public class GetQuestionAnswerDetailsQueryHandler : IRequestHandler<GetQuestionAnswerDetailsQuery, ApplicationResult<QuestionAnswerDetailsDto>>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ILogger<GetQuestionAnswerDetailsQueryHandler> _logger;
    private readonly IMemberService _memberService;
    public GetQuestionAnswerDetailsQueryHandler(
        ISurveyRepository surveyRepository,
        ILogger<GetQuestionAnswerDetailsQueryHandler> logger,
        IMemberService memberService)
    {
        _surveyRepository = surveyRepository;
        _logger = logger;
        _memberService = memberService;
    }

    public async Task<ApplicationResult<QuestionAnswerDetailsDto>> Handle(GetQuestionAnswerDetailsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Get survey with response and questions
            var survey = await _surveyRepository.GetSurveyWithResponseAndQuestionsAsync(request.ResponseId, cancellationToken);
            if (survey == null)
                return ApplicationResult<QuestionAnswerDetailsDto>.Failure("نظرسنجی یا پاسخ یافت نشد");

            // Get response from survey aggregate
            var response = survey.Responses.FirstOrDefault(r => r.Id == request.ResponseId);
            if (response == null)
                return ApplicationResult<QuestionAnswerDetailsDto>.Failure("پاسخ یافت نشد");

                    // Authorization check if MemberId is provided
            if (!string.IsNullOrWhiteSpace(request.UserNationalNumber))
            {
                var nationalId = new NationalId(request.UserNationalNumber);
                var memberDetail = await _memberService.GetMemberDetailByNationalCodeAsync(nationalId);
                
                if (memberDetail == null || response.Participant.MemberId != memberDetail.Id)
                    return ApplicationResult<QuestionAnswerDetailsDto>.Failure("شما دسترسی به این پاسخ ندارید");
            }

            // Get question answer from response
            var questionAnswer = response.GetQuestionAnswer(request.QuestionId, 1);
            
            if (questionAnswer == null)
                return ApplicationResult<QuestionAnswerDetailsDto>.Failure("پاسخ سوال یافت نشد");

            // Get survey and question if requested
            Question? question = null;
            
            if (request.IncludeQuestionDetails || request.IncludeSurveyDetails)
            {
                survey = await _surveyRepository.GetWithQuestionsAsync(response.SurveyId, cancellationToken);
                question = survey?.Questions.FirstOrDefault(q => q.Id == request.QuestionId);
                
                if (question == null)
                    return ApplicationResult<QuestionAnswerDetailsDto>.Failure("سوال یافت نشد");
            }

            // Map to DTO
            var questionAnswerDto = new QuestionAnswerDetailsDto
            {
                Id = questionAnswer.Id,
                ResponseId = questionAnswer.ResponseId,
                QuestionId = questionAnswer.QuestionId,
                TextAnswer = questionAnswer.TextAnswer,
                SelectedOptionIds = questionAnswer.SelectedOptions.Select(so => so.OptionId).ToList(),
                IsAnswered = questionAnswer.HasAnswer(),
                IsComplete = questionAnswer.HasAnswer()
            };

            // Include question details if requested
            if (request.IncludeQuestionDetails)
            {
                questionAnswerDto.Question = MapQuestionToDto(question!);
                questionAnswerDto.SelectedOptions = questionAnswer.SelectedOptions
                    .Select(so => question?.Options.FirstOrDefault(o => o.Id == so.OptionId))
                    .Where(o => o != null)
                    .Select(MapOptionToDto)
                    .ToList();
            }

            return ApplicationResult<QuestionAnswerDetailsDto>.Success(questionAnswerDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting question answer details for ResponseId {ResponseId}, QuestionId {QuestionId}", 
                request.ResponseId, request.QuestionId);
            return ApplicationResult<QuestionAnswerDetailsDto>.Failure(ex, "خطا در دریافت جزئیات پاسخ سوال");
        }
    }

    private static QuestionDetailsDto MapQuestionToDto(Question question)
    {
        return new QuestionDetailsDto
        {
            Id = question.Id,
            SurveyId = question.SurveyId,
            Kind = question.Kind.ToString(),
            KindText = GetQuestionKindText(question.Kind),
            Text = question.Text,
            Order = question.Order,
            IsRequired = question.IsRequired,
            Options = question.Options
                .Where(o => o.IsActive)
                .OrderBy(o => o.Order)
                .Select(MapOptionToDto)
                .ToList()
        };
    }

    private static Contracts.Dtos.QuestionOptionDto MapOptionToDto(QuestionOption? option)
    {
        if (option == null)
            throw new ArgumentNullException(nameof(option));
            
        return new Contracts.Dtos.QuestionOptionDto
        {
            Id = option.Id,
            QuestionId = option.QuestionId,
            Text = option.Text,
            Order = option.Order,
            IsActive = option.IsActive
        };
    }

    private static string GetQuestionKindText(QuestionKind kind)
    {
        return kind switch
        {
            QuestionKind.FixedMCQ4 => "چهار گزینه‌ای ثابت",
            QuestionKind.ChoiceSingle => "انتخاب تک",
            QuestionKind.ChoiceMulti => "انتخاب چندگانه",
            QuestionKind.Textual => "متنی",
            _ => "نامشخص"
        };
    }
}
