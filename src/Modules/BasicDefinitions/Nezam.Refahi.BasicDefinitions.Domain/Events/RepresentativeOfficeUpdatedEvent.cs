using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.BasicDefinitions.Domain.Events;

/// <summary>
/// Domain event raised when a representative office is updated
/// </summary>
public class AgencyUpdatedEvent : DomainEvent
{
    public Guid OfficeId { get; }
    public string Code { get; }
    public string Name { get; }
    public string ExternalCode { get; }
    public string Address { get; }
    public string? ManagerName { get; }
    public string? ManagerPhone { get; }
    public bool IsActive { get; }
    public DateTime? EstablishedDate { get; }
    public DateTime OccurredAt { get; }

    public AgencyUpdatedEvent(
        Guid officeId,
        string code,
        string name,
        string externalCode,
        string address,
        string? managerName = null,
        string? managerPhone = null,
        bool isActive = true,
        DateTime? establishedDate = null)
    {
        OfficeId = officeId;
        Code = code;
        Name = name;
        ExternalCode = externalCode;
        Address = address;
        ManagerName = managerName;
        ManagerPhone = managerPhone;
        IsActive = isActive;
        EstablishedDate = establishedDate;
        OccurredAt = DateTime.UtcNow;
    }
}
