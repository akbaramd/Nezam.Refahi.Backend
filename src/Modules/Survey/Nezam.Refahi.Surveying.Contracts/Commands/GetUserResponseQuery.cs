using MediatR;
using Nezam.Refahi.Surveying.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Surveying.Contracts.Commands;

/// <summary>
/// Query to get user's response for a survey
/// </summary>
public class GetUserResponseQuery : IRequest<ApplicationResult<ResponseDto>>
{
    public Guid SurveyId { get; set; }
    public Guid UserId { get; set; }
    public int? AttemptNumber { get; set; } // If null, get latest attempt
}

public class GetUserResponseResponse
{
    public ResponseDto? Response { get; set; }
    public bool HasResponse { get; set; }
    public int TotalAttempts { get; set; }
    public bool CanCreateNewAttempt { get; set; }
    public string? Message { get; set; }
}
