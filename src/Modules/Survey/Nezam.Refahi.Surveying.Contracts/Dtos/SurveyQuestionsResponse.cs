using Nezam.Refahi.Surveying.Contracts.Dtos;

namespace Nezam.Refahi.Surveying.Contracts.Dtos;

/// <summary>
/// Response for survey questions query
/// </summary>
public class SurveyQuestionsResponse
{
    public Guid SurveyId { get; set; }
    public string SurveyTitle { get; set; } = string.Empty;
    public string SurveyDescription { get; set; } = string.Empty;
    public List<QuestionDto> Questions { get; set; } = new();
    public bool HasUserResponse { get; set; }
    public Guid? UserResponseId { get; set; }
    public int? UserAttemptNumber { get; set; }
}
