using Nezam.Refahi.Surveying.Contracts.Dtos;

namespace Nezam.Refahi.Surveying.Contracts.Dtos;

/// <summary>
/// Response for surveys with user's last response query
/// </summary>
public class SurveysWithUserLastResponseResponse
{
    public List<SurveyDto> Surveys { get; set; } = new();
    
    // Pagination information
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
    
    // User participation summary
    public int UserParticipatedSurveys { get; set; }
    public int UserCompletedSurveys { get; set; }
    public int UserActiveSurveys { get; set; }
    public int UserAvailableSurveys { get; set; }
    
    // Performance information
    public TimeSpan QueryExecutionTime { get; set; }
    public DateTime QueryExecutedAt { get; set; }
}
