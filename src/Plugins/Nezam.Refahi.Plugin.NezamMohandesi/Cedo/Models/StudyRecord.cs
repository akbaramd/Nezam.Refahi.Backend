namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class StudyRecord
{
    public int Id { get; set; }

    public int MemberId { get; set; }

    public int UniversityId { get; set; }

    public int StudyGradeId { get; set; }

    public DateTime Date { get; set; }

    public bool IsMainField { get; set; }

    public virtual Member Member { get; set; } = null!;

    public virtual StudyGrade StudyGrade { get; set; } = null!;

    public virtual University University { get; set; } = null!;
}
