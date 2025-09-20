namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class BuildingGroupSettingRatio
{
    public int Id { get; set; }

    public int BuildingGroupSettingId { get; set; }

    public int? MemberTypeId { get; set; }

    public int? ServiceFieldId { get; set; }

    public double MetrajRatio { get; set; }

    public double WorkItemRatio { get; set; }

    public int? ServiceTypeId { get; set; }

    public virtual BuildingGroupSetting BuildingGroupSetting { get; set; } = null!;

    public virtual LegalMemberType? MemberType { get; set; }

    public virtual ServiceField? ServiceField { get; set; }

    public virtual ServiceType? ServiceType { get; set; }
}
