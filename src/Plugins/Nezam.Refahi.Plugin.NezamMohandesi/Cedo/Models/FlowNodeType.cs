namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class FlowNodeType
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<FlowNode> FlowNodes { get; set; } = new List<FlowNode>();
}
