using MediatR;
using Nezam.Refahi.Surveying.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Surveying.Contracts.Queries;

/// <summary>
/// Query to get detailed information about a specific question answer for a specific response
/// </summary>
public class GetQuestionAnswerDetailsQuery : IRequest<ApplicationResult<QuestionAnswerDetailsDto>>
{
    public Guid ResponseId { get; set; }
    public Guid QuestionId { get; set; }
    public string? UserNationalNumber { get; set; } // Optional: for authorization check
    public bool IncludeQuestionDetails { get; set; } = true; // Include full question information
    public bool IncludeSurveyDetails { get; set; } = false; // Include survey information
}
