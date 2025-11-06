using MCA.SharedKernel.Domain.AggregateRoots;
using Nezam.Refahi.Surveying.Domain.Enums;
using Nezam.Refahi.Surveying.Domain.ValueObjects;
using Nezam.Refahi.Surveying.Domain.Events;

namespace Nezam.Refahi.Surveying.Domain.Entities;

/// <summary>
/// Survey aggregate root representing a survey with questions and responses
/// Single aggregate root following DDD principles
/// </summary>
public sealed class Survey : FullAggregateRoot<Guid>
{
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public SurveyState State { get; private set; }
    public DateTimeOffset? StartAt { get; private set; }
    public DateTimeOffset? EndAt { get; private set; }
    public bool IsAnonymous { get; private set; }
    public ParticipationPolicy ParticipationPolicy { get; private set; } = null!;
    public AudienceFilter? AudienceFilter { get; private set; }
    
    // Versioning and Freeze
    public int StructureVersion { get; private set; } = 1;
    public bool IsStructureFrozen { get; private set; }

    // Encapsulated child entities
    private readonly List<Question> _questions = new();
    private readonly List<Response> _responses = new();
    private readonly List<SurveyFeature> _surveyFeatures = new();
    private readonly List<SurveyCapability> _surveyCapabilities = new();

    // Read-only access to child entities
    public IReadOnlyCollection<Question> Questions => _questions.AsReadOnly();
    public IReadOnlyCollection<Response> Responses => _responses.AsReadOnly();
    public IReadOnlyCollection<SurveyFeature> SurveyFeatures => _surveyFeatures.AsReadOnly();
    public IReadOnlyCollection<SurveyCapability> SurveyCapabilities => _surveyCapabilities.AsReadOnly();

    // Private constructor for EF Core
    private Survey() : base() { }

    /// <summary>
    /// Creates a new survey
    /// </summary>
    public Survey(
        string title,
        string? description,
        bool isAnonymous,
        ParticipationPolicy participationPolicy,
        AudienceFilter? audienceFilter = null,
        DateTimeOffset? startAt = null,
        DateTimeOffset? endAt = null)
        : base(Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        if (startAt.HasValue && endAt.HasValue && startAt.Value > endAt.Value)
            throw new ArgumentException("Start date cannot be after end date");

        Title = title.Trim();
        Description = description?.Trim();
        State = SurveyState.Draft;
        IsAnonymous = isAnonymous;
        ParticipationPolicy = participationPolicy ?? throw new ArgumentNullException(nameof(participationPolicy));
        AudienceFilter = audienceFilter;
        StartAt = startAt;
        EndAt = endAt;
    }

    /// <summary>
    /// Adds a question to the survey using specification
    /// </summary>
    public Question AddQuestion(QuestionSpecification specification)
    {
        if (IsStructureFrozen)
            throw new InvalidOperationException("Cannot modify survey structure when frozen");

        if (State != SurveyState.Draft)
            throw new InvalidOperationException("Can only add questions to draft surveys");

        if (specification == null)
            throw new ArgumentNullException(nameof(specification));

        var question = new Question(Id, specification);
        _questions.Add(question);
        
        // Don't enforce invariants here - options will be added later
        // Invariants will be enforced when survey is activated or when needed
        return question;
    }

    /// <summary>
    /// Starts a new response for a participant
    /// </summary>
    public Response StartResponse(ParticipantInfo participant, DemographySnapshot? demographySnapshot = null)
    {
        if (!CanParticipantSubmit(participant))
            throw new InvalidOperationException("Participant cannot submit response");

        var attemptNumber = GetNextAttemptNumber(participant);
        var response = new Response(Id, participant, attemptNumber, demographySnapshot);
        _responses.Add(response);
        
        // Initialize navigation to first question
        var orderedQuestions = GetOrderedQuestionsList();
        if (orderedQuestions.Any())
        {
            response.NavigateToQuestion(orderedQuestions, questionId: null, repeatIndex: null, isFirst: true);
        }
        
        return response;
    }

