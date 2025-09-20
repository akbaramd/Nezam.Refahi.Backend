namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class SupervisionStepForm
{
    public int Id { get; set; }

    public int InvolvedMemberId { get; set; }

    public int StepId { get; set; }

    public string? ContradictionDescription { get; set; }

    public DateTime RegDate { get; set; }

    public int? FloorNumber { get; set; }

    public bool IsCopy { get; set; }

    public virtual InvolvedMember InvolvedMember { get; set; } = null!;

    public virtual SupervisionStep Step { get; set; } = null!;

    public virtual ICollection<SupervisionItemAnswer> SupervisionItemAnswers { get; set; } = new List<SupervisionItemAnswer>();
}
