using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nezam.New.EES.Models;

public class TemplateParameter
{
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// ID of the template this parameter belongs to
    /// </summary>
    public int TemplateId { get; set; }
    
    /// <summary>
    /// Name of the parameter as used in the DOCX template (mail merge field)
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Display name for the parameter (user-friendly name)
    /// </summary>
    [MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty;
    
    /// <summary>
    /// Description of the parameter
    /// </summary>
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Data type of the parameter (string, number, date, etc.)
    /// </summary>
    [MaxLength(50)]
    public string DataType { get; set; } = "string";
    
    /// <summary>
    /// Is this parameter required for the template
    /// </summary>
    public bool IsRequired { get; set; } = true;
    
    /// <summary>
    /// Default value for the parameter (if any)
    /// </summary>
    [MaxLength(1000)]
    public string? DefaultValue { get; set; }
    
    /// <summary>
    /// Order of the parameter in forms
    /// </summary>
    public int DisplayOrder { get; set; } = 0;
    
    /// <summary>
    /// Navigation property to parent template
    /// </summary>
    [ForeignKey("TemplateId")]
    public virtual ContractTemplate Template { get; set; } = null!;
} 