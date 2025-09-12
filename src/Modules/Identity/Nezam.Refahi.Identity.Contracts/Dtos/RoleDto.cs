namespace Nezam.Refahi.Identity.Contracts.Dtos;

/// <summary>
/// نقش (بدون الحاق مجموعهٔ کاربران برای جلوگیری از چرخهٔ ارجاع)
/// </summary>
public  class RoleDto
{
  public Guid Id { get; set; }

  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }
  public bool IsActive { get; set; }
  public bool IsSystemRole { get; set; }
  public int DisplayOrder { get; set; }

  public List<RoleClaimDto> Claims { get; set; } = new();

  // FullyAuditableAggregateRoot<Guid>
  public DateTime CreatedAt { get; set; }
  public string? CreatedBy { get; set; }
  public DateTime? UpdatedAt { get; set; }
  public string? UpdatedBy { get; set; }

  // اختیاری: دادهٔ مشتق‌شده که ممکن است سمت سرور محاسبه شود
  public int? ActiveUserCount { get; set; }
}