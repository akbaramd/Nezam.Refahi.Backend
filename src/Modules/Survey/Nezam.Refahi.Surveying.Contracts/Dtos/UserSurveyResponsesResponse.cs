using Nezam.Refahi.Surveying.Contracts.Dtos;

namespace Nezam.Refahi.Surveying.Contracts.Dtos;

/// <summary>
/// Response for user's survey responses query
/// </summary>
public class UserSurveyResponsesResponse
{
    public Guid SurveyId { get; set; }
    public string SurveyTitle { get; set; } = string.Empty;
    public string? SurveyDescription { get; set; }
    public List<ResponseDto> Responses { get; set; } = new();
    
    // Summary information
    public int TotalAttempts { get; set; }
    public int CompletedAttempts { get; set; }
    public int ActiveAttempts { get; set; }
    public int CanceledAttempts { get; set; }
    public int ExpiredAttempts { get; set; }
    
    // Latest response information
    public ResponseDto? LatestResponse { get; set; }
    public bool HasActiveResponse { get; set; }
    public bool CanStartNewAttempt { get; set; }
    public string? NextActionText { get; set; }
    
    // Survey information
    public int MaxAttemptsAllowed { get; set; }
    public int RemainingAttempts { get; set; }
    public bool IsSurveyActive { get; set; }
    public string? SurveyStatusText { get; set; }
}
