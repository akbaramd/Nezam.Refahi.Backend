namespace Nezam.Refahi.Recreation.Contracts.Services;

/// <summary>
/// Request DTO for participant validation
/// Contains only primitive types to avoid dependency on other module DTOs
/// </summary>
public class ParticipantValidationRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string NationalNumber { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string ParticipantType { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; } = DateTime.MinValue;
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? Notes { get; set; }
}

