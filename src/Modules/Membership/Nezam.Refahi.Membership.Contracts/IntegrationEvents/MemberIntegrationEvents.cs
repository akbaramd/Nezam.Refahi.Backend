using MediatR;

namespace Nezam.Refahi.Membership.Contracts.IntegrationEvents;

/// <summary>
/// Integration event published when a member is created in Membership context
/// Other contexts can subscribe to this event to create their own snapshots
/// </summary>
public class MemberCreatedIntegrationEvent : INotification
{
    public Guid MemberId { get; set; }
    public Guid ExternalUserId { get; set; }
    public string MembershipNumber { get; set; } = null!;
    public string NationalId { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public DateTime? BirthDate { get; set; }
    public List<string> Features { get; set; } = new();
    public List<string> Capabilities { get; set; } = new();
    public List<string> Roles { get; set; } = new();
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CorrelationId { get; set; } = null!;
}

/// <summary>
/// Integration event published when a member is updated in Membership context
/// </summary>
public class MemberUpdatedIntegrationEvent : INotification
{
    public Guid MemberId { get; set; }
    public Guid ExternalUserId { get; set; }
    public string MembershipNumber { get; set; } = null!;
    public string NationalId { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public DateTime? BirthDate { get; set; }
    public List<string> Features { get; set; } = new();
    public List<string> Capabilities { get; set; } = new();
    public List<string> Roles { get; set; } = new();
    public bool IsActive { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string CorrelationId { get; set; } = null!;
}

/// <summary>
/// Integration event published when a member is deactivated in Membership context
/// </summary>
public class MemberDeactivatedIntegrationEvent : INotification
{
    public Guid MemberId { get; set; }
    public Guid ExternalUserId { get; set; }
    public DateTime DeactivatedAt { get; set; }
    public string CorrelationId { get; set; } = null!;
}
