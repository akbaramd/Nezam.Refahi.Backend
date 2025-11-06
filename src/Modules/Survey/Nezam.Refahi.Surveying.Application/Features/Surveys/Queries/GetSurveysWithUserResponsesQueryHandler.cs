using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Surveying.Contracts.Dtos;
using Nezam.Refahi.Surveying.Contracts.Queries;
using Nezam.Refahi.Surveying.Domain.Entities;
using Nezam.Refahi.Surveying.Domain.Enums;
using Nezam.Refahi.Surveying.Domain.Repositories;
using Nezam.Refahi.Surveying.Domain.ValueObjects;

namespace Nezam.Refahi.Surveying.Application.Features.Surveys.Queries;

/// <summary>
/// Handler for getting surveys with user responses information
/// </summary>
public class GetSurveysWithUserResponsesQueryHandler : IRequestHandler<GetSurveysWithUserResponsesQuery, ApplicationResult<SurveysWithUserResponsesResponse>>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ILogger<GetSurveysWithUserResponsesQueryHandler> _logger;
    private readonly IMemberService _memberService;

    public GetSurveysWithUserResponsesQueryHandler(
        ISurveyRepository surveyRepository,
        ILogger<GetSurveysWithUserResponsesQueryHandler> logger,
        IMemberService memberService)
    {
        _surveyRepository = surveyRepository;
        _logger = logger;
        _memberService = memberService;
    }

    public async Task<ApplicationResult<SurveysWithUserResponsesResponse>> Handle(
        GetSurveysWithUserResponsesQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var startTime = DateTimeOffset.UtcNow;
            
            // Get member info if UserId is provided
            Guid? memberId = null;
            if (request.UserId.HasValue)
            {
                var memberDto = await _memberService.GetMemberByExternalIdAsync(request.UserId.Value.ToString());
                if (memberDto != null)
                {
                    memberId = memberDto.Id;
                }
            }
            
            // Parse attempt status if provided
            AttemptStatus? attemptStatus = null;
            if (!string.IsNullOrWhiteSpace(request.UserResponseStatus) && 
                Enum.TryParse<AttemptStatus>(request.UserResponseStatus, true, out var parsedStatus))
            {
                attemptStatus = parsedStatus;
            }

            // Parse survey state if provided
            SurveyState? surveyState = null;
            if (!string.IsNullOrWhiteSpace(request.State) && 
                Enum.TryParse<SurveyState>(request.State, true, out var parsedState))
            {
                surveyState = parsedState;
            }

            // Get surveys with pagination and filtering using the dedicated method
            var (surveys, totalCount) = await _surveyRepository.GetSurveysWithUserResponsesAsync(
                userId: memberId ?? Guid.Empty,
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                searchTerm: request.SearchTerm,
                state: surveyState,
                isAcceptingResponses: request.IsAcceptingResponses,
                hasUserResponse: request.HasUserResponse,
                canUserParticipate: request.CanUserParticipate,
                userResponseStatus: attemptStatus,
                userHasCompletedSurvey: request.UserHasCompletedSurvey,
                minUserCompletionPercentage: request.MinUserCompletionPercentage,
                maxUserCompletionPercentage: request.MaxUserCompletionPercentage,
                sortBy: !string.IsNullOrWhiteSpace(request.SortBy) ? request.SortBy : "CreatedAt",
                sortDirection: !string.IsNullOrWhiteSpace(request.SortDirection) ? request.SortDirection : "Desc",
                includeQuestions: request.IncludeQuestions,
                includeUserResponses: request.IncludeUserResponses,
                includeUserLastResponse: request.IncludeUserLastResponse,
                cancellationToken: cancellationToken);

            var surveysList = surveys.ToList();
            
            // Get user's latest responses for all surveys efficiently
            var userLastResponses = new Dictionary<Guid, Response>();
            var allUserResponses = new Dictionary<Guid, List<Response>>();
            
            if (memberId.HasValue)
            {
                var surveyIds = surveysList.Select(s => s.Id).ToList();
                
                if (request.IncludeUserLastResponse && surveyIds.Any())
                {
                    var latestResponses = await _surveyRepository.GetUserLatestResponsesAsync(
                        surveyIds, 
                        memberId.Value, 
                        cancellationToken);
                    
                    foreach (var kvp in latestResponses)
                    {
                        if (kvp.Value != null)
                        {
                            userLastResponses[kvp.Key] = kvp.Value;
                        }
                    }
                }

                if (request.IncludeUserResponses)
                {
                    // Get all user responses for each survey
                    foreach (var survey in surveysList)
                    {
                        var userResponses = survey.Responses
                            .Where(r => r.Participant.MemberId == memberId.Value)
                            .ToList();
                        
                        if (userResponses.Any())
                        {
                            allUserResponses[survey.Id] = userResponses;
                        }
                    }
                }
            }

            // Convert surveys to DTOs
            var surveyDtos = surveysList.Select(survey => 
                MapToSurveyDto(survey, request, userLastResponses, allUserResponses, memberId)).ToList();

            var response = new SurveysWithUserResponsesResponse
            {
                Surveys = surveyDtos,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
                HasNextPage = request.PageNumber < (int)Math.Ceiling((double)totalCount / request.PageSize),
                HasPreviousPage = request.PageNumber > 1,
                FilteredCount = totalCount,
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

            // Calculate statistics
            response.TotalActiveSurveys = surveyDtos.Count(s => s.IsActive);
            response.TotalScheduledSurveys = surveyDtos.Count(s => s.IsScheduled);
            response.TotalClosedSurveys = surveyDtos.Count(s => s.State == SurveyState.Completed.ToString());
            response.TotalArchivedSurveys = surveyDtos.Count(s => s.State == SurveyState.Archived.ToString());

            return ApplicationResult<SurveysWithUserResponsesResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting surveys with user responses");
            return ApplicationResult<SurveysWithUserResponsesResponse>.Failure(ex, "خطا در دریافت لیست نظرسنجی‌ها با پاسخ‌های کاربر");
        }
    }

    private SurveyDto MapToSurveyDto(Survey survey, GetSurveysWithUserResponsesQuery request, 
        Dictionary<Guid, Response> userLastResponses, Dictionary<Guid, List<Response>> allUserResponses, Guid? memberId)
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
            AllowMultipleSubmissions = survey.ParticipationPolicy.AllowMultipleSubmissions,
            CoolDownSeconds = survey.ParticipationPolicy.CoolDownSeconds,
            AllowBackNavigation = survey.ParticipationPolicy.AllowBackNavigation,
            TotalQuestions = survey.Questions.Count,
            RequiredQuestions = survey.Questions.Count(q => q.IsRequired),
            ResponseCount = survey.Responses.Count,
            UniqueParticipantCount = survey.Responses.Select(r => r.Participant.MemberId).Distinct().Count(),
            IsAcceptingResponses = survey.IsAcceptingResponses(),
            IsExpired = survey.EndAt.HasValue && survey.EndAt <= DateTimeOffset.UtcNow,
            IsScheduled = survey.StartAt.HasValue && survey.StartAt > DateTimeOffset.UtcNow,
            IsActive = survey.State == SurveyState.Published && survey.IsAcceptingResponses()
        };

        // Include user response information
        if (memberId.HasValue)
        {
            Response? latestResponse = null;
            List<Response> userResponses = new List<Response>();
            
            // Get user responses
            if (allUserResponses.ContainsKey(survey.Id))
            {
                userResponses = allUserResponses[survey.Id];
                latestResponse = userResponses.OrderByDescending(r => r.AttemptNumber).FirstOrDefault();
            }
            else if (userLastResponses.ContainsKey(survey.Id))
            {
                latestResponse = userLastResponses[survey.Id];
                userResponses = new List<Response> { latestResponse };
            }

            if (latestResponse != null)
            {
                dto.HasUserResponse = true;
                dto.UserLastResponse = MapToResponseDto(latestResponse, survey);
                dto.UserCompletionPercentage = dto.UserLastResponse.CompletionPercentage;
                dto.UserAnsweredQuestions = dto.UserLastResponse.AnsweredQuestions;
                dto.UserHasCompletedSurvey = dto.UserLastResponse.IsSubmitted || IsResponseComplete(survey, latestResponse);
                dto.UserAttemptCount = userResponses.Count;
                dto.RemainingAttempts = Math.Max(0, survey.ParticipationPolicy.MaxAttemptsPerMember - userResponses.Count);

                var participant = ParticipantInfo.ForMember(memberId.Value);
                dto.CanUserParticipate = CanUserParticipate(survey, participant, userResponses);
            }
            else
            {
                dto.HasUserResponse = false;
                dto.CanUserParticipate = survey.IsAcceptingResponses();
                dto.UserAttemptCount = 0;
                dto.RemainingAttempts = survey.ParticipationPolicy.MaxAttemptsPerMember;
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
                Order = q.Order,
                IsRequired = q.IsRequired,
                Options = q.Options.Select(o => new QuestionOptionDto
                {
                    Id = o.Id,
                    Text = o.Text,
                    Order = o.Order,
                    IsActive = o.IsActive
                }).ToList()
            }).ToList();
        }

        return dto;
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
            AttemptStatus = response.AttemptStatus.ToString(),
            IsActive = response.AttemptStatus == AttemptStatus.Active,
            IsSubmitted = response.AttemptStatus == AttemptStatus.Submitted,
            IsExpired = response.AttemptStatus == AttemptStatus.Expired,
            IsCanceled = response.AttemptStatus == AttemptStatus.Canceled,
            TotalQuestions = survey.Questions.Count,
            AnsweredQuestions = response.QuestionAnswers.Count(qa => qa.HasAnswer()),
            RequiredQuestions = survey.Questions.Count(q => q.IsRequired),
            CompletionPercentage = CalculateCompletionPercentage(response, survey),
            IsComplete = IsResponseComplete(survey, response)
        };
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

    private static bool IsResponseComplete(Survey survey, Response response)
    {
        var requiredQuestions = survey.Questions.Where(q => q.IsRequired).ToList();
        return requiredQuestions.All(q => response.HasAnswerForQuestion(q.Id));
    }

    private static decimal CalculateCompletionPercentage(Response response, Survey survey)
    {
        if (survey.Questions.Count == 0) return 100;
        var answeredQuestions = response.QuestionAnswers.Count(qa => qa.HasAnswer());
        return Math.Round((decimal)answeredQuestions / survey.Questions.Count * 100, 1);
    }

    private static string GetStateText(SurveyState state) => state switch
    {
        SurveyState.Draft => "پیش‌نویس",
        SurveyState.Published => "فعال",
        SurveyState.Completed => "بسته شده",
        SurveyState.Archived => "آرشیو شده",
        _ => "نامشخص"
    };
}

