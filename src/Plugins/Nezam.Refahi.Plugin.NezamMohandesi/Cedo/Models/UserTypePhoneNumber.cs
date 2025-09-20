namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class UserTypePhoneNumber
{
    public string Title { get; set; } = null!;

    public string ServiceField { get; set; } = null!;

    public int ServiceTypeId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public bool IsActive { get; set; }

    public string? PhoneNumber { get; set; }
}
