using System.ComponentModel.DataAnnotations;
using Nezam.Refahi.Domain.BoundedContexts.Shared.Entities;

namespace Nezam.Refahi.Domain.BoundedContexts.Surveis;

/// <summary>
/// Choice option for a question (only used for choice‑based questions).
/// </summary>
public class SurveyOptions : BaseEntity
{
    public Guid QuestionId { get; private set; }
    public SurveyQuestion Question { get; private set; } = null!;

    
    public string Text { get; private set; } = string.Empty;

    public int DisplayOrder { get; private set; }
    
    // Private constructor for EF Core
    private SurveyOptions() : base() { }
    
    public SurveyOptions(SurveyQuestion question, string text, int displayOrder) : base()
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Option text cannot be empty", nameof(text));
            
        if (displayOrder < 0)
            throw new ArgumentException("Display order must be a non-negative integer", nameof(displayOrder));
            
        Question = question ?? throw new ArgumentNullException(nameof(question));
        QuestionId = question.Id;
        Text = text;
        DisplayOrder = displayOrder;
    }
    
    public void UpdateText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Option text cannot be empty", nameof(text));
            
        Text = text;
        UpdateModifiedAt();
    }
    
    public void UpdateDisplayOrder(int displayOrder)
    {
        if (displayOrder < 0)
            throw new ArgumentException("Display order must be a non-negative integer", nameof(displayOrder));
            
        DisplayOrder = displayOrder;
        UpdateModifiedAt();
    }
}