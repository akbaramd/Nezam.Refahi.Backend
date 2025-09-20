namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class OrgPost
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<OrgUnitPersonnel> OrgUnitPersonnel { get; set; } = new List<OrgUnitPersonnel>();
}
