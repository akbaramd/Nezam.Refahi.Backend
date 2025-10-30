using System.ComponentModel.DataAnnotations;

namespace Nezam.New.EES.Models;

public class UserCertificate
{
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// ID of the user who owns this certificate
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// Friendly name for the certificate
    /// </summary>
    [MaxLength(100)]
    public string FriendlyName { get; set; } = string.Empty;
    
    /// <summary>
    /// Serial number of the certificate
    /// </summary>
    [MaxLength(100)]
    public string SerialNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// Certificate issuer
    /// </summary>
    [MaxLength(255)]
    public string Issuer { get; set; } = string.Empty;
    
    /// <summary>
    /// Certificate subject
    /// </summary>
    [MaxLength(255)]
    public string Subject { get; set; } = string.Empty;
    
    /// <summary>
    /// Date when the certificate becomes valid
    /// </summary>
    public DateTime NotBefore { get; set; }
    
    /// <summary>
    /// Date when the certificate expires
    /// </summary>
    public DateTime NotAfter { get; set; }
    
    /// <summary>
    /// Thumbprint of the certificate
    /// </summary>
    [MaxLength(64)]
    public string Thumbprint { get; set; } = string.Empty;
    
    /// <summary>
    /// Path to the stored certificate file
    /// </summary>
    [MaxLength(255)]
    public string? FilePath { get; set; }
    
    /// <summary>
    /// Is this certificate active and usable
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Date when this certificate was added
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Date when this certificate was last used
    /// </summary>
    public DateTime? LastUsedAt { get; set; }
} 