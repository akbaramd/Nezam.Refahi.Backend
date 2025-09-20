namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class SeparationInvolvedMember
{
    public int Id { get; set; }

    public int SeparationRequestId { get; set; }

    public int MemberServiceId { get; set; }

    public int StatusId { get; set; }

    public bool TeamLead { get; set; }

    public DateTime RegDate { get; set; }

    public virtual MemberService MemberService { get; set; } = null!;

    public virtual SeparationPlanRequest SeparationRequest { get; set; } = null!;

    public virtual ICollection<SeparationRequestSurveie> SeparationRequestSurveies { get; set; } = new List<SeparationRequestSurveie>();

    public virtual InvolvedMemberStatus Status { get; set; } = null!;
}
