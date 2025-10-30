using MCA.SharedKernel.Domain;
using Nezam.Refahi.Surveying.Domain.ValueObjects;
using Nezam.Refahi.Surveying.Domain.Enums;

namespace Nezam.Refahi.Surveying.Domain.Entities;

/// <summary>
/// Response entity representing a survey response
/// Internal entity of Survey aggregate root
/// </summary>
public sealed class Response : Entity<Guid>
{
    public Guid SurveyId { get; private set; }
    public ParticipantInfo Participant { get; private set; } = null!;
    public int AttemptNumber { get; private set; }
    public AttemptStatus AttemptStatus { get; private set; }
    public ResponseStatus Status { get; private set; }
    public DemographySnapshot? DemographySnapshot { get; private set; }
    public DateTimeOffset? SubmittedAt { get; private set; }
    public DateTimeOffset? CanceledAt { get; private set; }
    public DateTimeOffset? ExpiredAt { get; private set; }

    // Navigation properties
    private readonly List<QuestionAnswer> _questionAnswers = new();
    public IReadOnlyCollection<QuestionAnswer> QuestionAnswers => _questionAnswers.AsReadOnly();

    // Private constructor for EF Core
    private Response() : base() { }

    /// <summary>
    /// Creates a new response
    /// </summary>
    public Response(
        Guid surveyId,
        ParticipantInfo participant,
        int attemptNumber,
        DemographySnapshot? demographySnapshot = null)
        : base(Guid.NewGuid())
    {
        if (surveyId == Guid.Empty)
            throw new ArgumentException("Survey ID cannot be empty", nameof(surveyId));

        if (participant == null)
            throw new ArgumentNullException(nameof(participant));

        if (attemptNumber < 1)
            throw new ArgumentException("Attempt number must be at least 1", nameof(attemptNumber));

        SurveyId = surveyId;
        Participant = participant;
        AttemptNumber = attemptNumber;
        AttemptStatus = AttemptStatus.Active;
        Status = ResponseStatus.Answering;
        DemographySnapshot = demographySnapshot;
    }

    /// <summary>
    /// Adds or updates an answer for a specific question
    /// </summary>
    public void SetQuestionAnswer(Guid questionId, string? textAnswer = null, Dictionary<Guid,string>? selectedOptionIds = null, int repeatIndex = 1)
    {
        if (questionId == Guid.Empty)
            throw new ArgumentException("Question ID cannot be empty", nameof(questionId));

        if (repeatIndex < 1)
            throw new ArgumentException("Repeat index must be at least 1", nameof(repeatIndex));

        if (!CanBeModified())
            throw new InvalidOperationException($"Response cannot be modified in current status: {Status}. Only responses in 'Answering' or 'Reviewing' status can be modified.");

        // Find existing answer for this repeat index or create new one
        var existingAnswer = _questionAnswers.FirstOrDefault(qa => qa.QuestionId == questionId && qa.RepeatIndex == repeatIndex);
        if (existingAnswer == null)
        {
            existingAnswer = new QuestionAnswer(Id, questionId, repeatIndex);
            _questionAnswers.Add(existingAnswer);
        }

        // Set text answer if provided
        if (!string.IsNullOrWhiteSpace(textAnswer))
        {
            existingAnswer.SetTextAnswer(textAnswer);
        }

        // Set selected options if provided
        if (selectedOptionIds != null)
        {
            existingAnswer.ClearSelectedOptions();
            foreach (var optionId in selectedOptionIds)
            {
                if (optionId.Key != Guid.Empty)
                {
                    existingAnswer.AddSelectedOption(optionId.Key, optionId.Value); // TODO: Get actual option text
                }
            }
        }
    }

    /// <summary>
    /// Adds or updates an answer for a specific question with proper option texts
    /// </summary>
    public void SetQuestionAnswerWithOptionTexts(Guid questionId, string? textAnswer = null, IEnumerable<Guid>? selectedOptionIds = null, Dictionary<Guid, string>? optionTexts = null, int repeatIndex = 1)
    {
        if (questionId == Guid.Empty)
            throw new ArgumentException("Question ID cannot be empty", nameof(questionId));

        if (repeatIndex < 1)
            throw new ArgumentException("Repeat index must be at least 1", nameof(repeatIndex));

        // Find existing answer for this repeat index or create new one
        var existingAnswer = _questionAnswers.FirstOrDefault(qa => qa.QuestionId == questionId && qa.RepeatIndex == repeatIndex);
        if (existingAnswer == null)
        {
            existingAnswer = new QuestionAnswer(Id, questionId, repeatIndex);
            _questionAnswers.Add(existingAnswer);
        }

        // Set text answer if provided
        if (!string.IsNullOrWhiteSpace(textAnswer))
        {
            existingAnswer.SetTextAnswer(textAnswer);
        }

        // Set selected options if provided
        if (selectedOptionIds != null)
        {
            existingAnswer.ClearSelectedOptions();
            foreach (var optionId in selectedOptionIds)
            {
                if (optionId != Guid.Empty)
                {
                    var optionText = optionTexts?.GetValueOrDefault(optionId) ?? "Option Text";
                    existingAnswer.AddSelectedOption(optionId, optionText);
                }
            }
        }
    }

