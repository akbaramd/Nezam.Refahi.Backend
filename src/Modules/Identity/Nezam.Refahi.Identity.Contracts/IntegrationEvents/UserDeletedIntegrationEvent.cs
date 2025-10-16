using MediatR;

namespace Nezam.Refahi.Identity.Contracts.IntegrationEvents;

/// <summary>
/// Integration event published when a user is deleted
/// </summary>
public class UserDeletedIntegrationEvent : INotification
{
    public Guid UserId { get; set; }
    public string? PhoneNumber { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? NationalCode { get; set; }
    public DateTime DeletedAt { get; set; }
    public string DeletedBy { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}
