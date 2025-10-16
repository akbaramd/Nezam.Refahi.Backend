namespace Nezam.Refahi.Surveying.Contracts.Dtos;

/// <summary>
/// Question answer option data transfer object for client consumption
/// </summary>
public class QuestionAnswerOptionDto
{
    public Guid Id { get; set; }
    public Guid QuestionAnswerId { get; set; }
    public Guid OptionId { get; set; }
    public string OptionText { get; set; } = string.Empty;
}
