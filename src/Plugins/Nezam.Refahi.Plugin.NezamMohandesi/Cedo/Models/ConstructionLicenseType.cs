namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ConstructionLicenseType
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<BuildingCertificate> BuildingCertificates { get; set; } = new List<BuildingCertificate>();

    public virtual ICollection<ConstructionLicense> ConstructionLicenses { get; set; } = new List<ConstructionLicense>();
}
