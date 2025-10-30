using System.ComponentModel.DataAnnotations;

namespace Nezam.New.EES.Models;

public class ContractTemplate
{
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Name of the template
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Description of what this template is used for
    /// </summary>
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Path to the Word (DOCX) template file
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string FilePath { get; set; } = string.Empty;
    
    /// <summary>
    /// Contract type (e.g. Supervision, Design, Implementation)
    /// </summary>
    [MaxLength(50)]
    public string ContractType { get; set; } = string.Empty;
    
    /// <summary>
    /// Is this template currently active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Version number of this template
    /// </summary>
    public int Version { get; set; } = 1;
    
    /// <summary>
    /// Date when this template was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// ID of the user who created this template
    /// </summary>
    [MaxLength(50)]
    public string CreatedBy { get; set; } = string.Empty;
    
    /// <summary>
    /// Date when this template was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// ID of the user who last updated this template
    /// </summary>
    [MaxLength(50)]
    public string? UpdatedBy { get; set; }
    
    /// <summary>
    /// List of parameters required for mail merge in this template
    /// </summary>
    public virtual ICollection<TemplateParameter> Parameters { get; set; } = new List<TemplateParameter>();
    
    /// <summary>
    /// List of digital contracts created from this template
    /// </summary>
    public virtual ICollection<DigitalContract> Contracts { get; set; } = new List<DigitalContract>();
} 