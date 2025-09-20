namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class FlowEventMode
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<FlowEvent> FlowEvents { get; set; } = new List<FlowEvent>();
}
