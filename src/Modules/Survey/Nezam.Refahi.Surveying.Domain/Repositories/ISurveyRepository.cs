using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Surveying.Domain.Entities;
using Nezam.Refahi.Surveying.Domain.Enums;

namespace Nezam.Refahi.Surveying.Domain.Repositories;

/// <summary>
/// Repository interface for survey entities
/// </summary>
public interface ISurveyRepository : IRepository<Survey, Guid>
{
    /// <summary>
    /// Gets surveys by state
    /// </summary>
    Task<IEnumerable<Survey>> GetByStateAsync(SurveyState state, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets active surveys
    /// </summary>
    Task<IEnumerable<Survey>> GetActiveSurveysAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets surveys by feature code
    /// </summary>
    Task<IEnumerable<Survey>> GetByFeatureCodeAsync(string featureCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets surveys by capability code
    /// </summary>
    Task<IEnumerable<Survey>> GetByCapabilityCodeAsync(string capabilityCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets survey with questions
    /// </summary>
    Task<Survey?> GetWithQuestionsAsync(Guid surveyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets survey with responses
    /// </summary>
    Task<Survey?> GetWithResponsesAsync(Guid surveyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets survey with all related data (questions, responses, features, capabilities)
    /// </summary>
    Task<Survey?> GetWithAllDataAsync(Guid surveyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets surveys in date range
    /// </summary>
    Task<IEnumerable<Survey>> GetInDateRangeAsync(DateTimeOffset fromDate, DateTimeOffset toDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets surveys that need to be activated (scheduled surveys)
    /// </summary>
    Task<IEnumerable<Survey>> GetScheduledSurveysToActivateAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets surveys that need to be closed (expired surveys)
    /// </summary>
    Task<IEnumerable<Survey>> GetActiveSurveysToCloseAsync(CancellationToken cancellationToken = default);

    // New methods for complex queries with pagination and filtering

    /// <summary>
    /// Gets surveys with pagination, search, and filtering
    /// </summary>
    Task<(IEnumerable<Survey> Surveys, int TotalCount)> GetSurveysWithPaginationAsync(
        int pageNumber, 
        int pageSize, 
        string? searchTerm = null,
        SurveyState? state = null,
        bool? isAnonymous = null,
        bool? isAcceptingResponses = null,
        DateTime? startDateFrom = null,
        DateTime? startDateTo = null,
        DateTime? endDateFrom = null,
        DateTime? endDateTo = null,
        List<string>? requiredFeatures = null,
        List<string>? requiredCapabilities = null,
        List<string>? excludedFeatures = null,
        List<string>? excludedCapabilities = null,
        string sortBy = "CreatedAt",
        string sortDirection = "Desc",
        bool includeQuestions = false,
        bool includeResponses = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets surveys with user's responses for a specific user
    /// </summary>
    Task<(IEnumerable<Survey> Surveys, int TotalCount)> GetSurveysWithUserResponsesAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        SurveyState? state = null,
        bool? isAcceptingResponses = null,
        bool? hasUserResponse = null,
        bool? canUserParticipate = null,
        AttemptStatus? userResponseStatus = null,
        bool? userHasCompletedSurvey = null,
        decimal? minUserCompletionPercentage = null,
        decimal? maxUserCompletionPercentage = null,
        string sortBy = "CreatedAt",
        string sortDirection = "Desc",
        bool includeQuestions = false,
        bool includeUserResponses = true,
        bool includeUserLastResponse = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets survey statistics by state
    /// </summary>
    Task<(int ActiveCount, int ScheduledCount, int ClosedCount, int ArchivedCount)> GetSurveyStatisticsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets surveys with questions and options included
    /// </summary>
    Task<IEnumerable<Survey>> GetSurveysWithQuestionsAsync(
        IEnumerable<Guid> surveyIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets survey containing a specific response by response ID
    /// Efficient method to find survey that contains the response
    /// </summary>
    Task<Survey?> GetSurveyByResponseIdAsync(Guid responseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets survey with specific response and questions for efficient loading
    /// </summary>
    Task<Survey?> GetSurveyWithResponseAndQuestionsAsync(Guid responseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets survey with specific response and all related data
    /// </summary>
    Task<Survey?> GetSurveyWithResponseAndAllDataAsync(Guid responseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user's latest response for each survey in the provided list
    /// </summary>
    Task<Dictionary<Guid, Response?>> GetUserLatestResponsesAsync(
        IEnumerable<Guid> surveyIds,
        Guid userId,
        CancellationToken cancellationToken = default);
}
