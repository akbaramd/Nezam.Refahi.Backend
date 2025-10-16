using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Surveying.Contracts.Dtos;

public class NextQuestionResponseDto
{
    public Guid SurveyId { get; set; }
    public Guid ResponseId { get; set; }
    public Guid? CurrentQuestionId { get; set; }
    public Guid? NextQuestionId { get; set; }
    public bool HasNextQuestion { get; set; }
    public bool IsFirstQuestion { get; set; }
    public bool IsLastQuestion { get; set; }
    public int CurrentQuestionOrder { get; set; }
    public int NextQuestionOrder { get; set; }
    public int TotalQuestions { get; set; }
    public int AnsweredQuestions { get; set; }
    public decimal ProgressPercentage { get; set; }
    
    // Next Question Details
    public QuestionDetailsDto? NextQuestion { get; set; }
    
    // User's Answer for Next Question (if exists)
    public QuestionAnswerDetailsDto? UserAnswer { get; set; }
    
    // Navigation Information
    public QuestionNavigationDto Navigation { get; set; } = new();
    
    // Survey Progress
    public SurveyProgressDto Progress { get; set; } = new();
}

public class QuestionNavigationDto
{
    public Guid? PreviousQuestionId { get; set; }
    public Guid? NextQuestionId { get; set; }
    public bool CanGoBack { get; set; }
    public bool CanGoForward { get; set; }
    public bool CanSkip { get; set; }
    public bool IsRequired { get; set; }
}

public class SurveyProgressDto
{
    public int CurrentStep { get; set; }
    public int TotalSteps { get; set; }
    public decimal CompletionPercentage { get; set; }
    public int RemainingQuestions { get; set; }
    public bool IsComplete { get; set; }
    public string ProgressText { get; set; } = string.Empty;
}
