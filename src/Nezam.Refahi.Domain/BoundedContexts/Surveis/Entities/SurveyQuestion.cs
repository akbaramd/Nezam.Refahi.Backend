using System.ComponentModel.DataAnnotations;
using Nezam.Refahi.Domain.BoundedContexts.Shared.Entities;

namespace Nezam.Refahi.Domain.BoundedContexts.Surveis;

/// <summary>
/// An individual question inside a survey.
/// </summary>
public class SurveyQuestion : BaseEntity
{
    public Guid SurveyId { get; private set; }
    public Survey Survey { get; private set; } = null!;

    public string Text { get; private set; } = string.Empty;

    public QuestionType Type { get; private set; }

    public bool IsRequired { get; private set; }

    /// <summary>
    /// Zero‑based position within the survey (unique per survey).
    /// </summary>
    public int Order { get; private set; }

    public ICollection<SurveyOptions> Options { get; private set; } = new List<SurveyOptions>();
    public ICollection<SurveyAnswer> Answers { get; private set; } = new List<SurveyAnswer>();
    
    // Private constructor for EF Core
    private SurveyQuestion() : base() { }
    
    public SurveyQuestion(Survey survey, string text, QuestionType type, int order, bool isRequired = false) : base()
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Question text cannot be empty", nameof(text));
            
        if (order < 0)
            throw new ArgumentException("Order must be a non-negative integer", nameof(order));
            
        Survey = survey ?? throw new ArgumentNullException(nameof(survey));
        SurveyId = survey.Id;
        Text = text;
        Type = type;
        Order = order;
        IsRequired = isRequired;
    }
    
    public void UpdateText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Question text cannot be empty", nameof(text));
            
        Text = text;
        UpdateModifiedAt();
    }
    
    public void ChangeType(QuestionType type)
    {
        // Clear options if changing to a type that doesn't use them
        if ((type == QuestionType.OpenText || type == QuestionType.FileUpload) && 
            (Type == QuestionType.SingleChoice || Type == QuestionType.MultipleChoice || Type == QuestionType.Rating))
        {
            Options.Clear();
        }
        
        Type = type;
        UpdateModifiedAt();
    }
    
    public void SetRequired(bool isRequired)
    {
        IsRequired = isRequired;
        UpdateModifiedAt();
    }
    
    public void UpdateOrder(int order)
    {
        if (order < 0)
            throw new ArgumentException("Order must be a non-negative integer", nameof(order));
            
        Order = order;
        UpdateModifiedAt();
    }
    
    public void AddOption(string text, int displayOrder)
    {
        if (Type != QuestionType.SingleChoice && Type != QuestionType.MultipleChoice && Type != QuestionType.Rating)
            throw new InvalidOperationException("Options can only be added to choice-based questions");
            
        var option = new SurveyOptions(this, text, displayOrder);
        Options.Add(option);
        UpdateModifiedAt();
    }
    
    public void RemoveOption(Guid optionId)
    {
        var option = Options.FirstOrDefault(o => o.Id == optionId);
        if (option != null)
        {
            Options.Remove(option);
            UpdateModifiedAt();
        }
    }
}