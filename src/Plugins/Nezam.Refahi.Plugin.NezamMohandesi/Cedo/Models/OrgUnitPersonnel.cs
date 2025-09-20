namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class OrgUnitPersonnel
{
    public Guid Id { get; set; }

    public Guid UnitId { get; set; }

    public Guid PersonnelId { get; set; }

    public int? PostId { get; set; }

    public DateTime AssignedAt { get; set; }

    public DateTime? UnassignedAt { get; set; }

    public virtual OrgPersonnel Personnel { get; set; } = null!;

    public virtual OrgPost? Post { get; set; }

    public virtual OrgUnit Unit { get; set; } = null!;
}
