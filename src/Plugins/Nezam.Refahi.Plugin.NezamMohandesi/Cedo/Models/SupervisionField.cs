namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class SupervisionField
{
    public Guid Id { get; set; }

    public int FieldId { get; set; }

    public Guid SupervisionOrderId { get; set; }

    public int MinGradeId { get; set; }

    public virtual ICollection<AdditionalSupervisionField> AdditionalSupervisionFields { get; set; } = new List<AdditionalSupervisionField>();

    public virtual ServiceField Field { get; set; } = null!;

    public virtual EngineeringGrade MinGrade { get; set; } = null!;

    public virtual ICollection<SupervisionCandidate> SupervisionCandidates { get; set; } = new List<SupervisionCandidate>();

    public virtual SupervisionOrder SupervisionOrder { get; set; } = null!;
}
