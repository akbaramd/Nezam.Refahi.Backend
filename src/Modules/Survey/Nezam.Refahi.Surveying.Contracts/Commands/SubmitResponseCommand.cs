using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Surveying.Contracts.Commands;

/// <summary>
/// Command to submit response (C6)
/// </summary>
public class SubmitResponseCommand : IRequest<ApplicationResult<SubmitResponseResponse>>
{
    public Guid SurveyId { get; set; }
    public Guid ResponseId { get; set; }
    public string? IdempotencyKey { get; set; }
    public string? NationalNumber { get; set; } // For member authorization
}

public class SubmitResponseResponse
{
    public bool Submitted { get; set; }
    public DateTimeOffset? SubmittedAt { get; set; }
    public ResponseSummaryDto Summary { get; set; } = new();
    public List<string> ValidationErrors { get; set; } = new();
}

public class ResponseSummaryDto
{
    public int Answered { get; set; }
    public int Total { get; set; }
    public double Completion { get; set; }
    public List<Guid> UnansweredRequiredQuestions { get; set; } = new();
}
