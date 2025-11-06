
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nezam.Refahi.Surveying.Contracts.Commands;
using Nezam.Refahi.Surveying.Contracts.Queries;
using Nezam.Refahi.Surveying.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Application.Common.Interfaces;

namespace Nezam.Refahi.Surveying.Presentation.Endpoints;

public static class SurveyEndpoints
{
    public static WebApplication MapSurveyEndpoints(this WebApplication app)
    {
        var surveyGroup = app.MapGroup("/api/v1/surveys")
            .WithTags("Surveys");

        // ───────────────────── Get Survey Overview (Anonymous) ─────────────────────
        surveyGroup.MapGet("/{surveyId:guid}/overview", async (
                Guid surveyId,
                [FromServices] IMediator mediator) =>
            {
                var query = new GetSurveyOverviewQuery(surveyId);
                var result = await mediator.Send(query);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("GetSurveyOverview")
            .Produces<ApplicationResult<SurveyOverviewResponse>>()
            .Produces(400)
            .AllowAnonymous();

        // ───────────────────── Get Survey Details (Anonymous) ─────────────────────
        surveyGroup.MapGet("/{surveyId:guid}/details", async (
                Guid surveyId,
                [FromServices] IMediator mediator) =>
            {
                var query = new GetSurveyByIdQuery { SurveyId = surveyId };
                var result = await mediator.Send(query);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("GetSurveyDetails")
            .Produces<ApplicationResult<SurveyDto>>()
            .Produces(400);

        // ───────────────────── Get Survey Details with User Context (Authenticated) ─────────────────────
        surveyGroup.MapGet("/{surveyId:guid}/details/user", async (
                Guid surveyId,
                [FromServices] IMediator mediator,
                [FromServices] ICurrentUserService currentUserService) =>
            {
                var query = new GetSurveyByIdQuery 
                { 
                    SurveyId = surveyId,
                    UserNationalNumber = currentUserService.UserNationalNumber,
                    IncludeQuestions = true,
                    IncludeUserAnswers = true
                };
                var result = await mediator.Send(query);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("GetSurveyDetailsWithUser")
            .RequireAuthorization()
            .Produces<ApplicationResult<SurveyDto>>()
            .Produces(400);

        // ───────────────────── Get Participation Status (Authenticated) ─────────────────────
        surveyGroup.MapGet("/{surveyId:guid}/participation", async (
                Guid surveyId,
                [FromServices] IMediator mediator,
                [FromServices] ICurrentUserService currentUserService) =>
            {
                if (string.IsNullOrWhiteSpace(currentUserService.UserNationalNumber))
                {
                    return Results.BadRequest(new ApplicationResult<ParticipationStatusResponse>
                    {
                        IsSuccess = false,
                        Message = "User national number not found in token",
                        Errors = new[] { "User authentication token is missing national_id claim" }
                    });
                }

                var query = new GetParticipationStatusQuery(surveyId, currentUserService.UserNationalNumber);
                var result = await mediator.Send(query);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("GetParticipationStatus")
            .RequireAuthorization()
            .Produces<ApplicationResult<ParticipationStatusResponse>>()
            .Produces(400);

        // ───────────────────── Get Current Question (Authenticated) ─────────────────────
        surveyGroup.MapGet("/{surveyId:guid}/responses/{responseId:guid}/questions/current", async (
                Guid surveyId,
                Guid responseId,
                int? repeatIndex,
                [FromServices] IMediator mediator) =>
            {
                var query = new GetCurrentQuestionQuery(surveyId, responseId, repeatIndex);
                var result = await mediator.Send(query);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("GetCurrentQuestion")
            .RequireAuthorization()
            .Produces<ApplicationResult<CurrentQuestionResponse>>()
            .Produces(400);

        // ───────────────────── Get Question by ID (Authenticated) ─────────────────────
        surveyGroup.MapGet("/{surveyId:guid}/responses/{responseId:guid}/questions/{questionId:guid}", async (
                Guid surveyId,
                Guid responseId,
                Guid questionId,
                int? repeatIndex,
                [FromServices] IMediator mediator) =>
            {
                var query = new GetQuestionByIdQuery(surveyId, responseId, questionId, repeatIndex);
                var result = await mediator.Send(query);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("GetQuestionById")
            .RequireAuthorization()
            .Produces<ApplicationResult<QuestionByIdResponse>>()
            .Produces(400);

        // ───────────────────── Get Response Progress (Authenticated) ─────────────────────
        surveyGroup.MapGet("/{surveyId:guid}/responses/{responseId:guid}/progress", async (
                Guid surveyId,
                Guid responseId,
                [FromServices] IMediator mediator) =>
            {
                var query = new GetResponseProgressQuery(surveyId, responseId);
                var result = await mediator.Send(query);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("GetResponseProgress")
            .RequireAuthorization()
            .Produces<ApplicationResult<ResponseProgressResponse>>()
            .Produces(400);

        // ───────────────────── List Questions for Navigation (Authenticated) ─────────────────────
        surveyGroup.MapGet("/{surveyId:guid}/responses/{responseId:guid}/questions", async (
                Guid surveyId,
                Guid responseId,
                [FromServices] IMediator mediator,
                [FromQuery] bool includeBackNavigation = false) =>
            {
                var query = new ListQuestionsForNavigationQuery(surveyId, responseId, includeBackNavigation);
                var result = await mediator.Send(query);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("ListQuestionsForNavigation")
            .RequireAuthorization()
            .Produces<ApplicationResult<QuestionsNavigationResponse>>()
            .Produces(400);

        // ───────────────────── Get Active Surveys (Anonymous) ─────────────────────
        surveyGroup.MapGet("/active", async (
                [FromServices] IMediator mediator,
                [FromQuery] int pageNumber = 1,
                [FromQuery] int pageSize = 20,
                [FromQuery] string? featureKey = null,
                [FromQuery] string? capabilityKey = null) =>
            {
                var query = new GetActiveSurveysQuery
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    FeatureKey = featureKey,
                    CapabilityKey = capabilityKey
                };
                var result = await mediator.Send(query);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("GetActiveSurveys")
            .Produces<ApplicationResult<ActiveSurveysResponse>>()
            .Produces(400)
            .AllowAnonymous();

        // ───────────────────── Get Surveys with User Last Response (Authenticated) ─────────────────────
        surveyGroup.MapGet("/user/last-responses", async (
                [FromServices] IMediator mediator,
                [FromServices] ICurrentUserService currentUserService,
                [FromQuery] int pageNumber = 1,
                [FromQuery] int pageSize = 20,
                [FromQuery] string? searchTerm = null,
                [FromQuery] string? state = null,
                [FromQuery] bool? isAcceptingResponses = null,
                [FromQuery] string? sortBy = null,
                [FromQuery] string? sortDirection = null,
                [FromQuery] bool includeQuestions = false,
                [FromQuery] bool includeUserLastResponse = true) =>
            {
                var query = new GetSurveysWithUserLastResponseQuery
                {
                    NationalNumber = currentUserService.UserNationalNumber,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    SearchTerm = searchTerm,
                    State = state,
                    IsAcceptingResponses = isAcceptingResponses,
                    SortBy = sortBy ?? "CreatedAt",
                    SortDirection = sortDirection ?? "Desc",
                    IncludeQuestions = includeQuestions,
                    IncludeUserLastResponse = includeUserLastResponse
                };
                var result = await mediator.Send(query);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("GetSurveysWithUserLastResponse")
            .RequireAuthorization()
            .Produces<ApplicationResult<SurveysWithUserLastResponseResponse>>()
            .Produces(400);

        // ───────────────────── Get Surveys with User Responses (Authenticated) ─────────────────────
        surveyGroup.MapGet("/user/responses", async (
                [FromServices] IMediator mediator,
                [FromServices] ICurrentUserService currentUserService,
                [FromQuery] int pageNumber = 1,
                [FromQuery] int pageSize = 20,
                [FromQuery] string? searchTerm = null,
                [FromQuery] string? state = null,
                [FromQuery] bool? isAcceptingResponses = null,
                [FromQuery] string? userResponseStatus = null,
                [FromQuery] bool? hasUserResponse = null,
                [FromQuery] bool? canUserParticipate = null,
                [FromQuery] decimal? minUserCompletionPercentage = null,
                [FromQuery] decimal? maxUserCompletionPercentage = null,
                [FromQuery] string? sortBy = null,
                [FromQuery] string? sortDirection = null,
                [FromQuery] bool includeQuestions = false,
                [FromQuery] bool includeUserResponses = true,
                [FromQuery] bool includeUserLastResponse = true) =>
            {
                var query = new GetSurveysWithUserResponsesQuery
                {
                    UserId = currentUserService.UserId,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    SearchTerm = searchTerm,
                    State = state,
                    IsAcceptingResponses = isAcceptingResponses,
                    UserResponseStatus = userResponseStatus,
                    HasUserResponse = hasUserResponse,
                    CanUserParticipate = canUserParticipate,
                    MinUserCompletionPercentage = minUserCompletionPercentage,
                    MaxUserCompletionPercentage = maxUserCompletionPercentage,
                    SortBy = sortBy ?? "CreatedAt",
                    SortDirection = sortDirection ?? "Desc",
                    IncludeQuestions = includeQuestions,
                    IncludeUserResponses = includeUserResponses,
                    IncludeUserLastResponse = includeUserLastResponse
                };
                var result = await mediator.Send(query);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("GetSurveysWithUserResponses")
            .RequireAuthorization()
            .Produces<ApplicationResult<SurveysWithUserResponsesResponse>>()
            .Produces(400);

        // ───────────────────── Get Survey Questions (Anonymous/Authenticated) ─────────────────────
        surveyGroup.MapGet("/{surveyId:guid}/questions", async (
                Guid surveyId,
                [FromServices] IMediator mediator,
                [FromServices] ICurrentUserService? currentUserService = null,
                [FromQuery] bool includeUserAnswers = false) =>
            {
                var query = new GetSurveyQuestionsQuery
                {
                    SurveyId = surveyId,
                    UserNationalNumber = currentUserService?.UserNationalNumber,
                    IncludeUserAnswers = includeUserAnswers
                };
                var result = await mediator.Send(query);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("GetSurveyQuestions")
            .Produces<ApplicationResult<SurveyQuestionsResponse>>()
            .Produces(400);

        // ───────────────────── Get Survey Questions Details (Anonymous/Authenticated) ─────────────────────
        surveyGroup.MapGet("/{surveyId:guid}/questions/details", async (
                Guid surveyId,
                [FromServices] IMediator mediator,
                [FromServices] ICurrentUserService? currentUserService = null,
                [FromQuery] bool includeUserAnswers = false,
                [FromQuery] bool includeStatistics = false) =>
            {
                var query = new GetSurveyQuestionsDetailsQuery
                {
                    SurveyId = surveyId,
                    UserNationalNumber = currentUserService?.UserNationalNumber,
                    IncludeUserAnswers = includeUserAnswers,
                    IncludeStatistics = includeStatistics
                };
                var result = await mediator.Send(query);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("GetSurveyQuestionsDetails")
            .Produces<ApplicationResult<SurveyQuestionsDetailsResponse>>()
            .Produces(400);

        // ───────────────────── Get Survey Questions with Answers (Authenticated) ─────────────────────
        surveyGroup.MapGet("/{surveyId:guid}/questions/with-answers", async (
                Guid surveyId,
                [FromServices] IMediator mediator,
                [FromServices] ICurrentUserService currentUserService,
                [FromQuery] int? attemptNumber = null) =>
            {
                var query = new GetSurveyQuestionsWithAnswersQuery
                {
                    SurveyId = surveyId,
                    UserNationalNumber = currentUserService.UserNationalNumber,
                    AttemptNumber = attemptNumber
                };
                var result = await mediator.Send(query);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("GetSurveyQuestionsWithAnswers")
            .RequireAuthorization()
            .Produces<ApplicationResult<SurveyQuestionsWithAnswersResponse>>()
            .Produces(400);

        // ───────────────────── Get User Survey Responses (Authenticated) ─────────────────────
        surveyGroup.MapGet("/{surveyId:guid}/user/responses", async (
                Guid surveyId,
                [FromServices] IMediator mediator,
                [FromServices] ICurrentUserService currentUserService,
                [FromQuery] bool includeAnswers = false,
                [FromQuery] bool includeLastAnswersOnly = false) =>
            {
                var query = new GetUserSurveyResponsesQuery
                {
                    SurveyId = surveyId,
                    NationalNumber = currentUserService.UserNationalNumber,
                    IncludeAnswers = includeAnswers,
                    IncludeLastAnswersOnly = includeLastAnswersOnly
                };
                var result = await mediator.Send(query);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("GetUserSurveyResponses")
            .RequireAuthorization()
            .Produces<ApplicationResult<UserSurveyResponsesResponse>>()
            .Produces(400);

        // ───────────────────── Get Response Details (Authenticated) ─────────────────────
        surveyGroup.MapGet("/responses/{responseId:guid}/details", async (
                Guid responseId,
                [FromServices] IMediator mediator,
                [FromServices] ICurrentUserService currentUserService,
                [FromQuery] bool includeQuestionDetails = true,
                [FromQuery] bool includeSurveyDetails = true) =>
            {
                var query = new GetResponseDetailsQuery
                {
                    ResponseId = responseId,
                    UserNationalNumber = currentUserService.UserNationalNumber,
                    IncludeQuestionDetails = includeQuestionDetails,
                    IncludeSurveyDetails = includeSurveyDetails
                };
                var result = await mediator.Send(query);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("GetResponseDetails")
            .RequireAuthorization()
            .Produces<ApplicationResult<ResponseDetailsDto>>()
            .Produces(400);

        // ───────────────────── Get Question Answer Details (Authenticated) ─────────────────────
        surveyGroup.MapGet("/responses/{responseId:guid}/questions/{questionId:guid}/answer", async (
                Guid responseId,
                Guid questionId,
                [FromServices] IMediator mediator,
                [FromServices] ICurrentUserService currentUserService,
                [FromQuery] bool includeQuestionDetails = true,
                [FromQuery] bool includeSurveyDetails = false) =>
            {
                var query = new GetQuestionAnswerDetailsQuery
                {
                    ResponseId = responseId,
                    QuestionId = questionId,
                    UserNationalNumber = currentUserService.UserNationalNumber,
                    IncludeQuestionDetails = includeQuestionDetails,
                    IncludeSurveyDetails = includeSurveyDetails
                };
                var result = await mediator.Send(query);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("GetQuestionAnswerDetails")
            .RequireAuthorization()
            .Produces<ApplicationResult<QuestionAnswerDetailsDto>>()
            .Produces(400);

        // ───────────────────── Start Survey Response (Authenticated) ─────────────────────
        surveyGroup.MapPost("/{surveyId:guid}/responses", async (
                Guid surveyId,
                [FromBody] StartSurveyResponseRequest request,
                [FromServices] IMediator mediator,
                [FromServices] ICurrentUserService currentUserService,
                [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey = null) =>
            {
                var command = new StartSurveyResponseCommand
                {
                    SurveyId = surveyId,
                    NationalNumber = currentUserService.UserNationalNumber,
                    ParticipantHash = request.ParticipantHash,
                    ForceNewAttempt = request.ForceNewAttempt,
                    DemographyData = request.DemographySnapshot,
                    ResumeActiveIfAny = request.ResumeActiveIfAny,
                    IdempotencyKey = idempotencyKey
                };
                var result = await mediator.Send(command);
                return result.IsSuccess ? Results.Created($"/api/v1/surveys/{surveyId}/responses/{result.Data!.ResponseId}", result) : Results.BadRequest(result);
            })
            .WithName("StartSurveyResponse")
            .RequireAuthorization()
            .Produces<ApplicationResult<StartSurveyResponseResponse>>(201)
            .Produces(400);

        // ───────────────────── Answer Question (Authenticated) ─────────────────────
        surveyGroup.MapPut("/{surveyId:guid}/responses/{responseId:guid}/answers/{questionId:guid}", async (
                Guid surveyId,
                Guid responseId,
                Guid questionId,
                [FromBody] AnswerQuestionRequest request,
                [FromServices] IMediator mediator,
                [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey = null) =>
            {
                var command = new AnswerQuestionCommand
                {
                    ResponseId = responseId,
                    QuestionId = questionId,
                    TextAnswer = request.TextAnswer,
                    SelectedOptionIds = request.SelectedOptionIds,
                    IdempotencyKey = idempotencyKey,
                    AllowBackNavigation = request.AllowBackNavigation
                };
                var result = await mediator.Send(command);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("AnswerQuestion")
            .RequireAuthorization()
            .Produces<ApplicationResult<AnswerQuestionResponse>>()
            .Produces(400);

        // ───────────────────── Navigation Commands (Authenticated) ─────────────────────
        
        // Go to Next Question (C3)
        surveyGroup.MapPost("/{surveyId:guid}/responses/{responseId:guid}/navigation/next", async (
                Guid surveyId,
                Guid responseId,
                [FromServices] IMediator mediator) =>
            {
                var command = new GoNextQuestionCommand
                {
                    SurveyId = surveyId,
                    ResponseId = responseId
                };
                var result = await mediator.Send(command);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("GoNextQuestion")
            .RequireAuthorization()
            .Produces<ApplicationResult<GoNextQuestionResponse>>()
            .Produces(400)
            .Produces(409);

        // Go to Previous Question (C4)
        surveyGroup.MapPost("/{surveyId:guid}/responses/{responseId:guid}/navigation/prev", async (
                Guid surveyId,
                Guid responseId,
                [FromServices] IMediator mediator) =>
            {
                var command = new GoPreviousQuestionCommand
                {
                    SurveyId = surveyId,
                    ResponseId = responseId
                };
                var result = await mediator.Send(command);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("GoPreviousQuestion")
            .RequireAuthorization()
            .Produces<ApplicationResult<GoPreviousQuestionResponse>>()
            .Produces(400)
            .Produces(409);

        // Jump to Specific Question (C5)
        surveyGroup.MapPost("/{surveyId:guid}/responses/{responseId:guid}/navigation/jump", async (
                Guid surveyId,
                Guid responseId,
                [FromBody] JumpToQuestionRequest request,
                [FromServices] IMediator mediator) =>
            {
                var command = new JumpToQuestionCommand
                {
                    SurveyId = surveyId,
                    ResponseId = responseId,
                    TargetQuestionId = request.TargetQuestionId
                };
                var result = await mediator.Send(command);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("JumpToQuestion")
            .RequireAuthorization()
            .Produces<ApplicationResult<JumpToQuestionResponse>>()
            .Produces(400)
            .Produces(409);

        // ───────────────────── Submit Response (C6) ─────────────────────
        surveyGroup.MapPost("/{surveyId:guid}/responses/{responseId:guid}/submit", async (
                Guid surveyId,
                Guid responseId,
                [FromServices] IMediator mediator,
                [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey = null) =>
            {
                var command = new SubmitResponseCommand
                {
                    SurveyId = surveyId,
                    ResponseId = responseId,
                    IdempotencyKey = idempotencyKey
                };
                var result = await mediator.Send(command);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("SubmitResponse")
            .RequireAuthorization()
            .Produces<ApplicationResult<SubmitResponseResponse>>()
            .Produces(400)
            .Produces(409);

        // ───────────────────── Cancel Response (C7) ─────────────────────
        surveyGroup.MapPost("/{surveyId:guid}/responses/{responseId:guid}/cancel", async (
                Guid surveyId,
                Guid responseId,
                [FromServices] IMediator mediator) =>
            {
                var command = new CancelResponseCommand
                {
                    SurveyId = surveyId,
                    ResponseId = responseId
                };
                var result = await mediator.Send(command);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("CancelResponse")
            .RequireAuthorization()
            .Produces<ApplicationResult<CancelResponseResponse>>()
            .Produces(400);

        // ───────────────────── AutoSave Answers (C8) ─────────────────────
        surveyGroup.MapPatch("/{surveyId:guid}/responses/{responseId:guid}/autosave", async (
                Guid surveyId,
                Guid responseId,
                [FromBody] AutoSaveAnswersRequest request,
                [FromServices] IMediator mediator) =>
            {
                var command = new AutoSaveAnswersCommand
                {
                    SurveyId = surveyId,
                    ResponseId = responseId,
                    Answers = request.Answers.Select(a => new Contracts.Commands.AutoSaveAnswerDto
                    {
                        QuestionId = a.QuestionId,
                        TextAnswer = a.TextAnswer,
                        SelectedOptionIds = a.SelectedOptionIds
                    }).ToList(),
                    Mode = (Contracts.Commands.AutoSaveMode)request.Mode
                };
                var result = await mediator.Send(command);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("AutoSaveAnswers")
            .RequireAuthorization()
            .Produces<ApplicationResult<AutoSaveAnswersResponse>>()
            .Produces(400);





        // ───────────────────── Get Specific Question by Index ─────────────────────
        surveyGroup.MapGet("/{surveyId:guid}/questions/{questionIndex:int}", async (
                Guid surveyId,
                int questionIndex,
                [FromServices] IMediator mediator,
                [FromQuery] string? userNationalNumber,
                [FromQuery] Guid? responseId,
                [FromQuery] bool includeUserAnswers = true,
                [FromQuery] bool includeNavigationInfo = true,
                [FromQuery] bool includeStatistics = false
                ) =>
            {
                var query = new GetSpecificQuestionQuery
                {
                    SurveyId = surveyId,
                    QuestionIndex = questionIndex,
                    UserNationalNumber = userNationalNumber,
                    ResponseId = responseId,
                    IncludeUserAnswers = includeUserAnswers,
                    IncludeNavigationInfo = includeNavigationInfo,
                    IncludeStatistics = includeStatistics
                };
                var result = await mediator.Send(query);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("GetSpecificQuestion")
            .Produces<ApplicationResult<GetSpecificQuestionResponse>>()
            .Produces(400)
            .AllowAnonymous();

        // ───────────────────── Get Previous Questions ─────────────────────
        surveyGroup.MapGet("/{surveyId:guid}/questions/previous", async (
                Guid surveyId,
                [FromServices] IMediator mediator,
                [FromQuery] int currentQuestionIndex,
                [FromQuery] int maxCount = 10,
                [FromQuery] string? userNationalNumber = null,
                [FromQuery] Guid? responseId = null,
                [FromQuery] bool includeUserAnswers = true,
                [FromQuery] bool includeNavigationInfo = true
                ) =>
            {
                var query = new GetPreviousQuestionsQuery
                {
                    SurveyId = surveyId,
                    CurrentQuestionIndex = currentQuestionIndex,
                    MaxCount = maxCount,
                    UserNationalNumber = userNationalNumber,
                    ResponseId = responseId,
                    IncludeUserAnswers = includeUserAnswers,
                    IncludeNavigationInfo = includeNavigationInfo
                };
                var result = await mediator.Send(query);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .WithName("GetPreviousQuestions")
            .Produces<ApplicationResult<GetPreviousQuestionsResponse>>()
            .Produces(400)
            .AllowAnonymous();

        return app;
    }
}

// ───────────────────── Request Models ─────────────────────

public record StartSurveyResponseRequest
{
    public string? ParticipantHash { get; init; }
    public bool ForceNewAttempt { get; init; }
    public Dictionary<string, string>? DemographySnapshot { get; init; }
    public bool ResumeActiveIfAny { get; init; } = true;
}

public record AnswerQuestionRequest
{
    public string? TextAnswer { get; init; }
    public List<Guid>? SelectedOptionIds { get; init; }
    public bool AllowBackNavigation { get; init; } = true;
}

public record SubmitSurveyResponseRequest
{
    public List<SurveyQuestionAnswerDto> Answers { get; init; } = new();
}

public record SurveyQuestionAnswerDto
{
    public Guid QuestionId { get; init; }
    public string? TextAnswer { get; init; }
    public List<Guid>? SelectedOptionIds { get; init; }
}

// ───────────────────── New Request Models ─────────────────────

public record JumpToQuestionRequest
{
    public Guid TargetQuestionId { get; init; }
}

public record AutoSaveAnswersRequest
{
    public List<AutoSaveAnswerRequestDto> Answers { get; init; } = new();
    public AutoSaveMode Mode { get; init; } = AutoSaveMode.Merge;
}

public record AutoSaveAnswerRequestDto
{
    public Guid QuestionId { get; init; }
    public string? TextAnswer { get; init; }
    public List<Guid>? SelectedOptionIds { get; init; }
}

public enum AutoSaveMode
{
    Merge,
    Overwrite
}