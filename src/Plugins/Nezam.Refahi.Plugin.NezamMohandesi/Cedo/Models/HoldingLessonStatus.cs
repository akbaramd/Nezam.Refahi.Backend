namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class HoldingLessonStatus
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<HoldingLesson> HoldingLessons { get; set; } = new List<HoldingLesson>();
}
