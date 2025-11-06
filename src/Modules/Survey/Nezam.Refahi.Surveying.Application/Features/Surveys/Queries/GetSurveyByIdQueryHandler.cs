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

namespace Nezam.Refahi.Surveying.Application.Features.Surveys.Queries;

/// <summary>
/// Handler for GetSurveyByIdQuery
/// </summary>
public class GetSurveyByIdQueryHandler : IRequestHandler<GetSurveyByIdQuery, ApplicationResult<SurveyDto>>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ILogger<GetSurveyByIdQueryHandler> _logger;
    private readonly IMemberService _memberService;
    private readonly ParticipationRulesDomainService _participationRulesService;

    public GetSurveyByIdQueryHandler(
        ISurveyRepository surveyRepository,
        ILogger<GetSurveyByIdQueryHandler> logger,
        IMemberService memberService,
        ParticipationRulesDomainService participationRulesService)
    {
        _surveyRepository = surveyRepository;
        _logger = logger;
        _memberService = memberService;
        _participationRulesService = participationRulesService;
    }

    public async Task<ApplicationResult<SurveyDto>> Handle(GetSurveyByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Get survey with responses if user national number is provided
            Survey? survey;
            if (!string.IsNullOrWhiteSpace(request.UserNationalNumber))
            {
                survey = await _surveyRepository.GetWithResponsesAsync(request.SurveyId, cancellationToken);
            }
            else
            {
                survey = await _surveyRepository.GetByIdAsync(request.SurveyId, cancellationToken);
            }
            
            if (survey == null)
                return ApplicationResult<SurveyDto>.Failure("نظرسنجی یافت نشد");

            var surveyDto = MapToDto(survey);

            // Include questions if requested
            if (request.IncludeQuestions)
            {
                // Get user's latest response for question mapping
                Response? userLatestResponse = null;
                if (!string.IsNullOrWhiteSpace(request.UserNationalNumber))
                {
                    var nationalId = new NationalId(request.UserNationalNumber);
                    var memberDetail = await _memberService.GetMemberDetailByNationalCodeAsync(nationalId);
                    if (memberDetail != null)
                    {
                        var userResponses = survey.Responses
                            .Where(r => r.Participant.MemberId == memberDetail.Id)
                            .OrderByDescending(r => r.AttemptNumber)
                            .ToList();
                            
                        // Prioritize submitted responses for question mapping
                        userLatestResponse = userResponses
                            .Where(r => r.AttemptStatus == AttemptStatus.Submitted)
                            .OrderByDescending(r => r.SubmittedAt ?? DateTimeOffset.MinValue)
                            .FirstOrDefault() 
                            ?? userResponses.FirstOrDefault();
                    }
                }

                surveyDto.Questions = survey.Questions
                    .OrderBy(q => q.Order)
                    .Select(q => MapQuestionToDtoWithUserAnswers(q, userLatestResponse))
                    .ToList();

                surveyDto.TotalQuestions = surveyDto.Questions.Count;
                surveyDto.RequiredQuestions = surveyDto.Questions.Count(q => q.IsRequired);
            }

            // Include features and capabilities
            surveyDto.Features = survey.SurveyFeatures
                .Select(sf => new SurveyFeatureDto
                {
                    Id = sf.Id,
                    SurveyId = sf.SurveyId,
                    FeatureCode = sf.FeatureCode,
                    FeatureTitleSnapshot = sf.FeatureTitleSnapshot
                })
                .ToList();

            surveyDto.Capabilities = survey.SurveyCapabilities
                .Select(sc => new SurveyCapabilityDto
                {
                    Id = sc.Id,
                    SurveyId = sc.SurveyId,
                    CapabilityCode = sc.CapabilityCode,
                    CapabilityTitleSnapshot = sc.CapabilityTitleSnapshot
                })
                .ToList();

            // Check comprehensive participation eligibility
            if (!string.IsNullOrWhiteSpace(request.UserNationalNumber))
            {
                _logger.LogInformation("Processing survey {SurveyId} for user with national number {UserNationalNumber}", 
                    request.SurveyId, request.UserNationalNumber);
                
                // Get member info from national number
                var nationalId = new NationalId(request.UserNationalNumber);
                var memberDetail = await _memberService.GetMemberDetailByNationalCodeAsync(nationalId);
                
                if (memberDetail == null)
                {
                    _logger.LogWarning("Member not found for national number {UserNationalNumber}", request.UserNationalNumber);
                }
                else
                {
                    _logger.LogInformation("Found member {MemberId} for national number {UserNationalNumber}", 
                        memberDetail.Id, request.UserNationalNumber);
                }
                
                if (memberDetail != null)
                {
                    var userResponses = survey.Responses
                        .Where(r => r.Participant.MemberId == memberDetail.Id)
                        .OrderByDescending(r => r.AttemptNumber)
                        .ToList();

                    _logger.LogInformation("Found {ResponseCount} responses for member {MemberId} in survey {SurveyId}", 
                        userResponses.Count, memberDetail.Id, request.SurveyId);

                    surveyDto.ResponseCount = survey.Responses.Count;
                    surveyDto.IsAcceptingResponses = survey.IsAcceptingResponses();
                    
                    // Map user responses to DTOs
                    surveyDto.UserResponses = userResponses.Select(r => MapResponseToDto(r, survey)).ToList();
                    surveyDto.HasUserResponse = userResponses.Any();
                    
                    // Get latest response - prioritize submitted responses
                    var latestResponse = userResponses
                        .Where(r => r.AttemptStatus == AttemptStatus.Submitted)
                        .OrderByDescending(r => r.SubmittedAt ?? DateTimeOffset.MinValue)
                        .FirstOrDefault() 
                        ?? userResponses.FirstOrDefault();
                        
                    if (latestResponse != null)
                    {
                        surveyDto.UserLastResponse = MapResponseToDto(latestResponse, survey);
                        surveyDto.UserCompletionPercentage = CalculateCompletionPercentage(latestResponse, survey);
                        surveyDto.UserAnsweredQuestions = latestResponse.QuestionAnswers.Count(qa => qa.HasAnswer());
                        surveyDto.UserHasCompletedSurvey = latestResponse.AttemptStatus == AttemptStatus.Submitted;
                    }
                    
                    // Comprehensive eligibility check including capabilities and features
                    var eligibilityResult = await CheckComprehensiveEligibility(survey, memberDetail.Id, latestResponse, cancellationToken);
                    surveyDto.CanParticipate = eligibilityResult.CanParticipate;
                    surveyDto.CanUserParticipate = eligibilityResult.CanParticipate;
                    surveyDto.ParticipationMessage = eligibilityResult.Message;
                    
                    // Additional user-specific information
                    surveyDto.UserAttemptCount = userResponses.Count;
                    surveyDto.RemainingAttempts = Math.Max(0, survey.ParticipationPolicy.MaxAttemptsPerMember - userResponses.Count);
                    
                    // Detailed eligibility information
                    surveyDto.UserEligibility = await GetDetailedEligibilityInfo(survey, memberDetail.Id, latestResponse, cancellationToken);
                }
            }

            return ApplicationResult<SurveyDto>.Success(surveyDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting survey by ID {SurveyId}", request.SurveyId);
            return ApplicationResult<SurveyDto>.Failure(ex, "خطا در دریافت نظرسنجی");
        }
    }

    private static SurveyDto MapToDto(Survey survey)
    {
        return new SurveyDto
        {
            Id = survey.Id,
            Title = survey.Title,
            Description = survey.Description,
            State = survey.State.ToString(),
            StateText = GetSurveyStateText(survey.State),
            StartAt = survey.StartAt?.DateTime,
            EndAt = survey.EndAt?.DateTime,
            IsAnonymous = survey.IsAnonymous,
            CreatedAt = survey.CreatedAt,
            MaxAttemptsPerMember = survey.ParticipationPolicy.MaxAttemptsPerMember,
            AllowMultipleSubmissions = survey.ParticipationPolicy.AllowMultipleSubmissions,
            CoolDownSeconds = survey.ParticipationPolicy.CoolDownSeconds,
            AudienceFilter = survey.AudienceFilter?.FilterExpression
        };
    }

    private static QuestionDto MapQuestionToDto(Question question)
    {
        return new QuestionDto
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

    private static QuestionDto MapQuestionToDtoWithUserAnswers(Question question, Response? userResponse)
    {
        var questionDto = MapQuestionToDto(question);
        
        // Add user-specific answer data
        if (userResponse != null)
        {
            var questionAnswer = userResponse.QuestionAnswers.FirstOrDefault(qa => qa.QuestionId == question.Id);
            
            if (questionAnswer != null && questionAnswer.HasAnswer())
            {
                // Create QuestionAnswerDto for user answer
                questionDto.UserAnswer = new QuestionAnswerDto
                {
                    Id = questionAnswer.Id,
                    QuestionId = questionAnswer.QuestionId,
                    TextAnswer = questionAnswer.TextAnswer,
                    SelectedOptions = questionAnswer.SelectedOptions.Select(so => new QuestionAnswerOptionDto
                    {
                        OptionId = so.OptionId,
                        OptionText = so.OptionText
                    }).ToList()
                };
                
                questionDto.IsAnswered = true;
                questionDto.IsComplete = true;
                
                // Mark selected options
                var selectedOptionIds = questionAnswer.SelectedOptions.Select(so => so.OptionId).ToHashSet();
                foreach (var option in questionDto.Options)
                {
                    option.IsSelected = selectedOptionIds.Contains(option.Id);
                }
            }
            else
            {
                questionDto.UserAnswer = null;
                questionDto.IsAnswered = false;
                questionDto.IsComplete = false;
            }
        }
        else
        {
            questionDto.UserAnswer = null;
            questionDto.IsAnswered = false;
            questionDto.IsComplete = false;
        }
        
        return questionDto;
    }

    private static Contracts.Dtos.QuestionOptionDto MapOptionToDto(QuestionOption option)
    {
        return new Contracts.Dtos.QuestionOptionDto
        {
            Id = option.Id,
            QuestionId = option.QuestionId,
            Text = option.Text,
            Order = option.Order,
            IsActive = option.IsActive
        };
    }

    /// <summary>
    /// Comprehensive eligibility check that includes all conditions from AnswerQuestionCommand
    /// </summary>
    private Task<(bool CanParticipate, string Message)> CheckComprehensiveEligibility(
        Survey survey, 
        Guid memberId, 
        Response? existingResponse, 
        CancellationToken cancellationToken)
    {
        // 1. Check survey state
        if (!survey.IsAcceptingResponses())
        {
            return Task.FromResult((false, "نظرسنجی در حال حاضر فعال نیست"));
        }

        // 2. Check time constraints
        var now = DateTimeOffset.UtcNow;
        if (survey.StartAt.HasValue && now < survey.StartAt.Value)
        {
            return Task.FromResult((false, "نظرسنجی هنوز شروع نشده است"));
        }

        if (survey.EndAt.HasValue && now > survey.EndAt.Value)
        {
            return Task.FromResult((false, "نظرسنجی به پایان رسیده است"));
        }

        // 3. Check member authorization (capabilities and features)
        // Note: We skip detailed capability/feature checks for now since we don't have direct user ID lookup
        // This can be enhanced later when we have a proper user ID to national number mapping
        
        // 4. Check attempt limits and participation rules
        if (existingResponse != null)
        {
            // Check if user has reached max attempts
            if (existingResponse.AttemptNumber >= survey.ParticipationPolicy.MaxAttemptsPerMember)
            {
                return Task.FromResult((false, "حداکثر تعداد تلاش‌های مجاز را انجام داده‌اید"));
            }

            // Check if multiple submissions are allowed
            if (!survey.ParticipationPolicy.AllowMultipleSubmissions)
            {
                return Task.FromResult((false, "شما قبلاً در این نظرسنجی شرکت کرده‌اید"));
            }

            // Check cooldown period
            if (survey.ParticipationPolicy.CoolDownSeconds.HasValue && existingResponse.SubmittedAt.HasValue)
            {
                var cooldownEnd = existingResponse.SubmittedAt.Value.AddSeconds(survey.ParticipationPolicy.CoolDownSeconds.Value);
                if (now < cooldownEnd)
                {
                    var remainingSeconds = (int)(cooldownEnd - now).TotalSeconds;
                    return Task.FromResult((false, $"لطفاً {remainingSeconds} ثانیه صبر کنید"));
                }
            }
        }

        // 5. Check if survey is accepting responses
        if (!survey.IsAcceptingResponses())
        {
            return Task.FromResult((false, "نظرسنجی در حال حاضر پاسخ‌ها را نمی‌پذیرد"));
        }

        // All checks passed
        return Task.FromResult((true, "شما می‌توانید در این نظرسنجی شرکت کنید"));
    }

    /// <summary>
    /// Gets detailed eligibility information for comprehensive user participation check
    /// </summary>
    private Task<UserEligibilityInfo> GetDetailedEligibilityInfo(
        Survey survey, 
        Guid memberId, 
        Response? existingResponse, 
        CancellationToken cancellationToken)
    {
        var eligibility = new UserEligibilityInfo
        {
            SurveyStartTime = survey.StartAt?.DateTime,
            SurveyEndTime = survey.EndAt?.DateTime,
            CurrentAttempts = existingResponse?.AttemptNumber ?? 0,
            MaxAllowedAttempts = survey.ParticipationPolicy.MaxAttemptsPerMember,
            RemainingAttempts = Math.Max(0, survey.ParticipationPolicy.MaxAttemptsPerMember - (existingResponse?.AttemptNumber ?? 0))
        };

        // 1. Check survey state
        eligibility.IsSurveyActive = survey.IsAcceptingResponses();

        // 2. Check time constraints
        var now = DateTimeOffset.UtcNow;
        eligibility.IsWithinTimeWindow = true;
        
        if (survey.StartAt.HasValue && now < survey.StartAt.Value)
        {
            eligibility.IsWithinTimeWindow = false;
        }
        if (survey.EndAt.HasValue && now > survey.EndAt.Value)
        {
            eligibility.IsWithinTimeWindow = false;
        }

        // 3. Check member authorization (capabilities and features)
        try
        {
            // Get member info using user ID - we need to get national number first
            // Since we don't have a direct method to get member by ID, we'll skip this check for now
            // and rely on the basic eligibility checks
            eligibility.HasRequiredCapabilities = true; // Assume true for now
            eligibility.HasRequiredFeatures = true; // Assume true for now
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error checking member authorization for member {MemberId}", memberId);
            eligibility.HasRequiredCapabilities = false;
            eligibility.HasRequiredFeatures = false;
        }

        // 4. Check attempt limits and participation rules
        eligibility.WithinAttemptLimit = true;
        eligibility.CanSubmitMultiple = survey.ParticipationPolicy.AllowMultipleSubmissions;
        eligibility.NotInCooldown = true;

        if (existingResponse != null)
        {
            // Check if user has reached max attempts
            if (existingResponse.AttemptNumber >= survey.ParticipationPolicy.MaxAttemptsPerMember)
            {
                eligibility.WithinAttemptLimit = false;
            }

            // Check if multiple submissions are allowed
            if (!survey.ParticipationPolicy.AllowMultipleSubmissions)
            {
                eligibility.CanSubmitMultiple = false;
            }

            // Check cooldown period
            if (survey.ParticipationPolicy.CoolDownSeconds.HasValue && existingResponse.SubmittedAt.HasValue)
            {
                var cooldownEnd = existingResponse.SubmittedAt.Value.AddSeconds(survey.ParticipationPolicy.CoolDownSeconds.Value);
                eligibility.CooldownEndTime = cooldownEnd.DateTime;
                
                if (now < cooldownEnd)
                {
                    eligibility.NotInCooldown = false;
                    eligibility.RemainingCooldownSeconds = (int)(cooldownEnd - now).TotalSeconds;
                }
            }
        }

        // 5. Determine overall eligibility
        eligibility.CanParticipate = eligibility.IsSurveyActive && 
                                   eligibility.IsWithinTimeWindow && 
                                   eligibility.HasRequiredCapabilities && 
                                   eligibility.WithinAttemptLimit && 
                                   eligibility.NotInCooldown &&
                                   survey.IsAcceptingResponses();

        // 6. Set appropriate message and error code
        if (eligibility.CanParticipate)
        {
            eligibility.Message = "شما می‌توانید در این نظرسنجی شرکت کنید";
            eligibility.ErrorCode = "ELIGIBLE";
        }
        else
        {
            if (!eligibility.IsSurveyActive)
            {
                eligibility.Message = "نظرسنجی در حال حاضر فعال نیست";
                eligibility.ErrorCode = "SURVEY_NOT_ACTIVE";
            }
            else if (!eligibility.IsWithinTimeWindow)
            {
                if (survey.StartAt.HasValue && now < survey.StartAt.Value)
                {
                    eligibility.Message = "نظرسنجی هنوز شروع نشده است";
                    eligibility.ErrorCode = "SURVEY_NOT_STARTED";
                }
                else if (survey.EndAt.HasValue && now > survey.EndAt.Value)
                {
                    eligibility.Message = "نظرسنجی به پایان رسیده است";
                    eligibility.ErrorCode = "SURVEY_ENDED";
                }
            }
            else if (!eligibility.HasRequiredCapabilities)
            {
                eligibility.Message = "شما مجاز به شرکت در این نظرسنجی نیستید";
                eligibility.ErrorCode = "INSUFFICIENT_CAPABILITIES";
            }
            else if (!eligibility.WithinAttemptLimit)
            {
                eligibility.Message = "حداکثر تعداد تلاش‌های مجاز را انجام داده‌اید";
                eligibility.ErrorCode = "MAX_ATTEMPTS_REACHED";
            }
            else if (!eligibility.CanSubmitMultiple && existingResponse != null)
            {
                eligibility.Message = "شما قبلاً در این نظرسنجی شرکت کرده‌اید";
                eligibility.ErrorCode = "ALREADY_PARTICIPATED";
            }
            else if (!eligibility.NotInCooldown)
            {
                eligibility.Message = $"لطفاً {eligibility.RemainingCooldownSeconds} ثانیه صبر کنید";
                eligibility.ErrorCode = "COOLDOWN_ACTIVE";
            }
            else
            {
                eligibility.Message = "نظرسنجی در حال حاضر پاسخ‌ها را نمی‌پذیرد";
                eligibility.ErrorCode = "NOT_ACCEPTING_RESPONSES";
            }
        }

        return Task.FromResult(eligibility);
    }

    private static bool CanUserParticipate(Survey survey, Response? existingResponse, Guid userId)
    {
        // Check if survey is accepting responses
        if (!survey.IsAcceptingResponses())
            return false;

        // Check time constraints
        if (survey.StartAt.HasValue && DateTimeOffset.UtcNow < survey.StartAt.Value)
            return false;

        if (survey.EndAt.HasValue && DateTimeOffset.UtcNow > survey.EndAt.Value)
            return false;

        // Check attempt limits
        if (existingResponse != null)
        {
            if (!survey.ParticipationPolicy.AllowMultipleSubmissions)
                return false;

            if (existingResponse.AttemptNumber >= survey.ParticipationPolicy.MaxAttemptsPerMember)
                return false;

            // Check cooldown
            if (survey.ParticipationPolicy.CoolDownSeconds.HasValue)
            {
                var cooldownEnd = existingResponse.SubmittedAt?.AddSeconds(survey.ParticipationPolicy.CoolDownSeconds.Value);
                if (cooldownEnd.HasValue && DateTimeOffset.UtcNow < cooldownEnd.Value)
                    return false;
            }
        }

        return true;
    }

    private static string GetParticipationMessage(Survey survey, Response? existingResponse)
    {
        if (!survey.IsAcceptingResponses())
            return "نظرسنجی در حال حاضر فعال نیست";

        if (survey.StartAt.HasValue && DateTime.UtcNow < survey.StartAt.Value)
            return "نظرسنجی هنوز شروع نشده است";

        if (survey.EndAt.HasValue && DateTime.UtcNow > survey.EndAt.Value)
            return "نظرسنجی به پایان رسیده است";

        if (existingResponse != null)
        {
            if (!survey.ParticipationPolicy.AllowMultipleSubmissions)
                return "شما قبلاً در این نظرسنجی شرکت کرده‌اید";

            if (existingResponse.AttemptNumber >= survey.ParticipationPolicy.MaxAttemptsPerMember)
                return "حداکثر تعداد تلاش‌های مجاز را انجام داده‌اید";

            if (survey.ParticipationPolicy.CoolDownSeconds.HasValue)
            {
                var cooldownEnd = existingResponse.SubmittedAt?.AddSeconds(survey.ParticipationPolicy.CoolDownSeconds.Value);
                if (cooldownEnd.HasValue && DateTimeOffset.UtcNow < cooldownEnd.Value)
                    return $"لطفاً {survey.ParticipationPolicy.CoolDownSeconds} ثانیه صبر کنید";
            }
        }

        return "شما می‌توانید در این نظرسنجی شرکت کنید";
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

    private static ResponseDto MapResponseToDto(Response response, Survey survey)
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

    private static string GetAttemptStatusText(AttemptStatus status) => status switch
    {
        AttemptStatus.Active => "فعال",
        AttemptStatus.Submitted => "ارسال شده",
        AttemptStatus.Canceled => "لغو شده",
        AttemptStatus.Expired => "منقضی شده",
        _ => "نامشخص"
    };

    private static decimal CalculateCompletionPercentage(Response response, Survey survey)
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

    private static string GetNextActionText(Response response, Survey survey)
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
}
