namespace Nezam.Refahi.Identity.Contracts.Dtos;

/// <summary>
/// توکن‌های کاربر (حداقل دادهٔ ایمن؛ مقدار خام توکن را در DTO قرار ندهید مگر سناریوی صدور)
/// </summary>
public class UserTokenDto
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }

  public string TokenType { get; set; } = string.Empty; // e.g. "RefreshToken", "AccessToken"
  public DateTime ExpiresAt { get; set; }
  public bool IsRevoked { get; set; }
  public bool IsUsed { get; set; }

  // اختیاری: متادیتا
  public DateTime CreatedAt { get; set; }
  public string? CreatedBy { get; set; }
}