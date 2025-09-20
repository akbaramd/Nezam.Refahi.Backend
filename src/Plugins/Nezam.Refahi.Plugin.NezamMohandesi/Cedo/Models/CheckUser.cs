namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class CheckUser
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Father { get; set; }

    public string? NationalCode { get; set; }

    public string MembershipCode { get; set; } = null!;

    public bool IsActive { get; set; }

    public string? PhoneNumber { get; set; }
}
