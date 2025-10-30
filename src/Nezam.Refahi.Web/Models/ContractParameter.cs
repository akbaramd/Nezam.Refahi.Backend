using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nezam.New.EES.Models;

public class ContractParameter
{
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// ID of the contract this parameter belongs to
    /// </summary>
    public int ContractId { get; set; }
    
    /// <summary>
    /// Name of the parameter as used in the DOCX template (mail merge field)
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Value of the parameter
    /// </summary>
    [MaxLength(4000)]
    public string Value { get; set; } = string.Empty;
    
    /// <summary>
    /// ID of the template parameter this value is for (if any)
    /// </summary>
    public int? TemplateParameterId { get; set; }
    
    /// <summary>
    /// Navigation property to parent contract
    /// </summary>
    [ForeignKey("ContractId")]
    public virtual DigitalContract Contract { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to template parameter
    /// </summary>
    [ForeignKey("TemplateParameterId")]
    public virtual TemplateParameter? TemplateParameter { get; set; }
} 