namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class BuildingSubGroup
{
    public int Id { get; set; }

    public int BuildingGroupId { get; set; }

    public string Title { get; set; } = null!;

    public int SyncCode { get; set; }

    public virtual BuildingGroup BuildingGroup { get; set; } = null!;

    public virtual ICollection<BuildingGroupSetting> BuildingGroupSettings { get; set; } = new List<BuildingGroupSetting>();

    public virtual ICollection<CostTypeDetailSetting> CostTypeDetailSettings { get; set; } = new List<CostTypeDetailSetting>();

    public virtual ICollection<EngServiceTariffDetail> EngServiceTariffDetails { get; set; } = new List<EngServiceTariffDetail>();

    public virtual ICollection<MemberCapacityDetail> MemberCapacityDetails { get; set; } = new List<MemberCapacityDetail>();

    public virtual ICollection<PlanControlSetting> PlanControlSettings { get; set; } = new List<PlanControlSetting>();

    public virtual ICollection<SubWorkItemSetting> SubWorkItemSettings { get; set; } = new List<SubWorkItemSetting>();
}
