namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class SgaletterType
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<Sgaletter> Sgaletters { get; set; } = new List<Sgaletter>();
}
