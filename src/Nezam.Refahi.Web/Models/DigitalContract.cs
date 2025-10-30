using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nezam.New.EES.Models;

public class DigitalContract
{
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// External ID from the app (if any)
    /// </summary>
    public int? ExternalId { get; set; }
    
    /// <summary>
    /// Unique identifier for the contract (reference number)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string ContractNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// Title of the contract
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// ID of the template this contract uses
    /// </summary>
    public int TemplateId { get; set; }
    
    /// <summary>
    /// Current status of the contract
    /// 0 = Draft
    /// 1 = Ready for Signature
    /// 2 = Partially Signed
    /// 3 = Completed
    /// 4 = Rejected
    /// </summary>
    public int Status { get; set; } = 0;
    
    /// <summary>
    /// Path to the original Word (DOCX) document
    /// </summary>
    [MaxLength(255)]
    public string? DocxPath { get; set; }
    
    /// <summary>
    /// Path to the final PDF document
    /// </summary>
    [MaxLength(255)]
    public string? PdfPath { get; set; }
    
    /// <summary>
    /// SHA256 hash of the signed PDF for verification
    /// </summary>
    [MaxLength(64)]
    public string? PdfHash { get; set; }
    
    /// <summary>
    /// Date when the contract was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// ID of the user who created this contract
    /// </summary>
    [MaxLength(50)]
    public string CreatedBy { get; set; } = string.Empty;
    
    /// <summary>
    /// Date when the contract was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// ID of the user who last updated this contract
    /// </summary>
    [MaxLength(50)]
    public string? UpdatedBy { get; set; }
    
    /// <summary>
    /// Date when the final signed contract was completed
    /// </summary>
    public DateTime? SignedAt { get; set; }
    
    /// <summary>
    /// Date when the contract expires
    /// </summary>
    public DateTime? ExpiresAt { get; set; }
    
    /// <summary>
    /// Navigation property to parent template
    /// </summary>
    [ForeignKey("TemplateId")]
    public virtual ContractTemplate Template { get; set; } = null!;
    
    /// <summary>
    /// List of parameters for this contract
    /// </summary>
    public virtual ICollection<ContractParameter> Parameters { get; set; } = new List<ContractParameter>();
    
    /// <summary>
    /// List of signers for this contract
    /// </summary>
    public virtual ICollection<ContractSigner> Signers { get; set; } = new List<ContractSigner>();
    
    /// <summary>
    /// Audit trail for this contract
    /// </summary>
    public virtual ICollection<ContractAudit> AuditTrail { get; set; } = new List<ContractAudit>();
} 