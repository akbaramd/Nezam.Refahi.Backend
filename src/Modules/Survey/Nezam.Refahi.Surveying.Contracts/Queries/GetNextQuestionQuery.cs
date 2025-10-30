using MediatR;
using Nezam.Refahi.Surveying.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Surveying.Contracts.Queries;

/// <summary>
/// Query to get the next question in a survey for a specific response
/// If currentQuestionId is null, returns the first question
/// If currentQuestionId is provided, returns the question after it
/// If it's the final question, marks it as such
/// </summary>
public class GetNextQuestionQuery : IRequest<ApplicationResult<NextQuestionResponseDto>>
{
    public Guid SurveyId { get; set; }
    public Guid ResponseId { get; set; }
    public Guid? CurrentQuestionId { get; set; } // If null, returns first question
    public string? UserNationalNumber { get; set; } // Optional: for authorization check
    public bool IncludeUserAnswer { get; set; } = true; // Include user's answer if exists
}