    /// <summary>
    /// Submits a response and raises domain event
    /// </summary>
    public void SubmitResponse(Guid responseId)
    {
        var response = _responses.FirstOrDefault(r => r.Id == responseId);
        if (response == null)
            throw new ArgumentException("Response not found", nameof(responseId));

        if (!ValidateResponseComplete(response))
            throw new InvalidOperationException("Response is not complete");

        response.Submit();
        AddDomainEvent(new ResponseSubmittedEvent(Id, responseId, response.Participant, response.AttemptNumber));
    }

    /// <summary>
    /// Sets an answer for a specific question in a response
    /// </summary>
    public void SetResponseAnswer(Guid responseId, Guid questionId, string questionText, string? textAnswer = null, Dictionary<Guid,string>? selectedOptionIds = null)
    {
        var response = _responses.FirstOrDefault(r => r.Id == responseId);
        if (response == null)
            throw new ArgumentException("Response not found", nameof(responseId));

        if (!response.IsMutable)
            throw new InvalidOperationException("Cannot modify submitted response");

        response.SetQuestionAnswer(questionId, textAnswer, selectedOptionIds);
    }

    /// <summary>
    /// Checks if a participant can submit a response
    /// </summary>
    public bool CanParticipantSubmit(ParticipantInfo participant)
    {
        if (!IsAcceptingResponses())
            return false;

        var existingResponses = _responses.Where(r => r.Participant.Equals(participant)).ToList();
        var attemptNumber = existingResponses.Count + 1;

        return ParticipationPolicy.IsAttemptAllowed(attemptNumber);
    }

    /// <summary>
    /// Gets the next attempt number for a participant
    /// </summary>
    private int GetNextAttemptNumber(ParticipantInfo participant)
    {
        var existingResponses = _responses.Where(r => r.Participant.Equals(participant)).ToList();
        return existingResponses.Count + 1;
    }

    /// <summary>
    /// Validates if a response is complete
    /// </summary>
    private bool ValidateResponseComplete(Response response)
    {
        var requiredQuestions = _questions.Where(q => q.IsRequired).ToList();
        return requiredQuestions.All(q => response.HasAnswerForQuestion(q.Id));
    }

    /// <summary>
    /// Enforces aggregate invariants
    /// </summary>
    private void EnforceInvariants()
    {
        // Enforce all business rules here
        if (State == SurveyState.Published && !_questions.Any())
            throw new InvalidOperationException("Published survey must have questions");

        // Validate all questions have required options for choice questions
        foreach (var question in _questions)
        {
            if (question.Kind != QuestionKind.Textual && !question.Options.Any())
                throw new InvalidOperationException($"Question '{question.Text}' must have options");

            // Validate FixedMCQ4 has exactly 4 options
            if (question.Kind == QuestionKind.FixedMCQ4 && question.Options.Count != 4)
                throw new InvalidOperationException($"FixedMCQ4 question '{question.Text}' must have exactly 4 options");
        }
    }

    /// <summary>
    /// Adds a feature link to the survey using stable code
    /// </summary>
    public void AddFeature(string featureCode, string? featureTitleSnapshot = null)
    {
        if (IsStructureFrozen)
            throw new InvalidOperationException("Survey structure is frozen and cannot be modified");

        if (string.IsNullOrWhiteSpace(featureCode))
            throw new ArgumentException("Feature code cannot be empty", nameof(featureCode));

        if (_surveyFeatures.Any(sf => sf.FeatureCode == featureCode))
            return; // Already linked

        var surveyFeature = new SurveyFeature(Id, featureCode, featureTitleSnapshot);
        _surveyFeatures.Add(surveyFeature);
    }

    /// <summary>
    /// Adds a capability link to the survey using stable code
    /// </summary>
    public void AddCapability(string capabilityCode, string? capabilityTitleSnapshot = null)
    {
        if (IsStructureFrozen)
            throw new InvalidOperationException("Survey structure is frozen and cannot be modified");

        if (string.IsNullOrWhiteSpace(capabilityCode))
            throw new ArgumentException("Capability code cannot be empty", nameof(capabilityCode));

        if (_surveyCapabilities.Any(sc => sc.CapabilityCode == capabilityCode))
            return; // Already linked

        var surveyCapability = new SurveyCapability(Id, capabilityCode, capabilityTitleSnapshot);
        _surveyCapabilities.Add(surveyCapability);
    }

