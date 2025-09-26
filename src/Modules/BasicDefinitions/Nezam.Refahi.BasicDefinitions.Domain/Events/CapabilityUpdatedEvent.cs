using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.BasicDefinitions.Domain.Events;

/// <summary>
/// Domain event raised when a capability is updated
/// </summary>
public class CapabilityUpdatedEvent : DomainEvent
{
    public string CapabilityId { get; }
    public string Name { get; }
    public string Description { get; }
    public bool IsActive { get; }
    public DateTime? ValidFrom { get; }
    public DateTime? ValidTo { get; }
    public DateTime OccurredAt { get; }

    public CapabilityUpdatedEvent(
        string capabilityId,
        string name,
        string description,
        bool isActive = true,
        DateTime? validFrom = null,
        DateTime? validTo = null)
    {
        CapabilityId = capabilityId;
        Name = name;
        Description = description;
        IsActive = isActive;
        ValidFrom = validFrom;
        ValidTo = validTo;
        OccurredAt = DateTime.UtcNow;
    }
}
