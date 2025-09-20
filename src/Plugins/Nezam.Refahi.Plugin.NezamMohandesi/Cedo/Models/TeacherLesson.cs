namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class TeacherLesson
{
    public Guid Id { get; set; }

    public Guid LessonId { get; set; }

    public int MemberServiceId { get; set; }

    public bool IsActive { get; set; }

    public virtual Lesson Lesson { get; set; } = null!;

    public virtual MemberService MemberService { get; set; } = null!;
}
