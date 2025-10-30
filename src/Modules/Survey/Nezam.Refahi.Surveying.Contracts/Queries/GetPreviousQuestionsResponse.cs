using Nezam.Refahi.Surveying.Contracts.Dtos;

namespace Nezam.Refahi.Surveying.Contracts.Queries;

/// <summary>
/// Response for GetPreviousQuestionsQuery
/// </summary>
public class GetPreviousQuestionsResponse
{
    /// <summary>
    /// Survey basic information
    /// </summary>
    public SurveyBasicInfoDto Survey { get; set; } = null!;

    /// <summary>
    /// List of previous questions
    /// </summary>
    public List<PreviousQuestionDto> PreviousQuestions { get; set; } = new();

    /// <summary>
    /// Navigation information
    /// </summary>
    public PreviousQuestionsNavigationDto Navigation { get; set; } = null!;

    /// <summary>
    /// User's response status
    /// </summary>
    public UserResponseStatusDto? UserResponseStatus { get; set; }
}

/// <summary>
/// Previous question with user's answer
/// </summary>
public class PreviousQuestionDto
{
    /// <summary>
    /// Question details
    /// </summary>
    public QuestionDetailsDto Question { get; set; } = null!;

    /// <summary>
    /// Question index in survey
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// User's answer for this question (if available)
    /// </summary>
    public QuestionAnswerDto? UserAnswer { get; set; }

    /// <summary>
    /// Is this question answered by user
    /// </summary>
    public bool IsAnswered { get; set; }

    /// <summary>
    /// Can user navigate back to this question
    /// </summary>
    public bool CanNavigateTo { get; set; }
}

/// <summary>
/// Navigation information for previous questions
/// </summary>
public class PreviousQuestionsNavigationDto
{
    /// <summary>
    /// Current question index
    /// </summary>
    public int CurrentIndex { get; set; }

    /// <summary>
    /// Total number of questions in survey
    /// </summary>
    public int TotalQuestions { get; set; }

    /// <summary>
    /// Number of previous questions returned
    /// </summary>
    public int PreviousQuestionsCount { get; set; }

    /// <summary>
    /// Number of previous questions available (not returned due to MaxCount limit)
    /// </summary>
    public int MorePreviousAvailable { get; set; }

    /// <summary>
    /// Can navigate to previous questions
    /// </summary>
    public bool CanNavigatePrevious { get; set; }

    /// <summary>
    /// Progress information
    /// </summary>
    public ProgressInfoDto Progress { get; set; } = null!;
}

/// <summary>
/// Progress information
/// </summary>
public class ProgressInfoDto
{
    /// <summary>
    /// Questions answered count
    /// </summary>
    public int QuestionsAnswered { get; set; }

    /// <summary>
    /// Questions remaining count
    /// </summary>
    public int QuestionsRemaining { get; set; }

    /// <summary>
    /// Progress percentage (0-100)
    /// </summary>
    public int ProgressPercentage { get; set; }

    /// <summary>
    /// Required questions answered count
    /// </summary>
    public int RequiredQuestionsAnswered { get; set; }

    /// <summary>
    /// Required questions remaining count
    /// </summary>
    public int RequiredQuestionsRemaining { get; set; }
}
