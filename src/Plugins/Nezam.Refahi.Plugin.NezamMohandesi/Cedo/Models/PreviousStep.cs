namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class PreviousStep
{
    public int Id { get; set; }

    public int StepId { get; set; }

    public int PrevStepId { get; set; }

    public virtual FlowStep PrevStep { get; set; } = null!;

    public virtual FlowStep Step { get; set; } = null!;
}
