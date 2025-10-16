using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Surveying.Domain.Events;

/// <summary>
/// Domain event raised when survey structure is unfrozen
/// </summary>
public sealed class SurveyStructureUnfrozenEvent : DomainEvent
{
    public Guid SurveyId { get; }
    public int StructureVersion { get; }
    public DateTimeOffset UnfrozenAt { get; }

    public SurveyStructureUnfrozenEvent(Guid surveyId, int structureVersion)
    {
        SurveyId = surveyId;
        StructureVersion = structureVersion;
        UnfrozenAt = DateTimeOffset.UtcNow;
    }
}
