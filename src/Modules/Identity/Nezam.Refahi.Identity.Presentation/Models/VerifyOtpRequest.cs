using System.Text.Json.Serialization;

namespace Nezam.Refahi.Identity.Presentation.Models;

public record VerifyOtpRequest(
  string ChallengeId,
  string OtpCode,
  [property: JsonIgnore]
  string? Purpose  = null,
  [property: JsonIgnore]
  string? DeviceId = null,
  string? Scope = null); // IpAddress از HttpContext استخراج می‌شود
  
public record RefreshTokenRequest(
  string RefreshToken,  [property: JsonIgnore]
  string? DeviceId = null); // IpAddress 