using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nezam.New.EES.Models;

public class ContractSigner
{
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// ID of the contract this signer is for
    /// </summary>
    public int ContractId { get; set; }
    
    /// <summary>
    /// ID of the user who needs to sign
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// Full name of the signer
    /// </summary>
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;
    
    /// <summary>
    /// Email of the signer
    /// </summary>
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Role of the signer (e.g. Engineer, Client, Supervisor)
    /// </summary>
    [MaxLength(50)]
    public string Role { get; set; } = string.Empty;
    
    /// <summary>
    /// Order in which this person needs to sign (lower numbers sign first)
    /// </summary>
    public int SignOrder { get; set; } = 0;
    
    /// <summary>
    /// Has this person signed the contract yet
    /// </summary>
    public bool HasSigned { get; set; } = false;
    
    /// <summary>
    /// Date when the person signed
    /// </summary>
    public DateTime? SignedAt { get; set; }
    
    /// <summary>
    /// IP address from which the person signed
    /// </summary>
    [MaxLength(50)]
    public string? SignedFromIp { get; set; }
    
    /// <summary>
    /// Path to the signature image file
    /// </summary>
    [MaxLength(255)]
    public string? SignatureImagePath { get; set; }
    
    /// <summary>
    /// Certificate information (serial number, issuer) used for signing
    /// </summary>
    [MaxLength(1000)]
    public string? CertificateInfo { get; set; }
    
    /// <summary>
    /// Location on the document where to place the signature (X coordinate)
    /// </summary>
    public float? SignatureX { get; set; }
    
    /// <summary>
    /// Location on the document where to place the signature (Y coordinate)
    /// </summary>
    public float? SignatureY { get; set; }
    
    /// <summary>
    /// Page number where the signature should be placed
    /// </summary>
    public int? SignaturePage { get; set; }
    
    /// <summary>
    /// Width of the signature image when placed on the document
    /// </summary>
    public float? SignatureWidth { get; set; }
    
    /// <summary>
    /// Height of the signature image when placed on the document
    /// </summary>
    public float? SignatureHeight { get; set; }
    
    /// <summary>
    /// Navigation property to parent contract
    /// </summary>
    [ForeignKey("ContractId")]
    public virtual DigitalContract Contract { get; set; } = null!;
} 