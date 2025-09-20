namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class Attendee
{
    public Guid Id { get; set; }

    public Guid HoldingLessonId { get; set; }

    public int StatusId { get; set; }

    public DateTime Date { get; set; }

    public float? Score { get; set; }

    public DateTime? ScoreDate { get; set; }

    public Guid CourseHoldingMemberId { get; set; }

    public float? ReScore { get; set; }

    public DateTime? ReScoreDate { get; set; }

    public string? Test { get; set; }

    public virtual ICollection<AttendeeOption> AttendeeOptions { get; set; } = new List<AttendeeOption>();

    public virtual ICollection<AttendeeSession> AttendeeSessions { get; set; } = new List<AttendeeSession>();

    public virtual CourseHoldingMember CourseHoldingMember { get; set; } = null!;

    public virtual HoldingLesson HoldingLesson { get; set; } = null!;

    public virtual AttendeeStatus Status { get; set; } = null!;
}
