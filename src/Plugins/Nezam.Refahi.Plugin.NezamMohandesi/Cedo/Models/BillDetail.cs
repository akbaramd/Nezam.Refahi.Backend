namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class BillDetail
{
    public int Id { get; set; }

    public int BillId { get; set; }

    public int CostTypeId { get; set; }

    public decimal Amount { get; set; }

    public decimal Tax { get; set; }

    public virtual Bill Bill { get; set; } = null!;

    public virtual ICollection<BillDetailItem> BillDetailItems { get; set; } = new List<BillDetailItem>();

    public virtual CostType CostType { get; set; } = null!;
}