    /// <summary>
    /// Adds a selected option to the response for a specific question
    /// </summary>
    public void AddSelectedOption(Guid questionId, Guid optionId)
    {
        if (questionId == Guid.Empty)
            throw new ArgumentException("Question ID cannot be empty", nameof(questionId));

        if (optionId == Guid.Empty)
            throw new ArgumentException("Option ID cannot be empty", nameof(optionId));

        // Find existing answer or create new one
        var existingAnswer = _questionAnswers.FirstOrDefault(qa => qa.QuestionId == questionId);
        if (existingAnswer == null)
        {
            existingAnswer = new QuestionAnswer(Id, questionId);
            _questionAnswers.Add(existingAnswer);
        }

        // This method should not be used directly - use AddSelectedOptionWithText instead
        throw new InvalidOperationException("Use AddSelectedOptionWithText method to provide proper option text");
    }

    /// <summary>
    /// Updates selected options for a specific question with proper option texts
    /// </summary>
    public void UpdateSelectedOptions(Guid questionId, IEnumerable<Guid>? selectedOptionIds, Dictionary<Guid, string> optionTexts)
    {
        if (questionId == Guid.Empty)
            throw new ArgumentException("Question ID cannot be empty", nameof(questionId));

        // Find existing answer or create new one
        var existingAnswer = _questionAnswers.FirstOrDefault(qa => qa.QuestionId == questionId);
        if (existingAnswer == null)
        {
            existingAnswer = new QuestionAnswer(Id, questionId);
            _questionAnswers.Add(existingAnswer);
        }

        // Set selected options if provided
        if (selectedOptionIds != null)
        {
            existingAnswer.ClearSelectedOptions();
            foreach (var optionId in selectedOptionIds)
            {
                if (optionId != Guid.Empty && optionTexts.TryGetValue(optionId, out var optionText))
                {
                    existingAnswer.AddSelectedOption(optionId, optionText);
                }
            }
        }
    }

    /// <summary>
    /// Adds a selected option to the response with proper option text
    /// </summary>
    public void AddSelectedOptionWithText(Guid questionId, Guid optionId, string optionText)
    {
        if (questionId == Guid.Empty)
            throw new ArgumentException("Question ID cannot be empty", nameof(questionId));

        if (optionId == Guid.Empty)
            throw new ArgumentException("Option ID cannot be empty", nameof(optionId));

        if (string.IsNullOrWhiteSpace(optionText))
            throw new ArgumentException("Option text cannot be empty", nameof(optionText));

        // Find existing answer or create new one
        var existingAnswer = _questionAnswers.FirstOrDefault(qa => qa.QuestionId == questionId);
        if (existingAnswer == null)
        {
            existingAnswer = new QuestionAnswer(Id, questionId);
            _questionAnswers.Add(existingAnswer);
        }

        existingAnswer.AddSelectedOption(optionId, optionText);
    }

    /// <summary>
    /// Adds a text answer to the response for a specific question
    /// </summary>
    public void AddTextAnswer(Guid questionId, string text)
    {
        if (questionId == Guid.Empty)
            throw new ArgumentException("Question ID cannot be empty", nameof(questionId));

        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text answer cannot be empty", nameof(text));

        // Find existing answer or create new one
        var existingAnswer = _questionAnswers.FirstOrDefault(qa => qa.QuestionId == questionId);
        if (existingAnswer == null)
        {
            existingAnswer = new QuestionAnswer(Id, questionId);
            _questionAnswers.Add(existingAnswer);
        }

        existingAnswer.SetTextAnswer(text);
    }

    /// <summary>
    /// Updates the demographic snapshot
    /// </summary>
    public void UpdateDemographySnapshot(DemographySnapshot snapshot)
    {
        DemographySnapshot = snapshot ?? throw new ArgumentNullException(nameof(snapshot));
    }

    /// <summary>
    /// Gets selected options for a specific question
    /// </summary>
    public IEnumerable<Guid> GetSelectedOptionsForQuestion(Guid questionId)
    {
        var questionAnswer = _questionAnswers.FirstOrDefault(qa => qa.QuestionId == questionId);
        return questionAnswer?.GetSelectedOptionIds() ?? Enumerable.Empty<Guid>();
    }

    /// <summary>
    /// Gets text answer for a specific question
    /// </summary>
    public string? GetTextAnswerForQuestion(Guid questionId)
    {
        var questionAnswer = _questionAnswers.FirstOrDefault(qa => qa.QuestionId == questionId);
        return questionAnswer?.TextAnswer;
    }

    /// <summary>
    /// Gets the answer for a specific question
    /// </summary>
    public QuestionAnswer? GetQuestionAnswer(Guid questionId)
    {
        return _questionAnswers.FirstOrDefault(qa => qa.QuestionId == questionId);
    }

