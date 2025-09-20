namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class FlowStep
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? VisitedAt { get; set; }

    public DateTime? DoneAt { get; set; }

    public string? Comment { get; set; }

    public int FlowInstanceId { get; set; }

    public int FlowNodeId { get; set; }

    public int StatusId { get; set; }

    public Guid? OwnerId { get; set; }

    public string? Title { get; set; }

    public string? SubTitle { get; set; }

    public virtual FlowInstance FlowInstance { get; set; } = null!;

    public virtual FlowNode FlowNode { get; set; } = null!;

    public virtual ParaUser? Owner { get; set; }

    public virtual ICollection<PreviousStep> PreviousStepPrevSteps { get; set; } = new List<PreviousStep>();

    public virtual ICollection<PreviousStep> PreviousStepSteps { get; set; } = new List<PreviousStep>();

    public virtual FlowStepStatus Status { get; set; } = null!;

    public virtual ICollection<StepOwnerPool> StepOwnerPools { get; set; } = new List<StepOwnerPool>();

    public virtual StepRecordLog? StepRecordLog { get; set; }

    public virtual ICollection<TakenUserAction> TakenUserActions { get; set; } = new List<TakenUserAction>();

    public virtual TimerTick? TimerTick { get; set; }
}
