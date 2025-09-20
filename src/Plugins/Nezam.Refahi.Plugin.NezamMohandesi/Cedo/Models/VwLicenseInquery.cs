namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class VwLicenseInquery
{
    public int CityId { get; set; }

    public string Title { get; set; } = null!;

    public int Id { get; set; }

    public bool IsShahrdariService { get; set; }

    public string? RefNumber { get; set; }

    public DateTime? RegDate { get; set; }

    public string Code { get; set; } = null!;

    public string LicenseNumber { get; set; } = null!;

    public string NationalCode { get; set; } = null!;

    public string? OwnerFullName { get; set; }
}
