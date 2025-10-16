using MediatR;
using Nezam.Refahi.Surveying.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Surveying.Contracts.Queries;

/// <summary>
/// Query to get user's responses to a specific survey
/// </summary>
public class GetUserSurveyResponsesQuery : IRequest<ApplicationResult<UserSurveyResponsesResponse>>
{
    public string? NationalNumber { get; set; } // National number for member identification
    public Guid SurveyId { get; set; }
    public bool IncludeAnswers { get; set; } = true;
    public bool IncludeLastAnswersOnly { get; set; } = false; // Only latest answer for each question
}
