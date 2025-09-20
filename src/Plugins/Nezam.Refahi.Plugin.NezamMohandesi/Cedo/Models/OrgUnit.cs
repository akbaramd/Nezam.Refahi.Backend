namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class OrgUnit
{
    public Guid Id { get; set; }

    public Guid AgencyId { get; set; }

    public Guid? ParentUnitId { get; set; }

    public int UnitTypeId { get; set; }

    public string Title { get; set; } = null!;

    public long? AutomationId { get; set; }

    public virtual CedoAgency Agency { get; set; } = null!;

    public virtual ICollection<OrgUnit> InverseParentUnit { get; set; } = new List<OrgUnit>();

    public virtual ICollection<OrgUnitPersonnel> OrgUnitPersonnel { get; set; } = new List<OrgUnitPersonnel>();

    public virtual OrgUnit? ParentUnit { get; set; }

    public virtual OrgUnitType UnitType { get; set; } = null!;
}
