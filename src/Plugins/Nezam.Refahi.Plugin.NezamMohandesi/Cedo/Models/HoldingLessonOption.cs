namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class HoldingLessonOption
{
    public int Id { get; set; }

    public Guid HoldingLessonId { get; set; }

    public string Title { get; set; } = null!;

    public int Cost { get; set; }

    public virtual ICollection<AttendeeOption> AttendeeOptions { get; set; } = new List<AttendeeOption>();

    public virtual HoldingLesson HoldingLesson { get; set; } = null!;
}
