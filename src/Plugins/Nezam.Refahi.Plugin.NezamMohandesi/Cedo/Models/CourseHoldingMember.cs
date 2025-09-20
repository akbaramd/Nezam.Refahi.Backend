namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class CourseHoldingMember
{
    public Guid Id { get; set; }

    public Guid? CourseHoldingId { get; set; }

    public int MemberId { get; set; }

    public DateTime RegDate { get; set; }

    public virtual ICollection<Attendee> Attendees { get; set; } = new List<Attendee>();

    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    public virtual CourseHolding? CourseHolding { get; set; }

    public virtual Member Member { get; set; } = null!;
}
