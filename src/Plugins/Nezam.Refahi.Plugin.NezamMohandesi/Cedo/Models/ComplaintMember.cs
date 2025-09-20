namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ComplaintMember
{
    public int Id { get; set; }

    public int ComplaintId { get; set; }

    public Guid UserId { get; set; }

    public int ComplaintMemberTypeId { get; set; }

    public DateTime RegDate { get; set; }

    public string? Description { get; set; }

    public virtual Complaint Complaint { get; set; } = null!;

    public virtual ComplaintMemberType ComplaintMemberType { get; set; } = null!;

    public virtual ICollection<ComplaintResultMember> ComplaintResultMembers { get; set; } = new List<ComplaintResultMember>();

    public virtual ICollection<ComplaintReview> ComplaintReviews { get; set; } = new List<ComplaintReview>();

    public virtual ParaUser User { get; set; } = null!;
}
