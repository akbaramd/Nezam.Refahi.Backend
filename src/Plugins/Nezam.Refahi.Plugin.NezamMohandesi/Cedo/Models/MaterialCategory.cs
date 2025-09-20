namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class MaterialCategory
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();
}
