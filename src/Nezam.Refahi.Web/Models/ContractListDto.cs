namespace Nezam.New.EES.Models;

/// <summary>
/// DTO for contract list display
/// </summary>
public class ContractListDto
{
    /// <summary>
    /// ID of the contract
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Contract number (reference code)
    /// </summary>
    public string ContractNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// Title of the contract
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Current status of the contract
    /// 0 = Draft, 1 = Pending, 2 = In Progress, 3 = Completed, 4 = Cancelled
    /// </summary>
    public int Status { get; set; }
    
    /// <summary>
    /// Date when the contract was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Name of the template used for the contract
    /// </summary>
    public string TemplateName { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether the current user has signed the contract
    /// </summary>
    public bool HasSigned { get; set; }
    
    /// <summary>
    /// Whether the user needs to sign the contract
    /// </summary>
    public bool SignatureRequired { get; set; }
    
    /// <summary>
    /// Status display text
    /// </summary>
    public string StatusText
    {
        get
        {
            return Status switch
            {
                0 => "Draft",
                1 => "Pending",
                2 => "In Progress",
                3 => "Completed",
                4 => "Cancelled",
                _ => "Unknown"
            };
        }
    }
} 