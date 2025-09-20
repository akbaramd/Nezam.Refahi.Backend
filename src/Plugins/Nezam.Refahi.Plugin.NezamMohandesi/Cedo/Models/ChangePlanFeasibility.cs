namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ChangePlanFeasibility
{
    public int Id { get; set; }

    public int ChangePlanRequestId { get; set; }

    public bool HasBuildingProgress { get; set; }

    public string? Description { get; set; }

    public bool IsConfirm { get; set; }

    public DateTime RegDate { get; set; }

    public virtual ICollection<ChangePlanFeasibilityJudgement> ChangePlanFeasibilityJudgements { get; set; } = new List<ChangePlanFeasibilityJudgement>();

    public virtual ChangePlanRequest ChangePlanRequest { get; set; } = null!;
}
