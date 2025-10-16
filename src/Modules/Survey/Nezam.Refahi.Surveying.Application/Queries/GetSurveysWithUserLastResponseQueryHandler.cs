using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;
using Nezam.Refahi.Surveying.Contracts.Dtos;
using Nezam.Refahi.Surveying.Contracts.Queries;
using Nezam.Refahi.Surveying.Domain.Entities;
using Nezam.Refahi.Surveying.Domain.Enums;
using Nezam.Refahi.Surveying.Domain.Repositories;
using Nezam.Refahi.Surveying.Domain.Services;
using Nezam.Refahi.Surveying.Domain.ValueObjects;

namespace Nezam.Refahi.Surveying.Application.Queries;

/// <summary>
/// Handler for getting surveys with user's last response information
/// </summary>
public class GetSurveysWithUserLastResponseQueryHandler : IRequestHandler<GetSurveysWithUserLastResponseQuery, ApplicationResult<SurveysWithUserLastResponseResponse>>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ILogger<GetSurveysWithUserLastResponseQueryHandler> _logger;
    private readonly IMemberInfoService _memberInfoService;

    public GetSurveysWithUserLastResponseQueryHandler(
        ISurveyRepository surveyRepository,
        ILogger<GetSurveysWithUserLastResponseQueryHandler> logger,
        IMemberInfoService memberInfoService)
    {
        _surveyRepository = surveyRepository;
        _logger = logger;
        _memberInfoService = memberInfoService;
    }

    public async Task<ApplicationResult<SurveysWithUserLastResponseResponse>> Handle(
        GetSurveysWithUserLastResponseQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var startTime = DateTimeOffset.UtcNow;
            
            // Get member info if national number is provided
            Guid? memberId = null;
            if (!string.IsNullOrWhiteSpace(request.NationalNumber))
            {
                var nationalId = new NationalId(request.NationalNumber);
                var memberInfo = await _memberInfoService.GetMemberInfoAsync(nationalId);
                if (memberInfo != null)
                {
                    memberId = memberInfo.Id;
                }
            }
            
            // Use repository method for complex query with pagination and filtering
            var (surveys, totalCount) = await _surveyRepository.GetSurveysWithPaginationAsync(
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                searchTerm: request.SearchTerm,
                state: !string.IsNullOrWhiteSpace(request.State) && Enum.TryParse<SurveyState>(request.State, true, out var state) ? state : null,
                isAcceptingResponses: request.IsAcceptingResponses,
                sortBy: !string.IsNullOrWhiteSpace(request.SortBy) ? request.SortBy : "CreatedAt",
                sortDirection: !string.IsNullOrWhiteSpace(request.SortDirection) ? request.SortDirection : "Desc",
                includeQuestions: request.IncludeQuestions,
                includeResponses: false, // We'll get user responses separately
                cancellationToken: cancellationToken);

            // Get user's last response for each survey if member ID is available
            var userLastResponses = new Dictionary<Guid, Response>();
            if (memberId.HasValue && request.IncludeUserLastResponse)
            {
                // Load surveys with responses to get user's responses
                var (surveysWithResponses, _) = await _surveyRepository.GetSurveysWithUserResponsesAsync(
                    memberId.Value, 1, int.MaxValue, string.Empty, null, null, null, null, null, null, null, null, "CreatedAt", "Desc", true, true, true, cancellationToken);
                
                foreach (var survey in surveysWithResponses)
                {
                    var userResponse = survey.Responses
                        .Where(r => r.Participant.MemberId == memberId.Value)
                        .OrderByDescending(r => r.AttemptNumber)
                        .FirstOrDefault();
                    
                    if (userResponse != null)
                    {
                        userLastResponses[survey.Id] = userResponse;
                    }
                }
            }

            // Convert to DTOs
            var surveyDtos = new List<SurveyDto>();
            foreach (var survey in surveys)
            {
                var surveyDto = await MapToSurveyDto(survey, request, userLastResponses, memberId);
                surveyDtos.Add(surveyDto);
            }

            var response = new SurveysWithUserLastResponseResponse
            {
                Surveys = surveyDtos,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
                HasNextPage = request.PageNumber < (int)Math.Ceiling((double)totalCount / request.PageSize),
                HasPreviousPage = request.PageNumber > 1,
                QueryExecutionTime = DateTimeOffset.UtcNow - startTime,
                QueryExecutedAt = DateTimeOffset.UtcNow.DateTime
            };

            // Calculate user participation summary
            if (memberId.HasValue)
            {
                response.UserParticipatedSurveys = surveyDtos.Count(s => s.HasUserResponse);
                response.UserCompletedSurveys = surveyDtos.Count(s => s.UserHasCompletedSurvey);
                response.UserActiveSurveys = surveyDtos.Count(s => s.UserLastResponse?.IsActive == true);
                response.UserAvailableSurveys = surveyDtos.Count(s => s.CanUserParticipate);
            }

            return ApplicationResult<SurveysWithUserLastResponseResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting surveys with user last response");
            return ApplicationResult<SurveysWithUserLastResponseResponse>.Failure("خطا در دریافت لیست نظرسنجی‌ها");
        }
    }

    private Task<SurveyDto> MapToSurveyDto(Survey survey, GetSurveysWithUserLastResponseQuery request, Dictionary<Guid, Response> userLastResponses, Guid? memberId)
    {
        var dto = new SurveyDto
        {
            Id = survey.Id,
            Title = survey.Title,
            Description = survey.Description,
            State = survey.State.ToString(),
            StateText = GetStateText(survey.State),
            StartAt = survey.StartAt?.DateTime,
            EndAt = survey.EndAt?.DateTime,
            CreatedAt = survey.CreatedAt,
            LastModifiedAt = survey.LastModifiedAt,
            IsAnonymous = survey.IsAnonymous,
            IsStructureFrozen = survey.IsStructureFrozen,
            MaxAttemptsPerMember = survey.ParticipationPolicy.MaxAttemptsPerMember,
            MaxAttemptsText = GetMaxAttemptsText(survey.ParticipationPolicy.MaxAttemptsPerMember),
            AllowMultipleSubmissions = survey.ParticipationPolicy.AllowMultipleSubmissions,
            AllowMultipleSubmissionsText = survey.ParticipationPolicy.AllowMultipleSubmissions ? "مجاز" : "غیرمجاز",
            CoolDownSeconds = survey.ParticipationPolicy.CoolDownSeconds,
            CoolDownText = GetCoolDownText(survey.ParticipationPolicy.CoolDownSeconds),
            AllowBackNavigation = survey.ParticipationPolicy.AllowBackNavigation,
            AllowBackNavigationText = survey.ParticipationPolicy.AllowBackNavigation ? "مجاز" : "غیرمجاز",
            AudienceFilter = survey.AudienceFilter?.FilterExpression,
            HasAudienceFilter = survey.AudienceFilter != null && !survey.AudienceFilter.IsEmpty(),
            TotalQuestions = survey.Questions.Count,
            RequiredQuestions = survey.Questions.Count(q => q.IsRequired),
            ResponseCount = survey.Responses.Count,
            UniqueParticipantCount = survey.Responses.Select(r => r.Participant.MemberId).Distinct().Count(),
            IsAcceptingResponses = survey.IsAcceptingResponses(),
            IsAcceptingResponsesText = survey.IsAcceptingResponses() ? "در حال پذیرش پاسخ" : "غیرفعال",
            IsExpired = survey.EndAt.HasValue && survey.EndAt <= DateTimeOffset.UtcNow,
            IsScheduled = survey.StartAt.HasValue && survey.StartAt > DateTimeOffset.UtcNow,
            IsActive = survey.State == SurveyState.Active
        };

        // Calculate duration and time remaining
        if (survey.StartAt.HasValue && survey.EndAt.HasValue)
        {
            dto.Duration = survey.EndAt.Value - survey.StartAt.Value;
            dto.DurationText = GetDurationText(dto.Duration.Value);
            
            if (survey.State == SurveyState.Active && survey.EndAt > DateTimeOffset.UtcNow)
            {
                dto.TimeRemaining = survey.EndAt.Value - DateTimeOffset.UtcNow;
                dto.TimeRemainingText = GetDurationText(dto.TimeRemaining.Value);
            }
        }

        // Include questions if requested
        if (request.IncludeQuestions)
        {
            dto.Questions = survey.Questions.Select(q => new QuestionDto
            {
                Id = q.Id,
                Text = q.Text,
                Kind = q.Kind.ToString(),
                KindText = GetQuestionKindText(q.Kind),
                Order = q.Order,
                IsRequired = q.IsRequired,
                IsRequiredText = q.IsRequired ? "اجباری" : "اختیاری",
                Options = q.Options.Select(o => new QuestionOptionDto
                {
                    Id = o.Id,
                    Text = o.Text,
                    Order = o.Order,
                    IsActive = o.IsActive
                }).ToList()
            }).ToList();
        }

        // Include user's last response if available
        if (memberId.HasValue && request.IncludeUserLastResponse && 
            userLastResponses.TryGetValue(survey.Id, out var lastResponse))
        {
            dto.HasUserResponse = true;
            dto.UserLastResponse = MapToResponseDto(lastResponse, survey);
            dto.UserCompletionPercentage = dto.UserLastResponse.CompletionPercentage;
            dto.UserAnsweredQuestions = dto.UserLastResponse.AnsweredQuestions;
            dto.UserHasCompletedSurvey = dto.UserLastResponse.IsSubmitted;

            // Get all user responses for this survey to calculate attempt count
            var allUserResponses = survey.Responses
                .Where(r => r.Participant.MemberId == memberId.Value)
                .ToList();
            dto.UserAttemptCount = allUserResponses.Count();
            dto.RemainingAttempts = Math.Max(0, survey.ParticipationPolicy.MaxAttemptsPerMember - allUserResponses.Count());

            // Check if user can participate
            var participant = ParticipantInfo.ForMember(memberId.Value);
            dto.CanUserParticipate = CanUserParticipate(survey, participant, allUserResponses);
            dto.CanParticipate = dto.CanUserParticipate; // Set legacy property
            dto.ParticipationMessage = GetParticipationMessage(survey, participant, allUserResponses);
        }
        else if (memberId.HasValue)
        {
            // User has no response for this survey
            dto.HasUserResponse = false;
            dto.CanUserParticipate = survey.IsAcceptingResponses();
            dto.CanParticipate = dto.CanUserParticipate; // Set legacy property
            dto.ParticipationMessage = survey.IsAcceptingResponses() ? "می‌توانید در این نظرسنجی شرکت کنید" : "نظرسنجی در حال حاضر فعال نیست";
            dto.UserAttemptCount = 0;
            dto.RemainingAttempts = survey.ParticipationPolicy.MaxAttemptsPerMember;
        }

        return Task.FromResult(dto);
    }

    private ResponseDto MapToResponseDto(Response response, Survey survey)
    {
        return new ResponseDto
        {
            Id = response.Id,
            SurveyId = response.SurveyId,
            AttemptNumber = response.AttemptNumber,
            SubmittedAt = response.SubmittedAt?.DateTime,
            ExpiredAt = response.ExpiredAt?.DateTime,
            CanceledAt = response.CanceledAt?.DateTime,
            ParticipantDisplayName = response.Participant.GetDisplayName(),
            ParticipantShortIdentifier = response.Participant.GetShortIdentifier(),
            IsAnonymous = response.Participant.IsAnonymous,
            IsAnonymousText = response.Participant.IsAnonymous ? "ناشناس" : "شناخته شده",
            AttemptStatus = response.AttemptStatus.ToString(),
            AttemptStatusText = GetAttemptStatusText(response.AttemptStatus),
            IsActive = response.AttemptStatus == AttemptStatus.Active,
            IsSubmitted = response.AttemptStatus == AttemptStatus.Submitted,
            IsExpired = response.AttemptStatus == AttemptStatus.Expired,
            IsCanceled = response.AttemptStatus == AttemptStatus.Canceled,
            QuestionAnswers = response.QuestionAnswers.Select(qa => new QuestionAnswerDto
            {
                Id = qa.Id,
                QuestionId = qa.QuestionId,
                TextAnswer = qa.TextAnswer,
                SelectedOptions = qa.SelectedOptions.Select(so => new QuestionAnswerOptionDto
                {
                    OptionId = so.OptionId,
                    OptionText = so.OptionText
                }).ToList()
            }).ToList(),
            TotalQuestions = survey.Questions.Count,
            AnsweredQuestions = response.QuestionAnswers.Count(qa => qa.HasAnswer()),
            RequiredQuestions = survey.Questions.Count(q => q.IsRequired),
            RequiredAnsweredQuestions = survey.Questions.Count(q => q.IsRequired && response.HasAnswerForQuestion(q.Id)),
            IsComplete = IsResponseComplete(survey, response),
            IsCompleteText = IsResponseComplete(survey, response) ? "کامل" : "ناقص",
            CompletionPercentage = CalculateCompletionPercentage(response, survey),
            CompletionPercentageText = $"{CalculateCompletionPercentage(response, survey):F1}%",
            SurveyTitle = survey.Title,
            SurveyDescription = survey.Description,
            CanContinue = response.AttemptStatus == AttemptStatus.Active,
            CanSubmit = response.AttemptStatus == AttemptStatus.Active && IsResponseComplete(survey, response),
            CanCancel = response.AttemptStatus == AttemptStatus.Active,
            NextActionText = GetNextActionText(response, survey)
        };
    }

    private string GetStateText(SurveyState state) => state switch
    {
        SurveyState.Draft => "پیش‌نویس",
        SurveyState.Scheduled => "زمان‌بندی شده",
        SurveyState.Active => "فعال",
        SurveyState.Closed => "بسته شده",
        SurveyState.Archived => "آرشیو شده",
        _ => "نامشخص"
    };

    private string GetAttemptStatusText(AttemptStatus status) => status switch
    {
        AttemptStatus.Active => "فعال",
        AttemptStatus.Submitted => "ارسال شده",
        AttemptStatus.Canceled => "لغو شده",
        AttemptStatus.Expired => "منقضی شده",
        _ => "نامشخص"
    };

    private string GetQuestionKindText(QuestionKind kind) => kind switch
    {
        QuestionKind.FixedMCQ4 => "چهار گزینه‌ای ثابت",
        QuestionKind.ChoiceSingle => "انتخاب تکی",
        QuestionKind.ChoiceMulti => "انتخاب چندگانه",
        QuestionKind.Textual => "متنی",
        _ => "نامشخص"
    };

    private string GetMaxAttemptsText(int maxAttempts) => maxAttempts switch
    {
        1 => "یک بار",
        int.MaxValue => "نامحدود",
        _ => $"{maxAttempts} بار"
    };

    private string GetCoolDownText(int? coolDownSeconds) => coolDownSeconds switch
    {
        null => "ندارد",
        0 => "ندارد",
        < 60 => $"{coolDownSeconds} ثانیه",
        < 3600 => $"{coolDownSeconds / 60} دقیقه",
        _ => $"{coolDownSeconds / 3600} ساعت"
    };

    private string GetDurationText(TimeSpan duration)
    {
        if (duration.TotalDays >= 1)
            return $"{(int)duration.TotalDays} روز و {duration.Hours} ساعت";
        if (duration.TotalHours >= 1)
            return $"{(int)duration.TotalHours} ساعت و {duration.Minutes} دقیقه";
        if (duration.TotalMinutes >= 1)
            return $"{(int)duration.TotalMinutes} دقیقه";
        return $"{duration.Seconds} ثانیه";
    }

    private string GetParticipationMessage(Survey survey, ParticipantInfo participant, List<Response> responses)
    {
        if (!survey.IsAcceptingResponses())
            return "نظرسنجی در حال حاضر فعال نیست";

        if (responses.Count >= survey.ParticipationPolicy.MaxAttemptsPerMember)
            return "حداکثر تعداد تلاش‌ها رسیده است";

        var lastResponse = responses.OrderByDescending(r => r.SubmittedAt ?? DateTimeOffset.MinValue).FirstOrDefault();
        if (lastResponse != null && survey.ParticipationPolicy.CoolDownSeconds.HasValue)
        {
            var timeSinceLastResponse = DateTimeOffset.UtcNow - (lastResponse.SubmittedAt ?? DateTimeOffset.MinValue);
            if (timeSinceLastResponse.TotalSeconds < survey.ParticipationPolicy.CoolDownSeconds.Value)
            {
                var remainingTime = TimeSpan.FromSeconds(survey.ParticipationPolicy.CoolDownSeconds.Value - timeSinceLastResponse.TotalSeconds);
                return $"لطفاً {GetDurationText(remainingTime)} صبر کنید";
            }
        }

        return "می‌توانید در این نظرسنجی شرکت کنید";
    }

    private string GetNextActionText(Response response, Survey survey)
    {
        return response.AttemptStatus switch
        {
            AttemptStatus.Active => IsResponseComplete(survey, response) ? "ارسال پاسخ" : "ادامه پاسخ‌دهی",
            AttemptStatus.Submitted => "پاسخ ارسال شده است",
            AttemptStatus.Canceled => "پاسخ لغو شده است",
            AttemptStatus.Expired => "پاسخ منقضی شده است",
            _ => "نامشخص"
        };
    }

    private decimal CalculateCompletionPercentage(Response response, Survey survey)
    {
        if (survey.Questions.Count == 0) return 100;
        var answeredQuestions = response.QuestionAnswers.Count(qa => qa.HasAnswer());
        return Math.Round((decimal)answeredQuestions / survey.Questions.Count * 100, 1);
    }

    private static bool IsResponseComplete(Survey survey, Response response)
    {
        // Check if all required questions are answered
        var requiredQuestions = survey.Questions.Where(q => q.IsRequired).ToList();
        return requiredQuestions.All(q => response.HasAnswerForQuestion(q.Id));
    }

    private static bool CanUserParticipate(Survey survey, ParticipantInfo participant, List<Response> responses)
    {
        if (!survey.IsAcceptingResponses())
            return false;

        if (responses.Count >= survey.ParticipationPolicy.MaxAttemptsPerMember)
            return false;

        var lastResponse = responses.OrderByDescending(r => r.SubmittedAt ?? DateTimeOffset.MinValue).FirstOrDefault();
        if (lastResponse != null && survey.ParticipationPolicy.CoolDownSeconds.HasValue)
        {
            var timeSinceLastResponse = DateTimeOffset.UtcNow - (lastResponse.SubmittedAt ?? DateTimeOffset.MinValue);
            if (timeSinceLastResponse.TotalSeconds < survey.ParticipationPolicy.CoolDownSeconds.Value)
                return false;
        }

        return true;
    }
}