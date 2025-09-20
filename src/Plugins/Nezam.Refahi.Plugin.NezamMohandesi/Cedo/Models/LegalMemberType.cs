namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class LegalMemberType
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<BuildingGroupSettingRatio> BuildingGroupSettingRatios { get; set; } = new List<BuildingGroupSettingRatio>();

    public virtual ICollection<LegalMember> LegalMembers { get; set; } = new List<LegalMember>();

    public virtual ICollection<ValidMemberService> ValidMemberServices { get; set; } = new List<ValidMemberService>();
}