    /// <summary>
    /// Publishes the survey, making it available for participation
    /// </summary>
    public void Publish()
    {
        if (State != SurveyState.Draft)
            throw new InvalidOperationException("Can only publish draft surveys");

        var now = DateTimeOffset.UtcNow;

        // Validate timing constraints if set
        if (StartAt.HasValue && now < StartAt.Value)
            throw new InvalidOperationException($"Survey cannot be published before start time: {StartAt.Value}");

        if (EndAt.HasValue && now >= EndAt.Value)
            throw new InvalidOperationException($"Survey cannot be published after end time: {EndAt.Value}");

        // Enforce invariants before publishing
        EnforceInvariants();

        State = SurveyState.Published;
    }

    /// <summary>
    /// Completes the survey (no longer accepting new responses)
    /// </summary>
    public void Complete()
    {
        if (State != SurveyState.Published)
            throw new InvalidOperationException("Can only complete published surveys");

        State = SurveyState.Completed;
    }

    /// <summary>
    /// Archives the survey
    /// </summary>
    public void Archive()
    {
        if (State != SurveyState.Completed)
            throw new InvalidOperationException("Can only archive completed surveys");

        State = SurveyState.Archived;
    }

    /// <summary>
    /// Checks if the survey is currently accepting responses
    /// </summary>
    public bool IsAcceptingResponses()
    {
        if (State != SurveyState.Published)
            return false;

        var now = DateTimeOffset.UtcNow;

        if (StartAt.HasValue && now < StartAt.Value)
            return false;

        if (EndAt.HasValue && now > EndAt.Value)
            return false;

        return true;
    }

    /// <summary>
    /// Checks if a participant can submit a response
    /// </summary>
    public bool CanParticipantSubmit(ParticipantInfo participant, int currentAttemptNumber, DateTime? lastAttemptTime = null)
    {
        if (!IsAcceptingResponses())
            return false;

        if (!ParticipationPolicy.IsAttemptAllowed(currentAttemptNumber))
            return false;

        if (!ParticipationPolicy.IsCoolDownPassed(lastAttemptTime))
            return false;

        return true;
    }

    /// <summary>
    /// Gets the question by ID
    /// </summary>
    public Question? GetQuestion(Guid questionId)
    {
        return _questions.FirstOrDefault(q => q.Id == questionId);
    }

    /// <summary>
    /// Gets the latest valid response for a participant
    /// Latest response is determined by SubmittedAt, not AttemptNumber
    /// Only considers Submitted responses as valid for "latest" determination
    /// </summary>
    public Response? GetLatestValidResponse(ParticipantInfo participant)
    {
        return _responses
            .Where(r => r.Participant.Equals(participant) && r.AttemptStatus == AttemptStatus.Submitted)
            .OrderByDescending(r => r.SubmittedAt ?? DateTimeOffset.MinValue)
            .FirstOrDefault();
    }

    /// <summary>
    /// Gets the latest response (any status) for a participant
    /// This includes Active, Submitted, Canceled, and Expired responses
    /// </summary>
    public Response? GetLatestResponse(ParticipantInfo participant)
    {
        return _responses
            .Where(r => r.Participant.Equals(participant))
            .OrderByDescending(r => r.SubmittedAt ?? r.CanceledAt ?? r.ExpiredAt ?? DateTimeOffset.MinValue)
            .FirstOrDefault();
    }

    /// <summary>
    /// Gets all responses for a participant ordered by submission time
    /// </summary>
    public IEnumerable<Response> GetParticipantResponses(ParticipantInfo participant)
    {
        return _responses
            .Where(r => r.Participant.Equals(participant))
            .OrderByDescending(r => r.SubmittedAt ?? r.CanceledAt ?? r.ExpiredAt ?? DateTimeOffset.MinValue);
    }

