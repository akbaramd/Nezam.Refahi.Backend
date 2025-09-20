namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class CourseQualifiication
{
    public int Id { get; set; }

    public Guid CourseId { get; set; }

    public int? ServiceFieldId { get; set; }

    public int? ServiceTypeId { get; set; }

    public int? EngGradeId { get; set; }

    public bool? FieldType { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual EngineeringGrade? EngGrade { get; set; }

    public virtual ServiceField? ServiceField { get; set; }

    public virtual ServiceType? ServiceType { get; set; }
}
