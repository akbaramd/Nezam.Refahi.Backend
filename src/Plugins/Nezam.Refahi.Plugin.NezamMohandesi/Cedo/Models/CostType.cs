namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class CostType
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<BillDetail> BillDetails { get; set; } = new List<BillDetail>();

    public virtual ICollection<CancelDossierBillDetail> CancelDossierBillDetails { get; set; } = new List<CancelDossierBillDetail>();

    public virtual ICollection<CostTypeDetail> CostTypeDetails { get; set; } = new List<CostTypeDetail>();

    public virtual CostTypeOrder? CostTypeOrder { get; set; }
}
