using MediatR;
using Nezam.Refahi.Surveying.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Surveying.Contracts.Commands;

/// <summary>
/// Command to submit complete survey response
/// </summary>
public class SubmitSurveyResponseCommand : IRequest<ApplicationResult<SubmitSurveyResponseResponse>>
{
    public Guid ResponseId { get; set; }
    public List<QuestionAnswerDto> Answers { get; set; } = new();
    public string? NationalNumber { get; set; } // For member authorization
}

public class SubmitSurveyResponseResponse
{
    public Guid ResponseId { get; set; }
    public Guid SurveyId { get; set; }
    public bool IsSubmitted { get; set; }
    public bool IsComplete { get; set; }
    public int AnsweredQuestions { get; set; }
    public int TotalQuestions { get; set; }
    public decimal CompletionPercentage { get; set; }
    public string? Message { get; set; }
}
