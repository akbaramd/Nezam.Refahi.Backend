namespace Nezam.Refahi.Identity.Contracts.Dtos;

/// <summary>
/// کاربر با مجموعه‌های مرجع (بدون فلت‌کردن)
/// </summary>
public  class UserDto
{
  // Core Identity
  public Guid Id { get; set; }
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public string? NationalId { get; set; }
  public string PhoneNumber { get; set; } = string.Empty;

  // Authentication & Status
  public bool IsPhoneVerified { get; set; }
  public DateTime? PhoneVerifiedAt { get; set; }
  public bool IsActive { get; set; }
  public DateTime? LastLoginAt { get; set; }
  public DateTime? LastAuthenticatedAt { get; set; }
  public int FailedAttempts { get; set; }
  public DateTime? LockedAt { get; set; }
  public string? LockReason { get; set; }
  public DateTime? UnlockAt { get; set; }

  // Device & Network
  public string? LastIpAddress { get; set; }
  public string? LastUserAgent { get; set; }
  public string? LastDeviceFingerprint { get; set; }

       

  // Audit (از FullAggregateRoot<Guid>)
  public DateTimeOffset CreatedAtUtc { get; set; }
  public string? CreatedBy { get; set; }
  public DateTimeOffset? UpdatedAtUtc { get; set; }
  public string? UpdatedBy { get; set; }
}