namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class FlowEvent
{
    public int Id { get; set; }

    public int EventTypeId { get; set; }

    public int EventModeId { get; set; }

    public int EventTriggerId { get; set; }

    public virtual CancelEvent? CancelEvent { get; set; }

    public virtual ErrorCatchEvent? ErrorCatchEvent { get; set; }

    public virtual FlowEventMode EventMode { get; set; } = null!;

    public virtual FlowEventTrigger EventTrigger { get; set; } = null!;

    public virtual FlowEventType EventType { get; set; } = null!;

    public virtual FlowNode IdNavigation { get; set; } = null!;

    public virtual MessageCatchEvent? MessageCatchEvent { get; set; }

    public virtual MessageThrowEvent? MessageThrowEvent { get; set; }

    public virtual TimerEvent? TimerEvent { get; set; }
}
