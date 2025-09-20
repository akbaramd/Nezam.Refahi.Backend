namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class GasRequestInvolvedMember
{
    public int Id { get; set; }

    public int? RequestId { get; set; }

    public int MemberServiceId { get; set; }

    public int StatusId { get; set; }

    public DateTime RegDate { get; set; }

    public int? OncallId { get; set; }

    public virtual GasRequestInvolvedMemberChangeLog? GasRequestInvolvedMemberChangeLog { get; set; }

    public virtual ICollection<GasRotatingForm> GasRotatingForms { get; set; } = new List<GasRotatingForm>();

    public virtual ICollection<GasSupervisionResult> GasSupervisionResults { get; set; } = new List<GasSupervisionResult>();

    public virtual MemberService MemberService { get; set; } = null!;

    public virtual GasRequestOncall? Oncall { get; set; }

    public virtual GasRequest? Request { get; set; }

    public virtual InvolvedMemberStatus Status { get; set; } = null!;
}