    /// <summary>
    /// Checks if participant has any submitted responses
    /// </summary>
    public bool HasParticipantSubmittedResponse(ParticipantInfo participant)
    {
        return _responses.Any(r => r.Participant.Equals(participant) && r.AttemptStatus == AttemptStatus.Submitted);
    }

    /// <summary>
    /// Gets the count of submitted responses for a participant
    /// </summary>
    public int GetParticipantSubmittedResponseCount(ParticipantInfo participant)
    {
        return _responses.Count(r => r.Participant.Equals(participant) && r.AttemptStatus == AttemptStatus.Submitted);
    }

    /// <summary>
    /// Gets questions ordered by their order property
    /// </summary>
    public IEnumerable<Question> GetOrderedQuestions()
    {
        return _questions.OrderBy(q => q.Order);
    }

    /// <summary>
    /// Gets questions as ordered list (for navigation)
    /// </summary>
    public IReadOnlyList<Question> GetOrderedQuestionsList()
    {
        return _questions.OrderBy(q => q.Order).ToList().AsReadOnly();
    }

    // ========== Navigation Behaviors (using Response) ==========

    /// <summary>
    /// Navigates response to a specific question by ID
    /// If questionId is null, navigates to first question (if isFirst=true) or last question (if isFirst=false)
    /// </summary>
    public void NavigateResponseToQuestion(Guid responseId, Guid? questionId = null, int? repeatIndex = null, bool isFirst = true)
    {
        var response = _responses.FirstOrDefault(r => r.Id == responseId);
        if (response == null)
            throw new ArgumentException("Response not found", nameof(responseId));

        if (response.SurveyId != Id)
            throw new ArgumentException("Response does not belong to this survey", nameof(responseId));

        var orderedQuestions = GetOrderedQuestionsList();
        response.NavigateToQuestion(orderedQuestions, questionId, repeatIndex, isFirst);
    }

    /// <summary>
    /// Navigates response to the next question
    /// </summary>
    public bool NavigateResponseToNext(Guid responseId)
    {
        var response = _responses.FirstOrDefault(r => r.Id == responseId);
        if (response == null)
            throw new ArgumentException("Response not found", nameof(responseId));

        if (response.SurveyId != Id)
            throw new ArgumentException("Response does not belong to this survey", nameof(responseId));

        var orderedQuestions = GetOrderedQuestionsList();
        return response.NavigateToNext(orderedQuestions);
    }

    /// <summary>
    /// Navigates response to the previous question
    /// </summary>
    public bool NavigateResponseToPrevious(Guid responseId)
    {
        var response = _responses.FirstOrDefault(r => r.Id == responseId);
        if (response == null)
            throw new ArgumentException("Response not found", nameof(responseId));

        if (response.SurveyId != Id)
            throw new ArgumentException("Response does not belong to this survey", nameof(responseId));

        var orderedQuestions = GetOrderedQuestionsList();
        return response.NavigateToPrevious(orderedQuestions);
    }

    /// <summary>
    /// Gets the current question for a response
    /// </summary>
    public Question? GetCurrentQuestionForResponse(Guid responseId)
    {
        var response = _responses.FirstOrDefault(r => r.Id == responseId);
        if (response == null)
            return null;

        if (response.SurveyId != Id)
            return null;

        var orderedQuestions = GetOrderedQuestionsList();
        return response.GetCurrentQuestion(orderedQuestions);
    }

    /// <summary>
    /// Gets the current navigation state for a response (question ID and repeat index)
    /// </summary>
    public (Guid? QuestionId, int RepeatIndex) GetCurrentNavigationState(Guid responseId)
    {
        var response = _responses.FirstOrDefault(r => r.Id == responseId);
        if (response == null)
            return (null, 1);

        return (response.CurrentQuestionId, response.CurrentRepeatIndex);
    }

    /// <summary>
    /// Gets the option text for a specific question and option ID
    /// </summary>
    public string? GetOptionText(Guid questionId, Guid optionId)
    {
        var question = _questions.FirstOrDefault(q => q.Id == questionId);
        if (question == null)
            return null;

        var option = question.Options.FirstOrDefault(o => o.Id == optionId);
        return option?.Text;
    }

