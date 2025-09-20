namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ContractPaymentDetail
{
    public int Id { get; set; }

    public int ContractId { get; set; }

    public long Amount { get; set; }

    public int PaymentTypeId { get; set; }

    public DateTime PaymentDate { get; set; }

    public string? Description { get; set; }

    public string? BankName { get; set; }

    public string? ChequeNumber { get; set; }

    public int ContractPaymentForId { get; set; }

    public virtual Contract Contract { get; set; } = null!;

    public virtual ContractPaymentFor ContractPaymentFor { get; set; } = null!;

    public virtual ContractPaymentType PaymentType { get; set; } = null!;
}
