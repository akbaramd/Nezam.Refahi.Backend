namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class BoundaryEvent
{
    public int Id { get; set; }

    public int BoundarySourceId { get; set; }

    public virtual FlowNode BoundarySource { get; set; } = null!;

    public virtual FlowNode IdNavigation { get; set; } = null!;
}
