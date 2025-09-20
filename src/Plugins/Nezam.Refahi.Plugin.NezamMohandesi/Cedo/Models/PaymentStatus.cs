namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class PaymentStatus
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<PaymentReceipt> PaymentReceipts { get; set; } = new List<PaymentReceipt>();
}
