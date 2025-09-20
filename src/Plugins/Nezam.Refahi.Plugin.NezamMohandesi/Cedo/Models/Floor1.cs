namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class Floor1
{
    public int Id { get; set; }

    public string? DossierNumber { get; set; }

    public int FloorTypeId { get; set; }

    public string Title { get; set; } = null!;

    public double Area { get; set; }

    public int BuildingUnitId { get; set; }

    public int Expr1 { get; set; }
}
