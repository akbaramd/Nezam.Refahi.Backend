namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class HoldingLesson
{
    public Guid Id { get; set; }

    public Guid CourseLessonId { get; set; }

    public int? TeacherMemberServiceId { get; set; }

    public Guid? CourseHoldingId { get; set; }

    public int Tuition { get; set; }

    public int MinCapacity { get; set; }

    public int MaxCapacity { get; set; }

    public float MaxAbsence { get; set; }

    public int StatusId { get; set; }

    public int ReTuition { get; set; }

    public DateTime? ReHoldingDate { get; set; }

    public DateTime ExamDate { get; set; }

    public DateTime? HoldingEnd { get; set; }

    public DateTime? HoldingStart { get; set; }

    public virtual ICollection<Attendee> Attendees { get; set; } = new List<Attendee>();

    public virtual CourseHolding? CourseHolding { get; set; }

    public virtual CourseLesson CourseLesson { get; set; } = null!;

    public virtual ICollection<HoldingLessonOption> HoldingLessonOptions { get; set; } = new List<HoldingLessonOption>();

    public virtual ICollection<HoldingLessonSession> HoldingLessonSessions { get; set; } = new List<HoldingLessonSession>();

    public virtual HoldingLessonStatus Status { get; set; } = null!;

    public virtual MemberService? TeacherMemberService { get; set; }
}
