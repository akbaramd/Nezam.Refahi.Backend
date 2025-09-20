namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class CostTypeOrder
{
    public int Id { get; set; }

    public int Order { get; set; }

    public virtual CostType IdNavigation { get; set; } = null!;
}
