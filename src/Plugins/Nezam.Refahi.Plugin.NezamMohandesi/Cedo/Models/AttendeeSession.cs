namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class AttendeeSession
{
    public Guid Id { get; set; }

    public Guid SessionId { get; set; }

    public Guid AttendeeId { get; set; }

    public int StatusId { get; set; }

    public DateTime RegDate { get; set; }

    public virtual Attendee Attendee { get; set; } = null!;

    public virtual HoldingLessonSession Session { get; set; } = null!;

    public virtual AttendeeSessionStatus Status { get; set; } = null!;
}
