using MediatR;
using Nezam.Refahi.Surveying.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Surveying.Contracts.Queries;

/// <summary>
/// Query to get detailed information about a specific response with all answers
/// </summary>
public class GetResponseDetailsQuery : IRequest<ApplicationResult<ResponseDetailsDto>>
{
    public Guid ResponseId { get; set; }
    public Guid? MemberId { get; set; } // Optional: for authorization check
    public bool IncludeQuestionDetails { get; set; } = true; // Include full question information
    public bool IncludeSurveyDetails { get; set; } = true; // Include survey information
}
