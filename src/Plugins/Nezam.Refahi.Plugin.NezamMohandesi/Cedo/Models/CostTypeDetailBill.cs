namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class CostTypeDetailBill
{
    public int Id { get; set; }

    public int CostTypeDetailId { get; set; }

    public int BillTypeId { get; set; }

    public virtual BillType BillType { get; set; } = null!;

    public virtual CostTypeDetail CostTypeDetail { get; set; } = null!;
}
