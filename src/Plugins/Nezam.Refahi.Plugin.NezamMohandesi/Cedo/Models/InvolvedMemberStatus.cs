namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class InvolvedMemberStatus
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<GasRequestInvolvedMember> GasRequestInvolvedMembers { get; set; } = new List<GasRequestInvolvedMember>();

    public virtual ICollection<InvolvedMember> InvolvedMembers { get; set; } = new List<InvolvedMember>();

    public virtual ICollection<SeparationInvolvedMember> SeparationInvolvedMembers { get; set; } = new List<SeparationInvolvedMember>();
}
