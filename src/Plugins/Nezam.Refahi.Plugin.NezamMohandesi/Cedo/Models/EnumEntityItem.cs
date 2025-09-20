namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class EnumEntityItem
{
    public int Id { get; set; }

    public int EntityId { get; set; }

    public int ItemId { get; set; }

    public string? ItemValue { get; set; }

    public string? ItemDisplay { get; set; }

    public virtual DataSourceEntity Entity { get; set; } = null!;
}
