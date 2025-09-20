namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class GasRequestOncallStatus
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<GasRequestOncallDescription> GasRequestOncallDescriptions { get; set; } = new List<GasRequestOncallDescription>();
}
