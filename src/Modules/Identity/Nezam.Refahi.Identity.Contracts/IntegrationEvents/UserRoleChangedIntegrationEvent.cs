using MediatR;

namespace Nezam.Refahi.Identity.Contracts.IntegrationEvents;

/// <summary>
/// Integration event published when a user's roles are changed
/// </summary>
public class UserRoleChangedIntegrationEvent : INotification
{
    public Guid UserId { get; set; }
    public string? PhoneNumber { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? NationalCode { get; set; }
    public List<string> AddedRoles { get; set; } = new();
    public List<string> RemovedRoles { get; set; } = new();
    public List<string> CurrentRoles { get; set; } = new();
    public DateTime ChangedAt { get; set; }
    public string ChangedBy { get; set; } = string.Empty;
}
