using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Surveying.Domain.Events;

/// <summary>
/// Domain event raised when survey structure is frozen
/// </summary>
public sealed class SurveyStructureFrozenEvent : DomainEvent
{
    public Guid SurveyId { get; }
    public int StructureVersion { get; }
    public DateTimeOffset FrozenAt { get; }

    public SurveyStructureFrozenEvent(Guid surveyId, int structureVersion)
    {
        SurveyId = surveyId;
        StructureVersion = structureVersion;
        FrozenAt = DateTimeOffset.UtcNow;
    }
}
