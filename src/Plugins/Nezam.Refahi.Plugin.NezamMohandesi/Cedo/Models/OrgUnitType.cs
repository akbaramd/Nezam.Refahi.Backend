namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class OrgUnitType
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<OrgUnit> OrgUnits { get; set; } = new List<OrgUnit>();
}
