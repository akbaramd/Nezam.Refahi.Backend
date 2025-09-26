using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.BasicDefinitions.Domain.Events;

/// <summary>
/// Domain event raised when a feature is updated
/// </summary>
public class FeaturesUpdatedEvent : DomainEvent
{
    public string FeatureId { get; }
    public string Title { get; }
    public string Type { get; }
    public DateTime OccurredAt { get; }

    public FeaturesUpdatedEvent(
        string featureId,
        string title,
        string type)
    {
        FeatureId = featureId;
        Title = title;
        Type = type;
        OccurredAt = DateTime.UtcNow;
    }
}
