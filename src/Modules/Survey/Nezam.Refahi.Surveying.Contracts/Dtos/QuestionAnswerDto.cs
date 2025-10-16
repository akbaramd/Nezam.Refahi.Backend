namespace Nezam.Refahi.Surveying.Contracts.Dtos;

/// <summary>
/// Question answer data transfer object for client consumption
/// </summary>
public class QuestionAnswerDto
{
    public Guid Id { get; set; }
    public Guid ResponseId { get; set; }
    public Guid QuestionId { get; set; }
    public int RepeatIndex { get; set; }
    public string? TextAnswer { get; set; }
    public List<Guid> SelectedOptionIds { get; set; } = new();
    public List<QuestionAnswerOptionDto> SelectedOptions { get; set; } = new();
    public bool HasAnswer { get; set; }
}
