using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Surveying.Contracts.Dtos;

public class SurveyQuestionsDetailsResponse
{
    public Guid SurveyId { get; set; }
    public string SurveyTitle { get; set; } = string.Empty;
    public string SurveyDescription { get; set; } = string.Empty;
    public SurveyStateInfoDto SurveyState { get; set; } = new();
    public List<QuestionDetailsDto> Questions { get; set; } = new();
    public UserAnswerStatusDto? UserAnswerStatus { get; set; }
    public SurveyStatisticsDto? Statistics { get; set; }
    public int TotalQuestions { get; set; }
    public int RequiredQuestions { get; set; }
}

public class UserAnswerInfoDto
{
    public bool HasAnswered { get; set; }
    public bool IsComplete { get; set; }
    public DateTime? AnsweredAt { get; set; }
    public string? TextAnswer { get; set; }
    public List<Guid> SelectedOptionIds { get; set; } = new();
    public List<QuestionOptionDto> SelectedOptions { get; set; } = new();
    public int AttemptNumber { get; set; }
}

public class UserAnswerStatusDto
{
    public Guid? ResponseId { get; set; }
    public int? AttemptNumber { get; set; }
    public DateTime? LastAnsweredAt { get; set; }
    public int AnsweredQuestions { get; set; }
    public int TotalQuestions { get; set; }
    public decimal CompletionPercentage { get; set; }
    public bool IsComplete { get; set; }
    public bool CanContinue { get; set; }
    public string? StatusMessage { get; set; }
}

public class SurveyStateInfoDto
{
    public string State { get; set; } = string.Empty;
    public string StateText { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsAcceptingResponses { get; set; }
    public DateTime? StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public bool IsAnonymous { get; set; }
    public ParticipationPolicyDto ParticipationPolicy { get; set; } = new();
}


