namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ConstructionLicenseGeoLab
{
    public int Id { get; set; }

    public int ConstructionLicenseId { get; set; }

    public int DeploymentLevel { get; set; }

    public int BoreCount { get; set; }

    public float BoreDepth { get; set; }

    public int DrillingTypeId { get; set; }

    public bool DownHoleTest { get; set; }

    public virtual ConstructionLicense ConstructionLicense { get; set; } = null!;

    public virtual DrillingType DrillingType { get; set; } = null!;
}
