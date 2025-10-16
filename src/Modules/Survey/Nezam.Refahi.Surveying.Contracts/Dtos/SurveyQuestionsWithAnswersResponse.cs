using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Surveying.Contracts.Dtos;

public class SurveyQuestionsWithAnswersResponse
{
    public Guid SurveyId { get; set; }
    public string SurveyTitle { get; set; } = string.Empty;
    public string SurveyDescription { get; set; } = string.Empty;
    public List<QuestionWithAnswersDto> Questions { get; set; } = new();
    public ResponseInfoDto? ResponseInfo { get; set; }
    public bool HasUserResponse { get; set; }
    public int TotalQuestions { get; set; }
    public int AnsweredQuestions { get; set; }
    public decimal CompletionPercentage { get; set; }
}

public class QuestionWithAnswersDto
{
    public Guid Id { get; set; }
    public Guid SurveyId { get; set; }
    public string Kind { get; set; } = string.Empty;
    public string KindText { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsRequired { get; set; }
    public List<QuestionOptionDto> Options { get; set; } = new();
    public List<QuestionAnswerDto> UserAnswers { get; set; } = new(); // All attempts for this question
    public QuestionAnswerDto? LatestAnswer { get; set; } // Latest answer for this question
    public bool IsAnswered { get; set; }
    public bool IsComplete { get; set; }
    public int AnswerCount { get; set; } // Number of times this question has been answered
}

public class ResponseInfoDto
{
    public Guid ResponseId { get; set; }
    public Guid SurveyId { get; set; }
    public int AttemptNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public string ParticipantDisplayName { get; set; } = string.Empty;
    public string ParticipantShortIdentifier { get; set; } = string.Empty;
    public bool IsAnonymous { get; set; }
    public int TotalQuestions { get; set; }
    public int AnsweredQuestions { get; set; }
    public decimal CompletionPercentage { get; set; }
    public bool IsComplete { get; set; }
}
