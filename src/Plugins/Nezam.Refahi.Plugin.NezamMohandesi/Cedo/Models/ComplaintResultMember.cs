namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ComplaintResultMember
{
    public int Id { get; set; }

    public int ComplaintResultId { get; set; }

    public int ComplaintMemberId { get; set; }

    public DateTime RegDate { get; set; }

    public DateTime? VisitDate { get; set; }

    public string? Description { get; set; }

    public virtual ComplaintMember ComplaintMember { get; set; } = null!;

    public virtual ComplaintCommunicated ComplaintResult { get; set; } = null!;
}
