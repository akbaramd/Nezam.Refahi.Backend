namespace Nezam.Refahi.Identity.Domain.Dtos;

/// <summary>
/// Claimهای نقش (Wrapper روی Claim با شناسهٔ RoleClaim)
/// </summary>
public  class RoleClaimDto
{
  public Guid Id { get; set; }
  public Guid RoleId { get; set; }
  public ClaimDto Claim { get; set; } = new();
  
  // Audit properties
  public DateTime CreatedAt { get; set; }
  public string? CreatedBy { get; set; }
}