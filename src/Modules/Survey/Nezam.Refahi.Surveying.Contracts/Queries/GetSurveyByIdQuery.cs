using MediatR;
using Nezam.Refahi.Surveying.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Surveying.Contracts.Queries;

/// <summary>
/// Query to get survey by ID with all related data
/// </summary>
public class GetSurveyByIdQuery : IRequest<ApplicationResult<SurveyDto>>
{
    public Guid SurveyId { get; set; }
    public string? UserNationalNumber { get; set; } // For checking participation eligibility
    public bool IncludeQuestions { get; set; } = true;
    public bool IncludeUserAnswers { get; set; } = true;
}
