namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class TerritoryType
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<Territory> Territories { get; set; } = new List<Territory>();
}
