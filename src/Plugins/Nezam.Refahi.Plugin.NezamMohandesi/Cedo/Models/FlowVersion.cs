namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class FlowVersion
{
    public int Id { get; set; }

    public int FlowId { get; set; }

    public int Version { get; set; }

    public virtual Flow Flow { get; set; } = null!;

    public virtual ICollection<FlowActor> FlowActors { get; set; } = new List<FlowActor>();

    public virtual ICollection<FlowInstance> FlowInstances { get; set; } = new List<FlowInstance>();

    public virtual ICollection<FlowNode> FlowNodes { get; set; } = new List<FlowNode>();
}
