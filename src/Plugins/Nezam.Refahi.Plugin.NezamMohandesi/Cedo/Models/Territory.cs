namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class Territory
{
    public int Id { get; set; }

    public int ConstructionLicenseId { get; set; }

    public int TerritoryTypeId { get; set; }

    public int PasswayTypeId { get; set; }

    public float DocumentDimension { get; set; }

    public float AvailableDimension { get; set; }

    public float AvailableWidth { get; set; }

    public float CorrectiveWidth { get; set; }

    public float AfterCorrectionDimension { get; set; }

    public virtual ConstructionLicense ConstructionLicense { get; set; } = null!;

    public virtual PasswayType PasswayType { get; set; } = null!;

    public virtual TerritoryType TerritoryType { get; set; } = null!;
}
