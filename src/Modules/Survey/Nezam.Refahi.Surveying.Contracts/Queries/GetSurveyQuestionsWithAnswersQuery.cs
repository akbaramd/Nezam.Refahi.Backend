using MediatR;
using Nezam.Refahi.Surveying.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Surveying.Contracts.Queries;

/// <summary>
/// Query to get survey questions with user answers for a specific response
/// </summary>
public class GetSurveyQuestionsWithAnswersQuery : IRequest<ApplicationResult<SurveyQuestionsWithAnswersResponse>>
{
    public Guid SurveyId { get; set; }
    public Guid? ResponseId { get; set; } // If provided, get answers for this specific response
    public string? UserNationalNumber { get; set; } // If provided, get answers for this member
    public int? AttemptNumber { get; set; } // If provided, get answers for this specific attempt
    public bool IncludeAllAttempts { get; set; } = false; // If true, include answers from all attempts
}
