namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class WorkItemSetting
{
    public int Id { get; set; }

    public int BuildingGroupId { get; set; }

    public int Max { get; set; }

    public double WorkItem { get; set; }

    public int? CityId { get; set; }

    public int? DossierTypeId { get; set; }

    public virtual BuildingGroup BuildingGroup { get; set; } = null!;

    public virtual City? City { get; set; }

    public virtual DossierType? DossierType { get; set; }
}
