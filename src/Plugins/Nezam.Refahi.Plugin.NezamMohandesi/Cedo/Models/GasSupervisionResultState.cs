namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class GasSupervisionResultState
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<GasSupervisionResult> GasSupervisionResults { get; set; } = new List<GasSupervisionResult>();
}
