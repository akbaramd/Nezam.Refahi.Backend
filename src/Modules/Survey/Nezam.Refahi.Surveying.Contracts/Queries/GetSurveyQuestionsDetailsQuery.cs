using MediatR;
using Nezam.Refahi.Surveying.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Surveying.Contracts.Queries;

/// <summary>
/// Query to get detailed information about survey questions
/// </summary>
public class GetSurveyQuestionsDetailsQuery : IRequest<ApplicationResult<SurveyQuestionsDetailsResponse>>
{
    public Guid SurveyId { get; set; }
    public string? UserNationalNumber { get; set; } // If provided, include user's answer status
    public bool IncludeUserAnswers { get; set; } = true;
    public bool IncludeStatistics { get; set; } = false; // Include answer statistics
}
