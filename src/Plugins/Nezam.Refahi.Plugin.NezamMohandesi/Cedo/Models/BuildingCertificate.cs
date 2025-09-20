namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class BuildingCertificate
{
    public int Id { get; set; }

    public int ConstructionLicenseId { get; set; }

    public int ConstructionLicenseTypeId { get; set; }

    public string BuildingCertificateNo { get; set; } = null!;

    public string? DossierNo { get; set; }

    public DateTime IssuedDate { get; set; }

    public int ValidityDuration { get; set; }

    public string? Description { get; set; }

    public double? ViolationPrivacy { get; set; }

    public virtual ConstructionLicense ConstructionLicense { get; set; } = null!;

    public virtual ConstructionLicenseType ConstructionLicenseType { get; set; } = null!;
}
