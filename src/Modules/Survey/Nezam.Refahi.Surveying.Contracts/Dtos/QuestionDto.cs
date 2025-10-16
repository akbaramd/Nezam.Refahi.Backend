namespace Nezam.Refahi.Surveying.Contracts.Dtos;

/// <summary>
/// Question data transfer object for client consumption
/// </summary>
public class QuestionDto
{
    public Guid Id { get; set; }
    public Guid SurveyId { get; set; }
    public string Kind { get; set; } = string.Empty;
    public string KindText { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsRequired { get; set; }
    public string IsRequiredText { get; set; } = string.Empty;
    
    // Related data
    public List<QuestionOptionDto> Options { get; set; } = new();
    
    // Answer info (if user has answered)
    public QuestionAnswerDto? UserAnswer { get; set; }
    public bool IsAnswered { get; set; }
    public bool IsComplete { get; set; }
}
