namespace Nezam.Refahi.Identity.Presentation.Models;

public record SendOtpRequest(string NationalCode, string? Purpose = null, string? DeviceId = null, string? Scope = null);