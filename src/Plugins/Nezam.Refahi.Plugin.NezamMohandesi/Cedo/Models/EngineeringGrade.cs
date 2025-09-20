namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class EngineeringGrade
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<CourseQualifiication> CourseQualifiications { get; set; } = new List<CourseQualifiication>();

    public virtual ICollection<ExecuterCompetenceSetting> ExecuterCompetenceSettings { get; set; } = new List<ExecuterCompetenceSetting>();

    public virtual ICollection<GradeCalculationSetting> GradeCalculationSettings { get; set; } = new List<GradeCalculationSetting>();

    public virtual ICollection<MemberService> MemberServices { get; set; } = new List<MemberService>();

    public virtual ICollection<SupervisionField> SupervisionFields { get; set; } = new List<SupervisionField>();
}