    /// <summary>
    /// Gets all option texts for a specific question
    /// </summary>
    public Dictionary<Guid, string> GetQuestionOptionTexts(Guid questionId)
    {
        var question = _questions.FirstOrDefault(q => q.Id == questionId);
        if (question == null)
            return new Dictionary<Guid, string>();

        return question.Options.ToDictionary(o => o.Id, o => o.Text);
    }

    /// <summary>
    /// Updates a response with selected options, ensuring proper option text is used
    /// </summary>
    public void UpdateResponseWithSelectedOptions(Guid responseId, Guid questionId, IEnumerable<Guid> selectedOptionIds)
    {
        var response = _responses.FirstOrDefault(r => r.Id == responseId);
        if (response == null)
            throw new InvalidOperationException($"Response {responseId} not found");

        var optionTexts = GetQuestionOptionTexts(questionId);
        
        // Update the response with proper option texts
        response.UpdateSelectedOptions(questionId, selectedOptionIds, optionTexts);
    }

    /// <summary>
    /// Adds a selected option to a response with proper option text
    /// </summary>
    public void AddSelectedOptionToResponse(Guid responseId, Guid questionId, Guid optionId)
    {
        var response = _responses.FirstOrDefault(r => r.Id == responseId);
        if (response == null)
            throw new InvalidOperationException($"Response {responseId} not found");

        var optionText = GetOptionText(questionId, optionId);
        if (string.IsNullOrEmpty(optionText))
            throw new InvalidOperationException($"Option {optionId} not found for question {questionId}");

        response.AddSelectedOptionWithText(questionId, optionId, optionText);
    }

    /// <summary>
    /// Updates the survey title
    /// </summary>
    public void UpdateTitle(string title)
    {
        if (IsStructureFrozen)
            throw new InvalidOperationException("Survey structure is frozen and cannot be modified");

        if (State != SurveyState.Draft)
            throw new InvalidOperationException("Can only update title of draft surveys");

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        Title = title.Trim();
    }

    /// <summary>
    /// Updates the survey description
    /// </summary>
    public void UpdateDescription(string? description)
    {
        if (IsStructureFrozen)
            throw new InvalidOperationException("Survey structure is frozen and cannot be modified");

        if (State != SurveyState.Draft)
            throw new InvalidOperationException("Can only update description of draft surveys");

        Description = description?.Trim();
    }

    /// <summary>
    /// Freezes the survey structure to prevent further modifications
    /// </summary>
    public void FreezeStructure()
    {
        if (IsStructureFrozen)
            throw new InvalidOperationException("Survey structure is already frozen");

        if (State != SurveyState.Draft)
            throw new InvalidOperationException("Can only freeze draft surveys");

        IsStructureFrozen = true;
        StructureVersion++;
        AddDomainEvent(new SurveyStructureFrozenEvent(Id, StructureVersion));
    }

    /// <summary>
    /// Unfreezes the survey structure (only for draft surveys)
    /// </summary>
    public void UnfreezeStructure()
    {
        if (!IsStructureFrozen)
            throw new InvalidOperationException("Survey structure is not frozen");

        if (State != SurveyState.Draft)
            throw new InvalidOperationException("Can only unfreeze draft surveys");

        IsStructureFrozen = false;
        StructureVersion++;
        AddDomainEvent(new SurveyStructureUnfrozenEvent(Id, StructureVersion));
    }

    /// <summary>
    /// Updates the participation policy
    /// </summary>
    public void UpdateParticipationPolicy(ParticipationPolicy policy)
    {
        if (State != SurveyState.Draft)
            throw new InvalidOperationException("Can only update participation policy of draft surveys");

        ParticipationPolicy = policy ?? throw new ArgumentNullException(nameof(policy));
    }

    /// <summary>
    /// Updates the audience filter
    /// </summary>
    public void UpdateAudienceFilter(AudienceFilter? filter)
    {
        if (State != SurveyState.Draft)
            throw new InvalidOperationException("Can only update audience filter of draft surveys");

        AudienceFilter = filter;
    }
}