    /// <summary>
    /// Gets the answer for a specific question and repeat index
    /// </summary>
    public QuestionAnswer? GetQuestionAnswer(Guid questionId, int repeatIndex)
    {
        return _questionAnswers.FirstOrDefault(qa => qa.QuestionId == questionId && qa.RepeatIndex == repeatIndex);
    }

    /// <summary>
    /// Gets all answers for a specific question (all repeat indices)
    /// </summary>
    public IEnumerable<QuestionAnswer> GetQuestionAnswers(Guid questionId)
    {
        return _questionAnswers.Where(qa => qa.QuestionId == questionId).OrderBy(qa => qa.RepeatIndex);
    }

    /// <summary>
    /// Gets the count of answered repeats for a specific question
    /// </summary>
    public int GetAnsweredRepeatCount(Guid questionId)
    {
        return _questionAnswers.Count(qa => qa.QuestionId == questionId && qa.HasAnswer());
    }

    /// <summary>
    /// Gets all repeat indices that have been answered for a specific question
    /// </summary>
    public IEnumerable<int> GetAnsweredRepeatIndices(Guid questionId)
    {
        return _questionAnswers
            .Where(qa => qa.QuestionId == questionId && qa.HasAnswer())
            .Select(qa => qa.RepeatIndex)
            .OrderBy(index => index);
    }

    /// <summary>
    /// Checks if the response has an answer for a specific question
    /// </summary>
    public bool HasAnswerForQuestion(Guid questionId)
    {
        var questionAnswer = _questionAnswers.FirstOrDefault(qa => qa.QuestionId == questionId);
        return questionAnswer?.HasAnswer() ?? false;
    }

    /// <summary>
    /// Checks if the response has an answer for a specific question and repeat index
    /// </summary>
    public bool HasAnswerForQuestion(Guid questionId, int repeatIndex)
    {
        var questionAnswer = _questionAnswers.FirstOrDefault(qa => qa.QuestionId == questionId && qa.RepeatIndex == repeatIndex);
        return questionAnswer?.HasAnswer() ?? false;
    }

    /// <summary>
    /// Gets all answered question IDs
    /// </summary>
    public IEnumerable<Guid> GetAnsweredQuestionIds()
    {
        return _questionAnswers
            .Where(qa => qa.HasAnswer())
            .Select(qa => qa.QuestionId);
    }

    /// <summary>
    /// Submits the response
    /// </summary>
    public void Submit()
    {
        if (AttemptStatus != AttemptStatus.Active)
            throw new InvalidOperationException("Only active attempts can be submitted");

        SubmittedAt = DateTimeOffset.UtcNow;
        AttemptStatus = AttemptStatus.Submitted;
        Status = ResponseStatus.Completed;
    }

    /// <summary>
    /// Cancels the response attempt
    /// </summary>
    public void Cancel()
    {
        if (AttemptStatus != AttemptStatus.Active)
            throw new InvalidOperationException("Only active attempts can be canceled");

        CanceledAt = DateTimeOffset.UtcNow;
        AttemptStatus = AttemptStatus.Canceled;
        Status = ResponseStatus.Cancelled;
    }

    /// <summary>
    /// Marks the response as expired
    /// </summary>
    public void Expire()
    {
        if (AttemptStatus != AttemptStatus.Active)
            throw new InvalidOperationException("Only active attempts can be expired");

        ExpiredAt = DateTimeOffset.UtcNow;
        AttemptStatus = AttemptStatus.Expired;
        Status = ResponseStatus.Expired;
    }

    /// <summary>
    /// Changes the response status to reviewing
    /// </summary>
    public void StartReviewing()
    {
        if (Status != ResponseStatus.Answering)
            throw new InvalidOperationException("Only responses in answering status can be moved to reviewing");

        Status = ResponseStatus.Reviewing;
    }

    /// <summary>
    /// Changes the response status back to answering from reviewing
    /// </summary>
    public void ResumeAnswering()
    {
        if (Status != ResponseStatus.Reviewing)
            throw new InvalidOperationException("Only responses in reviewing status can be resumed to answering");

        Status = ResponseStatus.Answering;
    }

    /// <summary>
    /// Checks if the response can be modified
    /// </summary>
    public bool CanBeModified()
    {
        return Status == ResponseStatus.Answering || Status == ResponseStatus.Reviewing;
    }

    /// <summary>
    /// Checks if the response is in reviewing state
    /// </summary>
    public bool IsReviewing()
    {
        return Status == ResponseStatus.Reviewing;
    }

    /// <summary>
    /// Checks if the response is completed
    /// </summary>
    public bool IsCompleted()
    {
        return Status == ResponseStatus.Completed;
    }

    /// <summary>
    /// Checks if the response is in a mutable state (can be modified)
    /// </summary>
    public bool IsMutable => AttemptStatus == AttemptStatus.Active;

    /// <summary>
    /// Checks if the response has been submitted
    /// </summary>
    public bool IsSubmitted => AttemptStatus == AttemptStatus.Submitted;

    /// <summary>
    /// Checks if the response is active
    /// </summary>
    public bool IsActive => AttemptStatus == AttemptStatus.Active;
}
