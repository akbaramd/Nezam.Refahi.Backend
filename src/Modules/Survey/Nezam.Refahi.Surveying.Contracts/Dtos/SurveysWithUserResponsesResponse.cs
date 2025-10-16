using Nezam.Refahi.Surveying.Contracts.Dtos;

namespace Nezam.Refahi.Surveying.Contracts.Dtos;

/// <summary>
/// Response for surveys with user responses query
/// </summary>
public class SurveysWithUserResponsesResponse

{
    public List<SurveyDto> Surveys { get; set; } = new();
    
    // Pagination information
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
    
    // Search and filter information
    public string? SearchTerm { get; set; }
    public string? AppliedFilters { get; set; }
    public int FilteredCount { get; set; }
    
    // User participation summary
    public int UserParticipatedSurveys { get; set; }
    public int UserCompletedSurveys { get; set; }
    public int UserActiveSurveys { get; set; }
    public int UserAvailableSurveys { get; set; }
    
    // Statistics
    public int TotalActiveSurveys { get; set; }
    public int TotalScheduledSurveys { get; set; }
    public int TotalClosedSurveys { get; set; }
    public int TotalArchivedSurveys { get; set; }
    
    // Performance information
    public TimeSpan QueryExecutionTime { get; set; }
    public DateTime QueryExecutedAt { get; set; }
}
