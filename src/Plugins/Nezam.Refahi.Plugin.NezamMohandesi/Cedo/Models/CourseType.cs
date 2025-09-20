namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class CourseType
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
