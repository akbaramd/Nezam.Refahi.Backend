using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Surveying.Contracts.Commands;

/// <summary>
/// Command to cancel response (C7)
/// </summary>
public class CancelResponseCommand : IRequest<ApplicationResult<CancelResponseResponse>>
{
    public Guid SurveyId { get; set; }
    public Guid ResponseId { get; set; }
}

public class CancelResponseResponse
{
    public bool Canceled { get; set; }
    public bool IsAbandoned { get; set; }
    public string? Message { get; set; }
}
