namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class PaymentComeBackRequest
{
    public int Id { get; set; }

    public int BillId { get; set; }

    public int RequestAmount { get; set; }

    public int PayedAmount { get; set; }

    public DateTime RegDate { get; set; }

    public string AccountNumber { get; set; } = null!;

    public string BankName { get; set; } = null!;

    public string? Description { get; set; }

    public int RequestStatusId { get; set; }

    public virtual Bill Bill { get; set; } = null!;

    public virtual ChangeDesignerRequestStatus RequestStatus { get; set; } = null!;
}
