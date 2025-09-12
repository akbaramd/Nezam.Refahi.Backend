namespace Nezam.Refahi.Identity.Contracts.Dtos;

/// <summary>
/// Claim اختصاصی کاربر (نه از نقش)
/// </summary>
public  class UserClaimDto
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public ClaimDto Claim { get; set; } = new();
  public bool IsActive { get; set; }
  public DateTime AssignedAt { get; set; }
  public DateTime? ExpiresAt { get; set; }
  public string? AssignedBy { get; set; }
  public string? Notes { get; set; }
}