using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Surveying.Contracts.Dtos;
using Nezam.Refahi.Surveying.Contracts.Queries;
using Nezam.Refahi.Surveying.Domain.Entities;
using Nezam.Refahi.Surveying.Domain.Enums;
using Nezam.Refahi.Surveying.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Membership.Contracts.Dtos;
using Nezam.Refahi.Shared.Domain.ValueObjects;
using Nezam.Refahi.Surveying.Domain.ValueObjects;

namespace Nezam.Refahi.Surveying.Application.Features.Responses.Queries;

/// <summary>
/// Handler for GetResponseDetailsQuery
/// </summary>
public class GetResponseDetailsQueryHandler : IRequestHandler<GetResponseDetailsQuery, ApplicationResult<ResponseDetailsDto>>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ILogger<GetResponseDetailsQueryHandler> _logger;
    private readonly IMemberService _memberService;
    public GetResponseDetailsQueryHandler(
        ISurveyRepository surveyRepository,
        ILogger<GetResponseDetailsQueryHandler> logger,
        IMemberService memberService)
    {
        _surveyRepository = surveyRepository;
        _logger = logger;
        _memberService = memberService;
    }

    public async Task<ApplicationResult<ResponseDetailsDto>> Handle(GetResponseDetailsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Get survey with response and all data
            var survey = await _surveyRepository.GetSurveyWithResponseAndAllDataAsync(request.ResponseId, cancellationToken);
            if (survey == null)
                return ApplicationResult<ResponseDetailsDto>.Failure("نظرسنجی یا پاسخ یافت نشد");

            // Get response from survey aggregate
            var response = survey.Responses.FirstOrDefault(r => r.Id == request.ResponseId);
            if (response == null)
                return ApplicationResult<ResponseDetailsDto>.Failure("پاسخ یافت نشد");

            // Authorization check if MemberId is provided and populate participant details
            MemberDetailDto? memberDetail = null;
            if (!string.IsNullOrWhiteSpace(request.UserNationalNumber))
            {
                var nationalId = new NationalId(request.UserNationalNumber);
                memberDetail = await _memberService.GetMemberDetailByNationalCodeAsync(nationalId);
                
                if (memberDetail == null || response.Participant.MemberId != memberDetail.Id)
                    return ApplicationResult<ResponseDetailsDto>.Failure("شما دسترسی به این پاسخ ندارید");
            }
            
            // Map participant with member details if available
            var participantDto = MapParticipantToDto(response.Participant, memberDetail);

            // Map to DTO
            var responseDto = new ResponseDetailsDto
            {
                Id = response.Id,
                SurveyId = response.SurveyId,
                AttemptNumber = response.AttemptNumber,
                Participant = participantDto,
                Survey = request.IncludeSurveyDetails ? MapSurveyToDto(survey) : null,
                QuestionAnswers = new List<QuestionAnswerDetailsDto>(),
                Statistics = new ResponseStatisticsDto(),
                Status = new ResponseStatusDto()
            };

            // Map question answers from response aggregate
            var questionAnswerDtos = new List<QuestionAnswerDetailsDto>();
            foreach (var questionAnswer in response.QuestionAnswers)
            {
                var questionAnswerDto = MapQuestionAnswerToDto(questionAnswer);
                
                // Include question details if requested
                if (request.IncludeQuestionDetails && survey != null)
                {
                    var question = survey.Questions.FirstOrDefault(q => q.Id == questionAnswer.QuestionId);
                    if (question != null)
                    {
                        questionAnswerDto.Question = MapQuestionToDto(question);
                        
                        // Map selected options with isSelected flag
                        var selectedOptionIds = questionAnswer.SelectedOptions.Select(so => so.OptionId).ToHashSet();
                        questionAnswerDto.SelectedOptions = question.Options
                            .Where(o => selectedOptionIds.Contains(o.Id))
                            .OrderBy(o => o.Order)
                            .Select(o => MapOptionToDtoWithSelection(o, true))
                            .ToList();
                        
                        // Also update question options to show which ones are selected
                        if (questionAnswerDto.Question.Options != null)
                        {
                            foreach (var option in questionAnswerDto.Question.Options)
                            {
                                option.IsSelected = selectedOptionIds.Contains(option.Id);
                            }
                        }
                    }
                }
                
                questionAnswerDtos.Add(questionAnswerDto);
            }

            responseDto.QuestionAnswers = questionAnswerDtos.OrderBy(qa => qa.Question?.Order ?? 0).ToList();

            // Calculate statistics
            responseDto.Statistics = CalculateResponseStatistics(response, questionAnswerDtos, survey);

            // Calculate status (must be after statistics as it uses them)
            responseDto.Status = CalculateResponseStatus(response, questionAnswerDtos, survey);
            
            // Also populate DemographyData if available
            if (response.DemographySnapshot != null && response.DemographySnapshot.Data != null)
            {
                responseDto.Participant.DemographyData = response.DemographySnapshot.Data;
            }

            return ApplicationResult<ResponseDetailsDto>.Success(responseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting response details for ResponseId {ResponseId}", request.ResponseId);
            return ApplicationResult<ResponseDetailsDto>.Failure(ex, "خطا در دریافت جزئیات پاسخ");
        }
    }

    private static ParticipantInfoDto MapParticipantToDto(Domain.ValueObjects.ParticipantInfo participant, MemberDetailDto? memberDetail = null)
    {
        var dto = new ParticipantInfoDto
        {
            MemberId = participant.MemberId,
            ParticipantHash = participant.ParticipantHash,
            IsAnonymous = participant.IsAnonymous,
            DemographyData = null // Will be populated from DemographySnapshot if available
        };

        // Populate member details if available
        if (memberDetail != null && !participant.IsAnonymous)
        {
            dto.NationalCode = memberDetail.NationalCode;
            dto.FullName = $"{memberDetail.FirstName} {memberDetail.LastName}".Trim();
        }

        return dto;
    }

    private static SurveyBasicInfoDto MapSurveyToDto(Survey survey)
    {
        return new SurveyBasicInfoDto
        {
            Id = survey.Id,
            Title = survey.Title,
            State = survey.State.ToString(),
            StateText = GetSurveyStateText(survey.State),
            IsActive = survey.IsAcceptingResponses(),
            IsAnonymous = survey.IsAnonymous,
            StartAt = survey.StartAt?.DateTime,
            EndAt = survey.EndAt?.DateTime,
            TotalQuestions = survey.Questions.Count,
            RequiredQuestions = survey.Questions.Count(q => q.IsRequired)
        };
    }

    private static QuestionAnswerDetailsDto MapQuestionAnswerToDto(Domain.Entities.QuestionAnswer questionAnswer)
    {
        return new QuestionAnswerDetailsDto
        {
            Id = questionAnswer.Id,
            ResponseId = questionAnswer.ResponseId,
            QuestionId = questionAnswer.QuestionId,
            TextAnswer = questionAnswer.TextAnswer,
            SelectedOptionIds = questionAnswer.SelectedOptions.Select(so => so.OptionId).ToList(),
            IsAnswered = questionAnswer.HasAnswer(),
            IsComplete = questionAnswer.HasAnswer()
        };
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
            IsActive = option.IsActive,
            IsSelected = false // Will be set by caller based on questionAnswer
        };
    }

    private static Contracts.Dtos.QuestionOptionDto MapOptionToDtoWithSelection(QuestionOption? option, bool isSelected)
    {
        if (option == null)
            throw new ArgumentNullException(nameof(option));
            
        return new Contracts.Dtos.QuestionOptionDto
        {
            Id = option.Id,
            QuestionId = option.QuestionId,
            Text = option.Text,
            Order = option.Order,
            IsActive = option.IsActive,
            IsSelected = isSelected
        };
    }

    private static ResponseStatisticsDto CalculateResponseStatistics(Response response, IEnumerable<QuestionAnswerDetailsDto> questionAnswers, Survey? survey)
    {
        var answeredQuestions = questionAnswers.Count(qa => qa.IsAnswered);
        var totalQuestions = survey?.Questions.Count ?? 0;
        var requiredQuestions = survey?.Questions.Count(q => q.IsRequired) ?? 0;
        var requiredAnsweredQuestions = questionAnswers.Count(qa => 
            qa.IsAnswered && survey?.Questions.FirstOrDefault(q => q.Id == qa.QuestionId)?.IsRequired == true);

        var completionPercentage = totalQuestions > 0 ? (decimal)answeredQuestions / totalQuestions * 100 : 0;
        var isComplete = answeredQuestions == totalQuestions && requiredAnsweredQuestions == requiredQuestions;

        var firstAnswerAt = (DateTime?)null; // QuestionAnswer is now Entity, no CreatedAt
        var lastAnswerAt = (DateTime?)null; // QuestionAnswer is now Entity, no CreatedAt
        var timeSpent = firstAnswerAt.HasValue && lastAnswerAt.HasValue ? lastAnswerAt.Value - firstAnswerAt.Value : (TimeSpan?)null;

        return new ResponseStatisticsDto
        {
            TotalQuestions = totalQuestions,
            AnsweredQuestions = answeredQuestions,
            RequiredQuestions = requiredQuestions,
            RequiredAnsweredQuestions = requiredAnsweredQuestions,
            CompletionPercentage = completionPercentage,
            IsComplete = isComplete,
            TimeSpent = timeSpent,
            FirstAnswerAt = firstAnswerAt,
            LastAnswerAt = lastAnswerAt
        };
    }

    private static ResponseStatusDto CalculateResponseStatus(Response response, IEnumerable<QuestionAnswerDetailsDto> questionAnswers, Survey? survey)
    {
        // Use entity methods to check status - this is the correct way
        var isSubmitted = response.AttemptStatus == AttemptStatus.Submitted || 
                         response.SubmittedAt.HasValue || 
                         response.IsCompleted();
        
        var isCanceled = response.AttemptStatus == AttemptStatus.Canceled;
        var isExpired = response.AttemptStatus == AttemptStatus.Expired;
        
        // CanContinue: Response must be modifiable AND survey must be accepting responses AND not submitted/canceled/expired
        var canContinue = response.CanBeModified() && 
                         survey != null && 
                         survey.IsAcceptingResponses() && 
                         !isSubmitted && 
                         !isCanceled && 
                         !isExpired;
        
        // Calculate completion status
        var answeredQuestions = questionAnswers.Count(qa => qa.IsAnswered);
        var totalQuestions = survey?.Questions.Count ?? 0;
        var requiredQuestions = survey?.Questions.Count(q => q.IsRequired) ?? 0;
        var requiredAnsweredQuestions = questionAnswers.Count(qa => 
            qa.IsAnswered && survey?.Questions.FirstOrDefault(q => q.Id == qa.QuestionId)?.IsRequired == true);
        
        var isComplete = answeredQuestions == totalQuestions && requiredAnsweredQuestions == requiredQuestions;
        
        // CanSubmit: Must be complete, can be modified, survey accepting, and not already submitted
        var canSubmit = isComplete && 
                       canContinue && 
                       !isSubmitted;

        // Determine status message based on actual response state using entity methods
        string statusMessage;
        if (isSubmitted)
        {
            statusMessage = "پاسخ با موفقیت ارسال شده است";
        }
        else if (isCanceled)
        {
            statusMessage = "پاسخ لغو شده است";
        }
        else if (isExpired)
        {
            statusMessage = "پاسخ منقضی شده است";
        }
        else if (isComplete)
        {
            statusMessage = "نظرسنجی تکمیل شده است. آماده ارسال می‌باشد";
        }
        else if (answeredQuestions == 0)
        {
            statusMessage = "هنوز شروع نشده است";
        }
        else
        {
            statusMessage = $"در حال تکمیل ({answeredQuestions}/{totalQuestions})";
        }

        return new ResponseStatusDto
        {
            ResponseStatus = response.Status.ToString(),
            ResponseStatusText = ResponseStatusHelper.GetPersianText(response.Status),
            CanContinue = canContinue,
            CanSubmit = canSubmit,
            IsSubmitted = isSubmitted,
            StatusMessage = statusMessage,
            ValidationErrors = new List<string>()
        };
    }

    private static string GetSurveyStateText(SurveyState state)
    {
        return state switch
        {
            SurveyState.Draft => "پیش‌نویس",
            SurveyState.Published => "فعال",
            SurveyState.Completed => "بسته شده",
            SurveyState.Archived => "آرشیو شده",
            _ => "نامشخص"
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
