using System.Text.Json.Serialization;

namespace Nezam.Refahi.Membership.Contracts.Dtos;

public class ExternalUserDto
{
  [JsonPropertyName("nationalNumber")]
  public string NationalNumber { get; set; } = string.Empty;

  [JsonPropertyName("firstName")]
  public string FirstName { get; set; } = string.Empty;

  [JsonPropertyName("lastName")]
  public string LastName { get; set; } = string.Empty;

  [JsonPropertyName("dateCreated")]
  public DateTime DateCreated { get; set; }

  [JsonPropertyName("dateModified")]
  public DateTime DateModified { get; set; }

  [JsonPropertyName("lastLoginDate")]
  public DateTime? LastLoginDate { get; set; }

  [JsonPropertyName("isActive")]
  public bool IsActive { get; set; }

  [JsonPropertyName("profilePictureUrl")]
  public string? ProfilePictureUrl { get; set; }

  [JsonPropertyName("department")]
  public string? Department { get; set; }

  [JsonPropertyName("jobTitle")]
  public string? JobTitle { get; set; }

  [JsonPropertyName("certificatePassword")]
  public string? CertificatePassword { get; set; }

  [JsonPropertyName("baleUserId")]
  public string? BaleUserId { get; set; }

  [JsonPropertyName("baleChatId")]
  public string? BaleChatId { get; set; }

  [JsonPropertyName("baleLastInteraction")]
  public DateTime? BaleLastInteraction { get; set; }

  [JsonPropertyName("loginType")]
  public ExternalLoginTypeDto LoginType { get; set; } = new();

  [JsonPropertyName("status")]
  public ExternalStatusDto Status { get; set; } = new();

  [JsonPropertyName("userAgencies")]
  public List<object> UserAgencies { get; set; } = new();

  [JsonPropertyName("fullName")]
  public string FullName { get; set; } = string.Empty;

  [JsonPropertyName("id")]
  public string Id { get; set; } = string.Empty;

  [JsonPropertyName("userName")]
  public string UserName { get; set; } = string.Empty;

  [JsonPropertyName("normalizedUserName")]
  public string NormalizedUserName { get; set; } = string.Empty;

  [JsonPropertyName("email")]
  public string Email { get; set; } = string.Empty;

  [JsonPropertyName("normalizedEmail")]
  public string NormalizedEmail { get; set; } = string.Empty;

  [JsonPropertyName("emailConfirmed")]
  public bool EmailConfirmed { get; set; }

  [JsonPropertyName("passwordHash")]
  public string PasswordHash { get; set; } = string.Empty;

  [JsonPropertyName("securityStamp")]
  public string SecurityStamp { get; set; } = string.Empty;

  [JsonPropertyName("concurrencyStamp")]
  public string ConcurrencyStamp { get; set; } = string.Empty;

  [JsonPropertyName("phoneNumber")]
  public string PhoneNumber { get; set; } = string.Empty;

  [JsonPropertyName("phoneNumberConfirmed")]
  public bool PhoneNumberConfirmed { get; set; }

  [JsonPropertyName("twoFactorEnabled")]
  public bool TwoFactorEnabled { get; set; }

  [JsonPropertyName("lockoutEnd")]
  public DateTime? LockoutEnd { get; set; }

  [JsonPropertyName("lockoutEnabled")]
  public bool LockoutEnabled { get; set; }

  [JsonPropertyName("accessFailedCount")]
  public int AccessFailedCount { get; set; }
}