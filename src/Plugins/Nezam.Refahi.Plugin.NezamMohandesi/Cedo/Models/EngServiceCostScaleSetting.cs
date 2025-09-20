namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class EngServiceCostScaleSetting
{
    public int Id { get; set; }

    public int DossierTypeId { get; set; }

    public float Scale { get; set; }

    public virtual DossierType DossierType { get; set; } = null!;
}
