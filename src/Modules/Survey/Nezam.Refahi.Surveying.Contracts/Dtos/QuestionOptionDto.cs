namespace Nezam.Refahi.Surveying.Contracts.Dtos;

/// <summary>
/// Question option data transfer object for client consumption
/// </summary>
public class QuestionOptionDto
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public string Text { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsActive { get; set; }
    public bool IsSelected { get; set; }
}
