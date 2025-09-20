namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class VwPaymentCallBackReport
{
    public int RequestAmount { get; set; }

    public int PayedAmount { get; set; }

    public DateTime RegDate { get; set; }

    public string AccountNumber { get; set; } = null!;

    public string BankName { get; set; } = null!;

    public string? Description { get; set; }

    public string Title { get; set; } = null!;

    public int RequestStatusId { get; set; }

    public string? DossierSerial { get; set; }
}
