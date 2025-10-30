using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Surveying.Contracts.Commands;

/// <summary>
/// Command to navigate to next question (C3)
/// </summary>
public class GoNextQuestionCommand : IRequest<ApplicationResult<GoNextQuestionResponse>>
{
    public Guid SurveyId { get; set; }
    public Guid ResponseId { get; set; }
}

public class GoNextQuestionResponse
{
    public Guid? CurrentQuestionId { get; set; }
    public int CurrentRepeatIndex { get; set; } = 1; // Current repeat index for repeatable questions
    public string ResponseStatus { get; set; } = string.Empty; // New response status
    public string ResponseStatusText { get; set; } = string.Empty; // Persian text for response status
    public CommandProgressDto Progress { get; set; } = new();
}

public class CommandProgressDto
{
    public int Answered { get; set; }
    public int Total { get; set; }
    public double CompletionPercentage { get; set; }
}
