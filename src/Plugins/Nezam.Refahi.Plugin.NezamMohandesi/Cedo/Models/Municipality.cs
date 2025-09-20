namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class Municipality
{
    public int Id { get; set; }

    public int CityId { get; set; }

    public string Title { get; set; } = null!;

    public int? RegionCode { get; set; }

    public string? RegionTitle { get; set; }

    public virtual ICollection<BuildingGroupSetting> BuildingGroupSettings { get; set; } = new List<BuildingGroupSetting>();

    public virtual City City { get; set; } = null!;

    public virtual ICollection<Estate> Estates { get; set; } = new List<Estate>();

    public virtual ICollection<LicenseInquery> LicenseInqueries { get; set; } = new List<LicenseInquery>();

    public virtual ICollection<SeparationPlanRequest> SeparationPlanRequests { get; set; } = new List<SeparationPlanRequest>();
}
