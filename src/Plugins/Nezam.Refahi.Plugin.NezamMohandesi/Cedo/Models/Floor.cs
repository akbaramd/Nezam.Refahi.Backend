namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class Floor
{
    public int Id { get; set; }

    public int ConstructionLicenseId { get; set; }

    public int FloorTypeId { get; set; }

    public int FloorRegisterStepId { get; set; }

    public virtual ICollection<BuildingUnit> BuildingUnits { get; set; } = new List<BuildingUnit>();

    public virtual ConstructionLicense ConstructionLicense { get; set; } = null!;

    public virtual FloorRegisterStep FloorRegisterStep { get; set; } = null!;

    public virtual FloorType FloorType { get; set; } = null!;
}
