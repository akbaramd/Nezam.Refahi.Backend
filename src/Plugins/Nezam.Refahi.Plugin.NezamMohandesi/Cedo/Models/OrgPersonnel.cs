namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class OrgPersonnel
{
    public Guid Id { get; set; }

    public virtual ParaUser IdNavigation { get; set; } = null!;

    public virtual ICollection<OrgUnitPersonnel> OrgUnitPersonnel { get; set; } = new List<OrgUnitPersonnel>();
}
