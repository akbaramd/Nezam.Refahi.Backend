namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class NodeShapeStyle
{
    public int Id { get; set; }

    public string? Color { get; set; }

    public string? FillColor { get; set; }

    public string? BorderColor { get; set; }

    public float? BorderWidth { get; set; }

    public virtual FlowNode IdNavigation { get; set; } = null!;
}
