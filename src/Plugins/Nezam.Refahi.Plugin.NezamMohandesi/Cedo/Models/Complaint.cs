namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class Complaint
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime RegDate { get; set; }

    public int ComplaintStatusId { get; set; }

    public string? IntialResult { get; set; }

    public string ComplainerAddress { get; set; } = null!;

    public string ComplainerZipCode { get; set; } = null!;

    public DateTime ViolationDate { get; set; }

    public string ViolationPlace { get; set; } = null!;

    public virtual ICollection<Complainee> Complainees { get; set; } = new List<Complainee>();

    public virtual ICollection<ComplaintCommunicated> ComplaintCommunicateds { get; set; } = new List<ComplaintCommunicated>();

    public virtual ICollection<ComplaintMember> ComplaintMembers { get; set; } = new List<ComplaintMember>();

    public virtual ICollection<ComplaintReview> ComplaintReviews { get; set; } = new List<ComplaintReview>();

    public virtual ComplaintStatus ComplaintStatus { get; set; } = null!;
}
