namespace Nezam.Refahi.Recreation.Contracts.Dtos;

/// <summary>
/// Guest participant information DTO
/// </summary>
public class GuestParticipantDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string NationalNumber { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string ParticipantType { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; } = DateTime.MinValue;
    public string? EmergencyContactName { get; set; } = string.Empty;
    public string? EmergencyContactPhone { get; set; } = string.Empty;
    public string? Notes { get; set; } = string.Empty;
}