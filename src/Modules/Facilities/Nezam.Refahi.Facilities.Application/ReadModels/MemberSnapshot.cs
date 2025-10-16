namespace Nezam.Refahi.Facilities.Application.ReadModels;

/// <summary>
/// Read Model for Member data snapshot in Facilities context
/// This acts as an Anti-Corruption Layer (ACL) for Membership context data
/// </summary>
public class MemberSnapshot
{
    /// <summary>
    /// Unique identifier for the snapshot
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// External User ID from Identity context
    /// </summary>
    public Guid ExternalUserId { get; set; }
    
    /// <summary>
    /// Member ID from Membership context
    /// </summary>
    public Guid MemberId { get; set; }
    
    /// <summary>
    /// Membership number
    /// </summary>
    public string MembershipNumber { get; set; } = null!;
    
    /// <summary>
    /// National ID
    /// </summary>
    public string NationalId { get; set; } = null!;
    
    /// <summary>
    /// Full name
    /// </summary>
    public string FullName { get; set; } = null!;
    
    /// <summary>
    /// Email address
    /// </summary>
    public string Email { get; set; } = null!;
    
    /// <summary>
    /// Phone number
    /// </summary>
    public string PhoneNumber { get; set; } = null!;
    
    /// <summary>
    /// Birth date
    /// </summary>
    public DateTime? BirthDate { get; set; }
    
    /// <summary>
    /// Member features (capabilities)
    /// </summary>
    public List<string> Features { get; set; } = new();
    
    /// <summary>
    /// Member capabilities
    /// </summary>
    public List<string> Capabilities { get; set; } = new();
    
    /// <summary>
    /// Member roles
    /// </summary>
    public List<string> Roles { get; set; } = new();
    
    /// <summary>
    /// Whether member is active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Snapshot creation timestamp
    /// </summary>
    public DateTime SnapshotCreatedAt { get; set; }
    
    /// <summary>
    /// Last update timestamp from source
    /// </summary>
    public DateTime LastUpdatedAt { get; set; }
}
