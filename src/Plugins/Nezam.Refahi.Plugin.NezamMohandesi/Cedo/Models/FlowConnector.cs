namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class FlowConnector
{
    public int Id { get; set; }

    public Guid UniqueId { get; set; }

    public string? Title { get; set; }

    public int Order { get; set; }

    public int SourceNodeId { get; set; }

    public int ConnectionTypeId { get; set; }

    public int TargetNodeId { get; set; }

    public string? DiagramLayoutSourcePort { get; set; }

    public string? DiagramLayoutTargetPort { get; set; }

    public string? ConditionScript { get; set; }

    public virtual FlowConnectionType ConnectionType { get; set; } = null!;

    public virtual FlowNode SourceNode { get; set; } = null!;

    public virtual FlowNode TargetNode { get; set; } = null!;
}
