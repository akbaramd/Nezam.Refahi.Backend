using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nezam.New.EES.Models;

public class ContractAudit
{
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// ID of the contract this audit entry is for
    /// </summary>
    public int ContractId { get; set; }
    
    /// <summary>
    /// Type of action (Created, Updated, Signed, Rejected, etc.)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string ActionType { get; set; } = string.Empty;
    
    /// <summary>
    /// Description of the action
    /// </summary>
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// ID of the user who performed the action
    /// </summary>
    [MaxLength(50)]
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// Username of the user who performed the action
    /// </summary>
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// IP address from which the action was performed
    /// </summary>
    [MaxLength(50)]
    public string? IpAddress { get; set; }
    
    /// <summary>
    /// Additional data related to the action (JSON)
    /// </summary>
    public string? Data { get; set; }
    
    /// <summary>
    /// When this action was performed
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Navigation property to parent contract
    /// </summary>
    [ForeignKey("ContractId")]
    public virtual DigitalContract Contract { get; set; } = null!;
} 