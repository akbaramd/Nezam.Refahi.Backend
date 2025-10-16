using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Surveying.Contracts.Commands;

/// <summary>
/// Command to jump to specific question (C5)
/// </summary>
public class JumpToQuestionCommand : IRequest<ApplicationResult<JumpToQuestionResponse>>
{
    public Guid SurveyId { get; set; }
    public Guid ResponseId { get; set; }
    public Guid TargetQuestionId { get; set; }
    public int? TargetRepeatIndex { get; set; } = 1; // Target repeat index for repeatable questions
}

public class JumpToQuestionResponse
{
    public Guid? CurrentQuestionId { get; set; }
    public int CurrentRepeatIndex { get; set; } = 1; // Current repeat index for repeatable questions
    public bool BackAllowed { get; set; }
    public string? Message { get; set; }
}
