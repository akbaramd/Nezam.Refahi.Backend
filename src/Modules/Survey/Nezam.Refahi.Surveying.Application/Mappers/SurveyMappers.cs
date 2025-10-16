using Nezam.Refahi.Surveying.Contracts.Dtos;
using Nezam.Refahi.Surveying.Domain.Entities;
using Nezam.Refahi.Surveying.Domain.Enums;
using Nezam.Refahi.Surveying.Domain.ValueObjects;

namespace Nezam.Refahi.Surveying.Application.Mappers;

/// <summary>
/// Professional extension methods for mapping Domain entities to DTOs
/// Provides both simple and detailed mapping options with proper error handling
/// </summary>
public static class SurveyMappers
{
    #region Survey Mappings

    /// <summary>
    /// Maps Survey entity to simple SurveyDto (for lists)
    /// </summary>
    public static SurveyDto ToDto(this Survey survey, Response? userResponse = null)
    {
        ArgumentNullException.ThrowIfNull(survey);

        return new SurveyDto
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
            AudienceFilter = survey.AudienceFilter?.ToString(),
            HasAudienceFilter = survey.AudienceFilter != null,
            Questions = survey.Questions.Select(q => q.ToDto()).ToList(),
            Features = survey.SurveyFeatures.Select(f => f.ToDto()).ToList(),
            Capabilities = survey.SurveyCapabilities.Select(c => c.ToDto()).ToList(),
            UserLastResponse = userResponse?.ToDto(),
            UserResponses = GetUserResponses(survey, userResponse),
            HasUserResponse = userResponse != null,
            CanUserParticipate = survey.IsAcceptingResponses(),
            CanParticipate = survey.IsAcceptingResponses(), // Legacy
            UserAttemptCount = GetUserAttemptCount(survey, userResponse),
            RemainingAttempts = GetRemainingAttempts(survey, userResponse),
            TotalQuestions = survey.Questions.Count,
            RequiredQuestions = survey.Questions.Count(q => q.IsRequired),
            ResponseCount = survey.Responses.Count,
            UniqueParticipantCount = GetUniqueParticipantCount(survey),
            IsAcceptingResponses = survey.IsAcceptingResponses(),
            IsAcceptingResponsesText = survey.IsAcceptingResponses() ? "در حال پذیرش پاسخ" : "غیرفعال",
            Duration = GetDuration(survey),
            DurationText = GetDurationText(survey.StartAt, survey.EndAt),
            TimeRemaining = GetTimeRemaining(survey.EndAt),
            TimeRemainingText = GetTimeRemainingText(survey.EndAt),
            IsExpired = IsExpired(survey),
            IsScheduled = survey.State == SurveyState.Scheduled,
            IsActive = survey.State == SurveyState.Active,
            UserCompletionPercentage = CalculateCompletionPercentage(userResponse, survey),
            UserAnsweredQuestions = GetUserAnsweredQuestions(userResponse),
            UserHasCompletedSurvey = userResponse?.SubmittedAt.HasValue ?? false
        };
    }

    /// <summary>
    /// Maps Survey entity to detailed SurveyDetailsDto (for single queries)
    /// </summary>
    public static SurveyDetailsDto ToDetailsDto(this Survey survey, Response? userResponse = null)
    {
        ArgumentNullException.ThrowIfNull(survey);

        return new SurveyDetailsDto
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
            StructureVersion = survey.StructureVersion,
            ParticipationPolicy = survey.ParticipationPolicy.ToDto(),
            AudienceFilter = survey.AudienceFilter?.ToDto(),
            Questions = survey.Questions.Select(q => q.ToDetailsDto()).ToList(),
            Features = survey.SurveyFeatures.Select(f => f.ToDto()).ToList(),
            Capabilities = survey.SurveyCapabilities.Select(c => c.ToDto()).ToList(),
            UserLastResponse = userResponse?.ToDetailsDto(),
            UserResponses = GetUserResponsesDetails(survey, userResponse),
            UserParticipation = GetUserParticipationInfo(survey, userResponse),
            Statistics = GetSurveyStatistics(survey),
            Timing = GetSurveyTiming(survey),
            Status = GetSurveyStatus(survey)
        };
    }

    #endregion

    #region Question Mappings

    /// <summary>
    /// Maps Question entity to simple QuestionDto (for lists)
    /// </summary>
    public static QuestionDto ToDto(this Question question, QuestionAnswer? userAnswer = null)
    {
        ArgumentNullException.ThrowIfNull(question);

        return new QuestionDto
        {
            Id = question.Id,
            SurveyId = question.SurveyId,
            Kind = question.Kind.ToString(),
            KindText = GetQuestionKindText(question.Kind),
            Text = question.Text,
            Order = question.Order,
            IsRequired = question.IsRequired,
            IsRequiredText = question.IsRequired ? "اجباری" : "اختیاری",
            Options = question.Options.Select(o => o.ToDto()).ToList(),
            UserAnswer = userAnswer?.ToDto(),
            IsAnswered = userAnswer?.HasAnswer() ?? false,
            IsComplete = userAnswer?.HasAnswer() ?? false
        };
    }

    /// <summary>
    /// Maps Question entity to detailed QuestionDetailsDto (for single queries)
    /// </summary>
    public static QuestionDetailsDto ToDetailsDto(this Question question, List<QuestionAnswer>? userAnswers = null)
    {
        ArgumentNullException.ThrowIfNull(question);

        var answers = userAnswers ?? new List<QuestionAnswer>();
        return new QuestionDetailsDto
        {
            Id = question.Id,
            SurveyId = question.SurveyId,
            Kind = question.Kind.ToString(),
            KindText = GetQuestionKindText(question.Kind),
            Text = question.Text,
            Order = question.Order,
            IsRequired = question.IsRequired,
            IsRequiredText = question.IsRequired ? "اجباری" : "اختیاری",
            RepeatPolicy = question.RepeatPolicy.ToDto(),
            Specification = new QuestionSpecificationDto(), // TODO: Map from question specification when available
            Options = question.Options.Select(o => o.ToDto()).ToList(),
            UserAnswers = answers.Select(a => a.ToDetailsDto()).ToList(),
            LatestUserAnswer = answers.OrderByDescending(a => a.Id).FirstOrDefault()?.ToDetailsDto(),
            IsAnswered = answers.Any(a => a.HasAnswer()),
            IsComplete = answers.Any(a => a.HasAnswer()),
            AnswerCount = answers.Count,
            Statistics = GetQuestionStatistics(question, answers),
            Validation = GetQuestionValidation(question, answers)
        };
    }

    #endregion

    #region Response Mappings

    /// <summary>
    /// Maps Response entity to simple ResponseDto (for lists)
    /// </summary>
    public static ResponseDto ToDto(this Response response)
    {
        ArgumentNullException.ThrowIfNull(response);

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
            IsAnonymousText = response.Participant.IsAnonymous ? "ناشناس" : "شناسایی شده",
            AttemptStatus = response.AttemptStatus.ToString(),
            AttemptStatusText = GetAttemptStatusText(response.AttemptStatus),
            IsActive = response.AttemptStatus == AttemptStatus.Active,
            IsSubmitted = response.AttemptStatus == AttemptStatus.Submitted,
            IsExpired = response.AttemptStatus == AttemptStatus.Expired,
            IsCanceled = response.AttemptStatus == AttemptStatus.Canceled,
            QuestionAnswers = response.QuestionAnswers.Select(qa => qa.ToDto()).ToList(),
            LastAnswers = response.QuestionAnswers.Where(qa => qa.HasAnswer()).Select(qa => qa.ToDto()).ToList(),
            TotalQuestions = 0, // Will be set by caller
            AnsweredQuestions = response.QuestionAnswers.Count(qa => qa.HasAnswer()),
            RequiredQuestions = 0, // Will be set by caller
            RequiredAnsweredQuestions = 0, // Will be set by caller
            IsComplete = false, // Will be set by caller
            IsCompleteText = "ناقص",
            CompletionPercentage = 0, // Will be set by caller
            CompletionPercentageText = "0%",
            ResponseDuration = GetResponseDuration(response),
            ResponseDurationText = GetResponseDurationText(response.SubmittedAt)
        };
    }

    /// <summary>
    /// Maps Response entity to detailed ResponseDetailsDto (for single queries)
    /// </summary>
    public static ResponseDetailsDto ToDetailsDto(this Response response)
    {
        ArgumentNullException.ThrowIfNull(response);

        return new ResponseDetailsDto
        {
            Id = response.Id,
            SurveyId = response.SurveyId,
            AttemptNumber = response.AttemptNumber,
 
            Participant = response.Participant.ToDto(),
            Survey = null, // Will be set by caller
            QuestionAnswers = response.QuestionAnswers.Select(qa => qa.ToDetailsDto()).ToList(),
            Statistics = GetResponseStatistics(response),
            Status = GetResponseStatus(response)
        };
    }

    #endregion

    #region Helper Methods

    private static string GetStateText(SurveyState state)
    {
        return state switch
        {
            SurveyState.Draft => "پیش‌نویس",
            SurveyState.Scheduled => "زمان‌بندی شده",
            SurveyState.Active => "فعال",
            SurveyState.Closed => "بسته",
            SurveyState.Archived => "آرشیو شده",
            _ => "نامشخص"
        };
    }

    private static string GetQuestionKindText(QuestionKind kind)
    {
        return kind switch
        {
            QuestionKind.Textual => "متنی",
            QuestionKind.ChoiceSingle => "تک انتخابی",
            QuestionKind.ChoiceMulti => "چند انتخابی",
            QuestionKind.FixedMCQ4 => "چهار گزینه‌ای ثابت",
            _ => "نامشخص"
        };
    }

    private static string GetAttemptStatusText(AttemptStatus status)
    {
        return status switch
        {
            AttemptStatus.Active => "فعال",
            AttemptStatus.Submitted => "ارسال شده",
            AttemptStatus.Canceled => "لغو شده",
            AttemptStatus.Expired => "منقضی شده",
            _ => "نامشخص"
        };
    }

    private static string GetMaxAttemptsText(int maxAttempts)
    {
        return maxAttempts switch
        {
            1 => "یک بار",
            int.MaxValue => "نامحدود",
            _ => $"{maxAttempts} بار"
        };
    }

    private static string? GetCoolDownText(int? coolDownSeconds)
    {
        if (!coolDownSeconds.HasValue) return null;
        
        return coolDownSeconds.Value switch
        {
            0 => "بدون تاخیر",
            < 60 => $"{coolDownSeconds} ثانیه",
            < 3600 => $"{coolDownSeconds / 60} دقیقه",
            _ => $"{coolDownSeconds / 3600} ساعت"
        };
    }

    private static TimeSpan? GetDuration(Survey survey)
    {
        if (!survey.StartAt.HasValue || !survey.EndAt.HasValue) return null;
        return survey.EndAt.Value - survey.StartAt.Value;
    }

    private static string? GetDurationText(DateTimeOffset? startAt, DateTimeOffset? endAt)
    {
        if (!startAt.HasValue || !endAt.HasValue) return null;
        
        var duration = endAt.Value - startAt.Value;
        return duration.TotalDays >= 1 ? $"{duration.Days} روز" :
               duration.TotalHours >= 1 ? $"{duration.Hours} ساعت" :
               $"{duration.Minutes} دقیقه";
    }

    private static TimeSpan? GetTimeRemaining(DateTimeOffset? endAt)
    {
        if (!endAt.HasValue) return null;
        var remaining = endAt.Value - DateTimeOffset.UtcNow;
        return remaining > TimeSpan.Zero ? remaining : null;
    }

    private static string? GetTimeRemainingText(DateTimeOffset? endAt)
    {
        var remaining = GetTimeRemaining(endAt);
        if (!remaining.HasValue) return null;
        
        return remaining.Value.TotalDays >= 1 ? $"{remaining.Value.Days} روز باقی مانده" :
               remaining.Value.TotalHours >= 1 ? $"{remaining.Value.Hours} ساعت باقی مانده" :
               $"{remaining.Value.Minutes} دقیقه باقی مانده";
    }

    private static TimeSpan? GetResponseDuration(Response response)
    {
        if (!response.SubmittedAt.HasValue) return null;
        // This would need the actual start time, which isn't available in Response
        // For now, return null as we don't have enough data
        return null;
    }

    private static string? GetResponseDurationText(DateTimeOffset? submittedAt)
    {
        if (!submittedAt.HasValue) return null;
        return "محاسبه شده"; // Placeholder since we don't have start time
    }

    private static bool IsExpired(Survey survey)
    {
        return survey.EndAt.HasValue && DateTimeOffset.UtcNow > survey.EndAt.Value;
    }

    private static decimal CalculateCompletionPercentage(Response? response, Survey survey)
    {
        if (response == null || survey.Questions.Count == 0) return 0;
        var answeredCount = response.QuestionAnswers.Count(qa => qa.HasAnswer());
        return (decimal)answeredCount / survey.Questions.Count * 100;
    }

    private static int GetUserAnsweredQuestions(Response? response)
    {
        return response?.QuestionAnswers.Count(qa => qa.HasAnswer()) ?? 0;
    }

    private static int GetUserAttemptCount(Survey survey, Response? userResponse)
    {
        if (userResponse == null) return 0;
        return survey.Responses.Count(r => r.Participant.MemberId == userResponse.Participant.MemberId);
    }

    private static int GetRemainingAttempts(Survey survey, Response? userResponse)
    {
        if (userResponse == null) return survey.ParticipationPolicy.MaxAttemptsPerMember;
        var usedAttempts = GetUserAttemptCount(survey, userResponse);
        return Math.Max(0, survey.ParticipationPolicy.MaxAttemptsPerMember - usedAttempts);
    }

    private static int GetUniqueParticipantCount(Survey survey)
    {
        return survey.Responses
            .Where(r => r.Participant.MemberId.HasValue)
            .Select(r => r.Participant.MemberId!.Value)
            .Distinct()
            .Count();
    }

    private static List<ResponseDto> GetUserResponses(Survey survey, Response? userResponse)
    {
        if (userResponse == null) return new List<ResponseDto>();
        return survey.Responses
            .Where(r => r.Participant.MemberId == userResponse.Participant.MemberId)
            .Select(r => r.ToDto())
            .ToList();
    }

    private static List<ResponseDetailsDto> GetUserResponsesDetails(Survey survey, Response? userResponse)
    {
        if (userResponse == null) return new List<ResponseDetailsDto>();
        return survey.Responses
            .Where(r => r.Participant.MemberId == userResponse.Participant.MemberId)
            .Select(r => r.ToDetailsDto())
            .ToList();
    }

    #endregion

    #region Complex Mapping Methods

    private static ParticipationPolicyDto ToDto(this ParticipationPolicy policy)
    {
        ArgumentNullException.ThrowIfNull(policy);

        return new ParticipationPolicyDto
        {
            MaxAttemptsPerMember = policy.MaxAttemptsPerMember,
            MaxAttemptsText = GetMaxAttemptsText(policy.MaxAttemptsPerMember),
            AllowMultipleSubmissions = policy.AllowMultipleSubmissions,
            AllowMultipleSubmissionsText = policy.AllowMultipleSubmissions ? "مجاز" : "غیرمجاز",
            CoolDownSeconds = policy.CoolDownSeconds,
            CoolDownText = GetCoolDownText(policy.CoolDownSeconds),
            AllowBackNavigation = policy.AllowBackNavigation,
            AllowBackNavigationText = policy.AllowBackNavigation ? "مجاز" : "غیرمجاز",
            RequireAllQuestions = false, // ParticipationPolicy doesn't have this property
            RequireAllQuestionsText = "اختیاری"
        };
    }

    private static AudienceFilterDto? ToDto(this AudienceFilter? filter)
    {
        if (filter == null) return null;
        
        return new AudienceFilterDto
        {
            FilterType = filter.GetType().Name,
            FilterTypeText = "فیلتر مخاطب",
            RequiredFeatures = new List<string>(), // TODO: Map from actual filter when available
            RequiredCapabilities = new List<string>(), // TODO: Map from actual filter when available
            ExcludedFeatures = new List<string>(), // TODO: Map from actual filter when available
            ExcludedCapabilities = new List<string>(), // TODO: Map from actual filter when available
            FilterCriteria = new Dictionary<string, object>() // TODO: Map from actual filter when available
        };
    }

    private static RepeatPolicyDto ToDto(this RepeatPolicy policy)
    {
        ArgumentNullException.ThrowIfNull(policy);

        return new RepeatPolicyDto
        {
            Kind = policy.Kind.ToString(),
            KindText = policy.Kind.ToString(),
            MaxRepeats = policy.MaxRepeats,
            MinRepeats = null, // RepeatPolicy doesn't have MinRepeats property
            IsRepeatable = policy.Kind != RepeatPolicyKind.None,
            RepeatDescription = GetRepeatDescription(policy)
        };
    }

    private static string GetRepeatDescription(RepeatPolicy policy)
    {
        return policy.Kind switch
        {
            RepeatPolicyKind.None => "غیرقابل تکرار",
            RepeatPolicyKind.Unbounded => "تکرار نامحدود",
            RepeatPolicyKind.Bounded => $"حداکثر {policy.MaxRepeats} تکرار",
            _ => "نامشخص"
        };
    }

    private static UserParticipationInfoDto GetUserParticipationInfo(Survey survey, Response? userResponse)
    {
        return new UserParticipationInfoDto
        {
            HasUserResponse = userResponse != null,
            CanUserParticipate = survey.IsAcceptingResponses(),
            CanParticipate = survey.IsAcceptingResponses(),
            UserAttemptCount = GetUserAttemptCount(survey, userResponse),
            RemainingAttempts = GetRemainingAttempts(survey, userResponse),
            LastAttemptAt = userResponse?.SubmittedAt?.DateTime,
            NextAttemptAllowedAt = null, // TODO: Calculate based on cooldown when available
            IsInCoolDown = false, // TODO: Calculate based on cooldown when available
            CoolDownRemaining = null, // TODO: Calculate based on cooldown when available
            CoolDownRemainingText = null // TODO: Calculate based on cooldown when available
        };
    }

    private static SurveyStatisticsDto GetSurveyStatistics(Survey survey)
    {
        return new SurveyStatisticsDto
        {
            TotalQuestions = survey.Questions.Count,
            RequiredQuestions = survey.Questions.Count(q => q.IsRequired),
            ResponseCount = survey.Responses.Count,
            UniqueParticipantCount = GetUniqueParticipantCount(survey),
            SubmittedResponseCount = survey.Responses.Count(r => r.SubmittedAt.HasValue),
            ActiveResponseCount = survey.Responses.Count(r => r.AttemptStatus == AttemptStatus.Active),
            CanceledResponseCount = survey.Responses.Count(r => r.AttemptStatus == AttemptStatus.Canceled),
            ExpiredResponseCount = survey.Responses.Count(r => r.AttemptStatus == AttemptStatus.Expired),
            AverageCompletionPercentage = 0, // TODO: Calculate when needed
            AverageResponseTime = 0, // TODO: Calculate when needed
            IsAcceptingResponses = survey.IsAcceptingResponses(),
            IsAcceptingResponsesText = survey.IsAcceptingResponses() ? "در حال پذیرش پاسخ" : "غیرفعال"
        };
    }

    private static SurveyTimingDto GetSurveyTiming(Survey survey)
    {
        return new SurveyTimingDto
        {
            Duration = GetDuration(survey),
            DurationText = GetDurationText(survey.StartAt, survey.EndAt),
            TimeRemaining = GetTimeRemaining(survey.EndAt),
            TimeRemainingText = GetTimeRemainingText(survey.EndAt),
            TimeElapsed = survey.StartAt.HasValue ? DateTimeOffset.UtcNow - survey.StartAt.Value : null,
            TimeElapsedText = null, // TODO: Format time elapsed when needed
            IsExpired = IsExpired(survey),
            IsScheduled = survey.State == SurveyState.Scheduled,
            IsActive = survey.State == SurveyState.Active,
            NextStateChangeAt = null, // TODO: Calculate next state change when needed
            NextStateChangeText = string.Empty
        };
    }

    private static SurveyStatusDto GetSurveyStatus(Survey survey)
    {
        return new SurveyStatusDto
        {
            IsDraft = survey.State == SurveyState.Draft,
            IsScheduled = survey.State == SurveyState.Scheduled,
            IsActive = survey.State == SurveyState.Active,
            IsClosed = survey.State == SurveyState.Closed,
            IsArchived = survey.State == SurveyState.Archived,
            CanBeEdited = survey.State == SurveyState.Draft,
            CanBeActivated = survey.State == SurveyState.Scheduled,
            CanBeClosed = survey.State == SurveyState.Active,
            CanBeArchived = survey.State == SurveyState.Closed,
            AvailableActions = GetAvailableActions(survey),
            StatusMessage = GetStatusMessage(survey)
        };
    }

    private static List<string> GetAvailableActions(Survey survey)
    {
        var actions = new List<string>();
        
        switch (survey.State)
        {
            case SurveyState.Draft:
                actions.Add("ویرایش");
                actions.Add("فعال‌سازی");
                break;
            case SurveyState.Scheduled:
                actions.Add("فعال‌سازی");
                actions.Add("ویرایش");
                break;
            case SurveyState.Active:
                actions.Add("بستن");
                break;
            case SurveyState.Closed:
                actions.Add("آرشیو");
                break;
        }
        
        return actions;
    }

    private static string GetStatusMessage(Survey survey)
    {
        return survey.State switch
        {
            SurveyState.Draft => "نظرسنجی در حالت پیش‌نویس است",
            SurveyState.Scheduled => "نظرسنجی زمان‌بندی شده است",
            SurveyState.Active => "نظرسنجی فعال است",
            SurveyState.Closed => "نظرسنجی بسته شده است",
            SurveyState.Archived => "نظرسنجی آرشیو شده است",
            _ => "وضعیت نامشخص"
        };
    }

    private static QuestionStatisticsDto GetQuestionStatistics(Question question, List<QuestionAnswer> answers)
    {
        return new QuestionStatisticsDto
        {
            TotalAnswers = answers.Count,
            RequiredAnswers = question.IsRequired ? answers.Count : 0,
            OptionalAnswers = question.IsRequired ? 0 : answers.Count,
            AnswerRate = 0, // TODO: Calculate when needed
            CompletionRate = 0, // TODO: Calculate when needed
            AverageAnswerTime = null, // TODO: Calculate when needed
            AverageAnswerTimeText = null,
            OptionSelectionCounts = new Dictionary<string, int>(), // TODO: Calculate when needed
            CommonTextAnswers = new List<string>() // TODO: Calculate when needed
        };
    }

    private static QuestionValidationDto GetQuestionValidation(Question question, List<QuestionAnswer> answers)
    {
        return new QuestionValidationDto
        {
            IsValid = true, // TODO: Validate when needed
            ValidationErrors = new List<string>(),
            ValidationWarnings = new List<string>(),
            HasRequiredValidation = question.IsRequired,
            HasLengthValidation = false, // TODO: Check specification when available
            HasPatternValidation = false, // TODO: Check specification when available
            ValidationMessage = string.Empty
        };
    }

    private static ResponseStatisticsDto GetResponseStatistics(Response response)
    {
        return new ResponseStatisticsDto
        {
            TotalQuestions = 0, // Will be set by caller
            AnsweredQuestions = response.QuestionAnswers.Count(qa => qa.HasAnswer()),
            RequiredQuestions = 0, // Will be set by caller
            RequiredAnsweredQuestions = 0, // Will be set by caller
            CompletionPercentage = 0, // Will be set by caller
            IsComplete = false, // Will be set by caller
            TimeSpent = GetResponseDuration(response),
            FirstAnswerAt = null, // TODO: Calculate when needed
            LastAnswerAt = null // TODO: Calculate when needed
        };
    }

    private static ResponseStatusDto GetResponseStatus(Response response)
    {
        return new ResponseStatusDto
        {
            CanContinue = response.AttemptStatus == AttemptStatus.Active,
            CanSubmit = response.AttemptStatus == AttemptStatus.Active,
            IsSubmitted = response.AttemptStatus == AttemptStatus.Submitted,
            StatusMessage = GetAttemptStatusText(response.AttemptStatus),
            ValidationErrors = new List<string>()
        };
    }

    #endregion
}