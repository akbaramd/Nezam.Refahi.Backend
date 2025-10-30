using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;
using Nezam.Refahi.Surveying.Application.Services;
using Nezam.Refahi.Surveying.Contracts.Commands;
using Nezam.Refahi.Surveying.Contracts.Dtos;
using Nezam.Refahi.Surveying.Domain.Entities;
using Nezam.Refahi.Surveying.Domain.Enums;
using Nezam.Refahi.Surveying.Domain.Repositories;
using Nezam.Refahi.Surveying.Domain.Services;
using Nezam.Refahi.Surveying.Domain.ValueObjects;

namespace Nezam.Refahi.Surveying.Application.Commands;

/// <summary>
/// Handler for StartSurveyResponseCommand
/// </summary>
public class StartSurveyResponseCommandHandler : IRequestHandler<StartSurveyResponseCommand, ApplicationResult<StartSurveyResponseResponse>>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ISurveyUnitOfWork _unitOfWork;
    private readonly ILogger<StartSurveyResponseCommandHandler> _logger;
    private readonly ParticipationRulesDomainService _participationRulesService;
    private readonly IMemberInfoService _memberInfoService;

    public StartSurveyResponseCommandHandler(
        ISurveyRepository surveyRepository,
        ISurveyUnitOfWork unitOfWork,
        ILogger<StartSurveyResponseCommandHandler> logger,
        ParticipationRulesDomainService participationRulesService,
        IMemberInfoService memberInfoService)
    {
        _surveyRepository = surveyRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _participationRulesService = participationRulesService;
        _memberInfoService = memberInfoService;
    }

    public async Task<ApplicationResult<StartSurveyResponseResponse>> Handle(StartSurveyResponseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Load survey aggregate with responses
            var survey = await _surveyRepository.GetWithResponsesAsync(request.SurveyId, cancellationToken);
            if (survey == null)
            {
                return ApplicationResult<StartSurveyResponseResponse>.Failure("نظرسنجی مورد نظر یافت نشد. لطفاً دوباره تلاش کنید.");
            }

            // 2. Create participant info
            var participant = await CreateParticipantInfoAsync(survey, request);
            
            // 3. Get existing response for this participant from survey aggregate
            var existingResponse = survey.Responses
                .Where(r => r.Participant.Equals(participant))
                .OrderByDescending(r => r.AttemptNumber)
                .FirstOrDefault();

            // 4. Handle active response scenarios
            if (existingResponse != null && existingResponse.IsActive)
            {
                var activeResponseResult = await HandleActiveResponseScenario(survey, existingResponse, request, cancellationToken);
                if (activeResponseResult != null)
                {
                    return activeResponseResult;
                }
                // If null is returned, continue with normal flow (ForceNewAttempt scenario)
            }

            // 5. Validate participation eligibility using domain services
            var eligibilityResult = ValidateParticipationEligibility(survey, participant, existingResponse, request).Result;
            if (!eligibilityResult.IsEligible)
            {
                return ApplicationResult<StartSurveyResponseResponse>.Failure(eligibilityResult.ErrorMessage ?? "متأسفانه امکان شرکت در این نظرسنجی وجود ندارد.");
            }

            // 6. Create new response using domain aggregate
            var response = survey.StartResponse(participant, CreateDemographySnapshot(request));
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 7. Build response DTO
            var responseDto = BuildResponseDto(survey, response, false).Result;

            return ApplicationResult<StartSurveyResponseResponse>.Success(responseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting survey response for SurveyId {SurveyId}, NationalNumber {NationalNumber}", 
                request.SurveyId, request.NationalNumber);
            return ApplicationResult<StartSurveyResponseResponse>.Failure("خطایی در شروع نظرسنجی رخ داد. لطفاً دوباره تلاش کنید.");
        }
    }

    /// <summary>
    /// Creates participant info from request data using member service
    /// </summary>
    private async Task<ParticipantInfo> CreateParticipantInfoAsync(Survey survey, StartSurveyResponseCommand request)
    {
        if (survey.IsAnonymous && !string.IsNullOrEmpty(request.ParticipantHash))
        {
            return ParticipantInfo.ForAnonymous(request.ParticipantHash);
        }
        
        // For non-anonymous surveys, get member info using national number
        if (string.IsNullOrWhiteSpace(request.NationalNumber))
        {
            throw new ArgumentException("شماره ملی برای نظرسنجی‌های غیرناشناس الزامی است", nameof(request.NationalNumber));
        }

        var nationalId = new NationalId(request.NationalNumber);
        var memberInfo = await _memberInfoService.GetMemberInfoAsync(nationalId);
        
        if (memberInfo == null)
        {
            throw new InvalidOperationException($"عضو با شماره ملی {request.NationalNumber} یافت نشد");
        }

        return ParticipantInfo.ForMember(memberInfo.Id);
    }

    /// <summary>
    /// Creates demography snapshot from request data
    /// </summary>
    private static DemographySnapshot? CreateDemographySnapshot(StartSurveyResponseCommand request)
    {
        if (request.DemographyData == null || !request.DemographyData.Any())
            return null;

        return new DemographySnapshot(request.DemographyData, schemaVersion: 1);
    }

    /// <summary>
    /// Handles scenarios where an active response already exists
    /// </summary>
    private async Task<ApplicationResult<StartSurveyResponseResponse>?> HandleActiveResponseScenario(
        Survey survey, 
        Response existingResponse, 
        StartSurveyResponseCommand request, 
        CancellationToken cancellationToken)
    {
        if (request.ResumeActiveIfAny)
        {
            // Resume existing active response
            var responseDto = BuildResponseDto(survey, existingResponse, true).Result;
            return ApplicationResult<StartSurveyResponseResponse>.Success(responseDto);
        }
        
        if (request.ForceNewAttempt)
        {
            // Check if we can cancel the active attempt and start new one
            if (!survey.ParticipationPolicy.AllowMultipleSubmissions)
            {
                return ApplicationResult<StartSurveyResponseResponse>.Failure("شما در حال حاضر در حال تکمیل این نظرسنجی هستید.");
            }
            
            // Cancel the active attempt and continue with new attempt creation
            existingResponse.Cancel();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            // Return null to indicate we should continue with normal flow
            return null;
        }
        
        return ApplicationResult<StartSurveyResponseResponse>.Failure("شما در حال حاضر در حال تکمیل این نظرسنجی هستید.");
    }

    /// <summary>
    /// Validates participation eligibility using domain services
    /// </summary>
    private Task<EligibilityValidationResult> ValidateParticipationEligibility(
        Survey survey, 
        ParticipantInfo participant, 
        Response? existingResponse, 
        StartSurveyResponseCommand request)
    {
        // Check survey state and timing
        if (!survey.IsAcceptingResponses())
        {
            return Task.FromResult(new EligibilityValidationResult(false, "SURVEY_NOT_ACTIVE", "این نظرسنجی در حال حاضر غیرفعال است و امکان شرکت وجود ندارد."));
        }

        // Check time constraints
        var now = DateTimeOffset.UtcNow;
        if (survey.StartAt.HasValue && now < survey.StartAt.Value)
        {
            return Task.FromResult(new EligibilityValidationResult(false, "SURVEY_NOT_STARTED", "این نظرسنجی هنوز شروع نشده است. لطفاً در زمان مقرر مراجعه کنید."));
        }

        if (survey.EndAt.HasValue && now > survey.EndAt.Value)
        {
            return Task.FromResult(new EligibilityValidationResult(false, "SURVEY_ENDED", "متأسفانه زمان شرکت در این نظرسنجی به پایان رسیده است."));
        }

        // Use domain service for complex participation rules
        if (existingResponse != null && !existingResponse.IsActive)
        {
            var attemptNumber = existingResponse.AttemptNumber + 1;
            var lastAttemptTime = existingResponse.SubmittedAt;
            
            if (!_participationRulesService.CanParticipantParticipate(survey, participant, attemptNumber, lastAttemptTime?.DateTime))
            {
                if (!survey.ParticipationPolicy.AllowMultipleSubmissions)
                {
                    return Task.FromResult(new EligibilityValidationResult(false, "ALREADY_PARTICIPATED", "شما قبلاً در این نظرسنجی شرکت کرده‌اید و امکان شرکت مجدد وجود ندارد."));
                }

                if (attemptNumber > survey.ParticipationPolicy.MaxAttemptsPerMember)
                {
                    return Task.FromResult(new EligibilityValidationResult(false, "MAX_ATTEMPTS_REACHED", $"شما حداکثر تعداد مجاز تلاش‌ها ({survey.ParticipationPolicy.MaxAttemptsPerMember}) را انجام داده‌اید."));
                }

                // Check cooldown using domain service
                if (survey.ParticipationPolicy.CoolDownSeconds.HasValue)
                {
                    var cooldownEnd = lastAttemptTime?.AddSeconds(survey.ParticipationPolicy.CoolDownSeconds.Value);
                    if (now < cooldownEnd)
                    {
                        var remainingSeconds = (int)(cooldownEnd - now).Value.TotalSeconds;
                        var remainingMinutes = remainingSeconds / 60;
                        var remainingHours = remainingMinutes / 60;
                        
                        string timeMessage;
                        if (remainingHours > 0)
                        {
                            timeMessage = $"{remainingHours} ساعت و {remainingMinutes % 60} دقیقه";
                        }
                        else if (remainingMinutes > 0)
                        {
                            timeMessage = $"{remainingMinutes} دقیقه و {remainingSeconds % 60} ثانیه";
                        }
                        else
                        {
                            timeMessage = $"{remainingSeconds} ثانیه";
                        }
                        
                        return Task.FromResult(new EligibilityValidationResult(false, "COOLDOWN_ACTIVE", $"لطفاً {timeMessage} صبر کنید"));
                    }
                }
            }
        }

        return Task.FromResult(new EligibilityValidationResult(true, null, null));
    }

    /// <summary>
    /// Builds response DTO with proper mapping
    /// </summary>
    private Task<StartSurveyResponseResponse> BuildResponseDto(Survey survey, Response response, bool isResumed)
    {
        var firstQuestion = survey.GetOrderedQuestions().FirstOrDefault();
        var cooldownInfo = GetCooldownInfo(survey, response);
        
        // Determine current repeat index for the first unanswered question
        var currentRepeatIndex = 1;
        if (isResumed && firstQuestion != null)
        {
            var answeredRepeats = response.GetAnsweredRepeatCount(firstQuestion.Id);
            if (answeredRepeats > 0 && firstQuestion.RepeatPolicy.Kind != Domain.ValueObjects.RepeatPolicyKind.None)
            {
                var answeredIndices = response.GetAnsweredRepeatIndices(firstQuestion.Id).ToHashSet();
                currentRepeatIndex = 1;
                while (answeredIndices.Contains(currentRepeatIndex))
                {
                    currentRepeatIndex++;
                }
            }
        }

        return Task.FromResult(new StartSurveyResponseResponse
        {
            ResponseId = response.Id,
            SurveyId = response.SurveyId,
            AttemptNumber = response.AttemptNumber,
            AttemptStatus = response.AttemptStatus.ToString(),
            CurrentQuestionId = firstQuestion?.Id,
            CurrentRepeatIndex = currentRepeatIndex,
            AllowsBackNavigation = survey.ParticipationPolicy.AllowBackNavigation,
            Cooldown = cooldownInfo,
            IsResumed = isResumed,
            CanAnswer = true,
            Message = isResumed ? "ادامه نظرسنجی" : "نظرسنجی شروع شد",
            EligibilityReasons = new List<string>()
        });
    }

    /// <summary>
    /// Result of eligibility validation
    /// </summary>
    private record EligibilityValidationResult(bool IsEligible, string? ErrorCode, string? ErrorMessage);

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

    private static QuestionOptionDto MapOptionToDto(QuestionOption option)
    {
        return new QuestionOptionDto
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

    private CooldownInfo? GetCooldownInfo(Survey survey, Response response)
    {
        if (!survey.ParticipationPolicy.CoolDownSeconds.HasValue)
            return null;

        var cooldownStart = response.SubmittedAt;
        if (!cooldownStart.HasValue)
            return null;
            
        var cooldownEnd = cooldownStart.Value.AddSeconds(survey.ParticipationPolicy.CoolDownSeconds.Value);
        var remainingSeconds = Math.Max(0, (int)(cooldownEnd - DateTimeOffset.UtcNow).TotalSeconds);

        return new CooldownInfo
        {
            Enabled = true,
            Seconds = remainingSeconds,
            Message = remainingSeconds > 0 ? $"لطفاً {GetTimeMessage(remainingSeconds)} صبر کنید" : null
        };
    }

    /// <summary>
    /// Formats time duration in a user-friendly Persian format
    /// </summary>
    private static string GetTimeMessage(int totalSeconds)
    {
        var hours = totalSeconds / 3600;
        var minutes = (totalSeconds % 3600) / 60;
        var seconds = totalSeconds % 60;

        if (hours > 0)
        {
            if (minutes > 0)
                return $"{hours} ساعت و {minutes} دقیقه";
            return $"{hours} ساعت";
        }
        else if (minutes > 0)
        {
            if (seconds > 0)
                return $"{minutes} دقیقه و {seconds} ثانیه";
            return $"{minutes} دقیقه";
        }
        else
        {
            return $"{seconds} ثانیه";
        }
    }
}
