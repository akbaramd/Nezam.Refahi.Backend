namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class MemberShipType
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<ExecuterCompetenceSetting> ExecuterCompetenceSettings { get; set; } = new List<ExecuterCompetenceSetting>();

    public virtual ICollection<Member> Members { get; set; } = new List<Member>();

    public virtual ICollection<ValidMemberService> ValidMemberServices { get; set; } = new List<ValidMemberService>();
}
