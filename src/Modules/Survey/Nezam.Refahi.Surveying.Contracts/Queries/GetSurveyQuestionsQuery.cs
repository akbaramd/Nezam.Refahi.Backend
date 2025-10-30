using MediatR;
using Nezam.Refahi.Surveying.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Surveying.Contracts.Queries;

/// <summary>
/// Query to get survey questions with all related data
/// </summary>
public class GetSurveyQuestionsQuery : IRequest<ApplicationResult<SurveyQuestionsResponse>>
{
    public Guid SurveyId { get; set; }
    public string? UserNationalNumber { get; set; } // For including user answers
    public bool IncludeUserAnswers { get; set; } = true;
    public bool IncludeOptions { get; set; } = true;
}
