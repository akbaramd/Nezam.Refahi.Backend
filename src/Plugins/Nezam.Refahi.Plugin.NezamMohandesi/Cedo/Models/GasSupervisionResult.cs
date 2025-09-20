namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class GasSupervisionResult
{
    public int Id { get; set; }

    public int InvolvedMemberId { get; set; }

    public int StateId { get; set; }

    public string Description { get; set; } = null!;

    public DateTime RegDate { get; set; }

    public virtual GasRequestInvolvedMember InvolvedMember { get; set; } = null!;

    public virtual GasSupervisionResultState State { get; set; } = null!;
}
