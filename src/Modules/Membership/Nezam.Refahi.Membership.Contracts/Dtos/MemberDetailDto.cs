namespace Nezam.Refahi.Membership.Contracts.Dtos;

/// <summary>
/// Detailed Data Transfer Object for Member entity - used for detailed member information
/// Inherits from MemberDto and adds relational data (capabilities, features, roles, agencies)
/// This DTO is suitable for detail views and single entity queries
/// </summary>
public class MemberDetailDto : MemberDto
{
    // Relational Data - Collections
    /// <summary>
    /// Member capabilities (string keys) - relational data
    /// </summary>
    public List<string> Capabilities { get; set; } = new();
    
    /// <summary>
    /// Member features (string keys) - relational data
    /// </summary>
    public List<string> Features { get; set; } = new();
    
    /// <summary>
    /// Member roles (role IDs) - relational data
    /// </summary>
    public List<Guid> RoleIds { get; set; } = new();
    
    /// <summary>
    /// Member representative offices (agency IDs) - relational data
    /// </summary>
    public List<Guid> AgencyIds { get; set; } = new();
    
    // Additional Detail Fields
    /// <summary>
    /// External User ID linking to Identity context
    /// </summary>
    public Guid ExternalUserId { get; set; }
    
    /// <summary>
    /// Membership start date
    /// </summary>
    public DateTime? MembershipStartDate { get; set; }
    
    /// <summary>
    /// Membership end date
    /// </summary>
    public DateTime? MembershipEndDate { get; set; }
    
    /// <summary>
    /// Address information
    /// </summary>
    public string? Address { get; set; }
    
    /// <summary>
    /// Additional notes
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Creator user identifier
    /// </summary>
    public string? CreatedBy { get; set; }
    
    /// <summary>
    /// Modifier user identifier
    /// </summary>
    public string? ModifiedBy { get; set; }
}
