namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class VwCapacityCalculationsB200
{
    public int Id { get; set; }

    public int BuildingGroupSettingId { get; set; }

    public int ServiceTypeId { get; set; }

    public int ServiceFieldId { get; set; }

    public double WorkItem { get; set; }

    public double MetrajRatio { get; set; }

    public int MaxWorkItem { get; set; }

    public int Order { get; set; }

    public string Title { get; set; } = null!;

    public string Expr1 { get; set; } = null!;

    public string Expr2 { get; set; } = null!;
}
