namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class Lesson
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string Code { get; set; } = null!;

    public int Duration { get; set; }

    public int Validity { get; set; }

    public virtual ICollection<CourseLesson> CourseLessons { get; set; } = new List<CourseLesson>();

    public virtual ICollection<TeacherLesson> TeacherLessons { get; set; } = new List<TeacherLesson>();
}
