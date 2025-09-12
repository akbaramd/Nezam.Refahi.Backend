namespace Nezam.Refahi.Identity.Contracts.Dtos;

/// <summary>
/// انتساب نقش به کاربر (سبک و بدون RoleDto داخلی برای پیشگیری از گراف‌های سنگین)
/// </summary>
public  class UserRoleDto
{
  public Guid Id { get; set; }                 // شناسهٔ UserRole
  public Guid UserId { get; set; }
  public Guid RoleId { get; set; }
  public string RoleName { get; set; } = string.Empty;

  public bool IsActive { get; set; }
  public DateTime AssignedAt { get; set; }
  public DateTime? ExpiresAt { get; set; }
  public string? AssignedBy { get; set; }
  public string? Notes { get; set; }
}