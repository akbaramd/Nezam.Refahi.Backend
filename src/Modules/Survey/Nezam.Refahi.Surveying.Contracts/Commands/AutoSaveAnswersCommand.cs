using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Surveying.Contracts.Commands;

/// <summary>
/// Command to autosave answers (C8)
/// </summary>
public class AutoSaveAnswersCommand : IRequest<ApplicationResult<AutoSaveAnswersResponse>>
{
    public Guid SurveyId { get; set; }
    public Guid ResponseId { get; set; }
    public List<AutoSaveAnswerDto> Answers { get; set; } = new();
    public AutoSaveMode Mode { get; set; } = AutoSaveMode.Merge;
}

public class AutoSaveAnswerDto
{
    public Guid QuestionId { get; set; }
    public string? TextAnswer { get; set; }
    public List<Guid>? SelectedOptionIds { get; set; }
}

public enum AutoSaveMode
{
    Merge,
    Overwrite
}

public class AutoSaveAnswersResponse
{
    public int SavedCount { get; set; }
    public string ResponseStatus { get; set; } = string.Empty; // New response status
    public string ResponseStatusText { get; set; } = string.Empty; // Persian text for response status
    public List<InvalidAnswerDto> Invalids { get; set; } = new();
}

public class InvalidAnswerDto
{
    public Guid QuestionId { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}
