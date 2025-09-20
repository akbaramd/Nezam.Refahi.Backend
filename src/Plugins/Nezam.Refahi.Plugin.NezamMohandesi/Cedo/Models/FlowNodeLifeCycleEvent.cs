namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class FlowNodeLifeCycleEvent
{
    public int Id { get; set; }

    public int NodeId { get; set; }

    public int EventTypeId { get; set; }

    public string? Script { get; set; }

    public virtual FlowNodeLifeCycleEventType EventType { get; set; } = null!;

    public virtual FlowNode Node { get; set; } = null!;
}
