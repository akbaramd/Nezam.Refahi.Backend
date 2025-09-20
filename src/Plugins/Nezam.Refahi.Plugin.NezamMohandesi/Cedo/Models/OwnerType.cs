namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class OwnerType
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<Owner> Owners { get; set; } = new List<Owner>();
}
