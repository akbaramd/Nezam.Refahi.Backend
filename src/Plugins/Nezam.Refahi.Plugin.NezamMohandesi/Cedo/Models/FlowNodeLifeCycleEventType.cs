namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class FlowNodeLifeCycleEventType
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<FlowNodeLifeCycleEvent> FlowNodeLifeCycleEvents { get; set; } = new List<FlowNodeLifeCycleEvent>();
}
