namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class NodeAnnotationLayout
{
    public int Id { get; set; }

    public double Width { get; set; }

    public double Height { get; set; }

    public double OffsetX { get; set; }

    public double OffsetY { get; set; }

    public virtual FlowNode IdNavigation { get; set; } = null!;
}
