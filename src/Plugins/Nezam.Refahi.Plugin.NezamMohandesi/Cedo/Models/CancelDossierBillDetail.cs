namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class CancelDossierBillDetail
{
    public int Id { get; set; }

    public int CancelDossierBillId { get; set; }

    public int CostTypeId { get; set; }

    public long ReturnAmount { get; set; }

    public string? Description { get; set; }

    public long PayedAmount { get; set; }

    public virtual CancelDossierBill CancelDossierBill { get; set; } = null!;

    public virtual CostType CostType { get; set; } = null!;
}
