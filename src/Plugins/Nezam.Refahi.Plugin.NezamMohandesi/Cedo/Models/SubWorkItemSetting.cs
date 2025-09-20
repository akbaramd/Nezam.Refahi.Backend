namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class SubWorkItemSetting
{
    public int Id { get; set; }

    public int SubGroupId { get; set; }

    public int Max { get; set; }

    public double WorkItem { get; set; }

    public int? CityId { get; set; }

    public int? DossierTypeId { get; set; }

    public int? ServiceTypeId { get; set; }

    public virtual City? City { get; set; }

    public virtual DossierType? DossierType { get; set; }

    public virtual ServiceType? ServiceType { get; set; }

    public virtual BuildingSubGroup SubGroup { get; set; } = null!;
}
