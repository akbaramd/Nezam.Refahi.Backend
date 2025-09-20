namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class MemberCapacityDetail
{
    public int Id { get; set; }

    public int MemberServiceId { get; set; }

    public float WorkItem { get; set; }

    public float Metraj { get; set; }

    public string? Description { get; set; }

    public bool Released { get; set; }

    public DateTime RegDate { get; set; }

    public int BuildingSubGroupId { get; set; }

    public int? InvolvedMemberId { get; set; }

    public int? QuotaYear { get; set; }

    public int Type { get; set; }

    public virtual BuildingSubGroup BuildingSubGroup { get; set; } = null!;

    public virtual MemberService MemberService { get; set; } = null!;
}
