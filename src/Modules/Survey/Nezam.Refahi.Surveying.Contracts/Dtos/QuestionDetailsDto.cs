namespace Nezam.Refahi.Surveying.Contracts.Dtos;

/// <summary>
/// Detailed Question data transfer object for single question queries
/// Contains comprehensive information including all related data
/// </summary>
public class QuestionDetailsDto
{
    public Guid Id { get; set; }
    public Guid SurveyId { get; set; }
    public string Kind { get; set; } = string.Empty;
    public string KindText { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsRequired { get; set; }
    public string IsRequiredText { get; set; } = string.Empty;
    
    // Repeat policy details
    public RepeatPolicyDto RepeatPolicy { get; set; } = new();
    
    // Question specification details
    public QuestionSpecificationDto Specification { get; set; } = new();
    
    // Related data - Detailed versions
    public List<QuestionOptionDto> Options { get; set; } = new();
    
    // Answer info (if user has answered)
    public List<QuestionAnswerDetailsDto> UserAnswers { get; set; } = new();
    public QuestionAnswerDetailsDto? LatestUserAnswer { get; set; }
    public bool IsAnswered { get; set; }
    public bool IsComplete { get; set; }
    public int AnswerCount { get; set; }
    
    // Question statistics
    public QuestionStatisticsDto Statistics { get; set; } = new();
    
    // Question validation
    public QuestionValidationDto Validation { get; set; } = new();
}

/// <summary>
/// Repeat policy details
/// </summary>
public class RepeatPolicyDto
{
    public string Kind { get; set; } = string.Empty;
    public string KindText { get; set; } = string.Empty;
    public int? MaxRepeats { get; set; }
    public int? MinRepeats { get; set; }
    public bool IsRepeatable { get; set; }
    public string RepeatDescription { get; set; } = string.Empty;
}

/// <summary>
/// Question specification details
/// </summary>
public class QuestionSpecificationDto
{
    public int? MinLength { get; set; }
    public int? MaxLength { get; set; }
    public string? Pattern { get; set; }
    public string? Placeholder { get; set; }
    public string? HelpText { get; set; }
    public bool AllowMultipleSelections { get; set; }
    public bool AllowCustomAnswers { get; set; }
    public List<string> ValidationRules { get; set; } = new();
}

/// <summary>
/// Question statistics
/// </summary>
public class QuestionStatisticsDto
{
    public int TotalAnswers { get; set; }
    public int RequiredAnswers { get; set; }
    public int OptionalAnswers { get; set; }
    public decimal AnswerRate { get; set; }
    public decimal CompletionRate { get; set; }
    public TimeSpan? AverageAnswerTime { get; set; }
    public string? AverageAnswerTimeText { get; set; }
    public Dictionary<string, int> OptionSelectionCounts { get; set; } = new();
    public List<string> CommonTextAnswers { get; set; } = new();
}

/// <summary>
/// Question validation information
/// </summary>
public class QuestionValidationDto
{
    public bool IsValid { get; set; }
    public List<string> ValidationErrors { get; set; } = new();
    public List<string> ValidationWarnings { get; set; } = new();
    public bool HasRequiredValidation { get; set; }
    public bool HasLengthValidation { get; set; }
    public bool HasPatternValidation { get; set; }
    public string ValidationMessage { get; set; } = string.Empty;
}
