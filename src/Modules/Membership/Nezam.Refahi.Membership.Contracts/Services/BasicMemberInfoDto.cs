namespace Nezam.Refahi.Membership.Contracts.Services;

/// <summary>
/// Basic member information DTO with limited fields for cross-context access
/// </summary>
public class BasicMemberInfoDto
{
  public Guid Id { get; set; }
  public string FullName { get; set; } = string.Empty;
  public string NationalCode { get; set; } = string.Empty;
  public string? MembershipNumber { get; set; }
  public bool IsActive { get; set; }
  public bool HasActiveMembership { get; set; }
}