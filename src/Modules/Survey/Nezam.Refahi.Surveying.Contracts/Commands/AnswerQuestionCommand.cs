using MediatR;
using Nezam.Refahi.Surveying.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Surveying.Contracts.Commands;

/// <summary>
/// Command to save/edit answer for a specific question (C2)
/// </summary>
public class AnswerQuestionCommand : IRequest<ApplicationResult<AnswerQuestionResponse>>
{
    public Guid ResponseId { get; set; }
    public Guid QuestionId { get; set; }
    public int RepeatIndex { get; set; } = 1; // Repeat index for repeatable questions
    public string? TextAnswer { get; set; }
    public List<Guid>? SelectedOptionIds { get; set; }
    public bool Overwrite { get; set; } = true; // Whether to overwrite existing answer for this repeat
    public string? IdempotencyKey { get; set; } // For idempotent operations
    public bool AllowBackNavigation { get; set; } = true; // Whether to allow answering previous questions
    public string? NationalNumber { get; set; } // For member authorization
}

public class AnswerQuestionResponse
{
    public Guid ResponseId { get; set; }
    public Guid QuestionId { get; set; }
    public int RepeatIndex { get; set; }
    public bool IsAnswered { get; set; }
    public bool IsOverwritten { get; set; } // Whether previous answer was overwritten
    public int AnsweredQuestions { get; set; }
    public int AnsweredRepeatsForThisQuestion { get; set; } // Number of answered repeats for this question
    public int? TotalRepeatsAllowed { get; set; } // null for Unbounded
    public double CompletionPercentage { get; set; }
    public string ResponseStatus { get; set; } = string.Empty; // Current response status
    public string ResponseStatusText { get; set; } = string.Empty; // Persian text for response status
    public string? Message { get; set; }
    public List<string> ValidationErrors { get; set; } = new(); // Question-specific validation errors
}
