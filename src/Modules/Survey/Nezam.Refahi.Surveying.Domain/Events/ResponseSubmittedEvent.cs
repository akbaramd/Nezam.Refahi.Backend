using MCA.SharedKernel.Domain.Events;
using Nezam.Refahi.Surveying.Domain.ValueObjects;

namespace Nezam.Refahi.Surveying.Domain.Events;

/// <summary>
/// Domain event raised when a survey response is submitted
/// Contains minimal data - derived metrics should be calculated in projections
/// </summary>
public sealed class ResponseSubmittedEvent : DomainEvent
{
    public Guid SurveyId { get; }
    public Guid ResponseId { get; }
    public ParticipantInfo Participant { get; }
    public int AttemptNumber { get; }
    public DateTimeOffset SubmittedAt { get; }

    public ResponseSubmittedEvent(
        Guid surveyId,
        Guid responseId,
        ParticipantInfo participant,
        int attemptNumber)
    {
        SurveyId = surveyId;
        ResponseId = responseId;
        Participant = participant;
        AttemptNumber = attemptNumber;
        SubmittedAt = DateTimeOffset.UtcNow;
    }
}
