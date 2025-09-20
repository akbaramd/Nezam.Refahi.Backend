namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class CourseLesson
{
    public Guid Id { get; set; }

    public Guid CourseId { get; set; }

    public Guid LessonId { get; set; }

    public bool Required { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<HoldingLesson> HoldingLessons { get; set; } = new List<HoldingLesson>();

    public virtual Lesson Lesson { get; set; } = null!;
}
