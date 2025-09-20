namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ConstructionLicenseStatus
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<ConstructionLicense> ConstructionLicenses { get; set; } = new List<ConstructionLicense>();
}
