using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Surveying.Contracts.Commands;

/// <summary>
/// Command to navigate to previous question (C4)
/// </summary>
public class GoPreviousQuestionCommand : IRequest<ApplicationResult<GoPreviousQuestionResponse>>
{
    public Guid SurveyId { get; set; }
    public Guid ResponseId { get; set; }
}

public class GoPreviousQuestionResponse
{
    public Guid? CurrentQuestionId { get; set; }
    public int CurrentRepeatIndex { get; set; } = 1; // Current repeat index for repeatable questions
    public CommandProgressDto Progress { get; set; } = new();
    public bool BackAllowed { get; set; }
    public string? Message { get; set; }
}
