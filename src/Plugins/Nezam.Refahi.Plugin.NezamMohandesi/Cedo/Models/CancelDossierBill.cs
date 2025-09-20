namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class CancelDossierBill
{
    public int Id { get; set; }

    public int CancelDossierRequestId { get; set; }

    public DateTime RegDate { get; set; }

    public string? Description { get; set; }

    public string? AccountNumber { get; set; }

    public string? BankName { get; set; }

    public virtual ICollection<CancelDossierBillDetail> CancelDossierBillDetails { get; set; } = new List<CancelDossierBillDetail>();

    public virtual CancelDossierRequest CancelDossierRequest { get; set; } = null!;
}
