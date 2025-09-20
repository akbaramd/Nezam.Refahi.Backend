namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class DiagramEntity
{
    public int Id { get; set; }

    public int DiagramId { get; set; }

    public int EntityId { get; set; }

    public int X { get; set; }

    public int Y { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public virtual DataSourceDiagram Diagram { get; set; } = null!;

    public virtual DataSourceEntity Entity { get; set; } = null!;
}
