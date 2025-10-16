
using MediatR;

namespace Nezam.Refahi.Identity.Contracts.IntegrationEvents;

/// <summary>
/// Integration event published when a user is updated
/// </summary>
public class UserUpdatedIntegrationEvent : INotification
{
    public Guid UserId { get; set; }
    public string? PhoneNumber { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? NationalCode { get; set; }
    public bool IsActive { get; set; }
    public bool IsPhoneVerified { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public List<string> Claims { get; set; } = new();
    public Dictionary<string, string> Preferences { get; set; } = new();
    public List<string> ChangedFields { get; set; } = new();
}
