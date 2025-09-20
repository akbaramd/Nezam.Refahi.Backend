namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class Course
{
    public Guid Id { get; set; }

    public int CourseTypeId { get; set; }

    public string Title { get; set; } = null!;

    public int MinReq { get; set; }

    public bool IsActive { get; set; }

    public bool HasExam { get; set; }

    public virtual ICollection<CourseHolding> CourseHoldings { get; set; } = new List<CourseHolding>();

    public virtual ICollection<CourseLesson> CourseLessons { get; set; } = new List<CourseLesson>();

    public virtual ICollection<CourseQualifiication> CourseQualifiications { get; set; } = new List<CourseQualifiication>();

    public virtual CourseType CourseType { get; set; } = null!;
}
