namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class PaymentReceipt
{
    public int Id { get; set; }

    public int BillId { get; set; }

    public decimal Amount { get; set; }

    public DateTime RegDate { get; set; }

    public string? TrackCode { get; set; }

    public int PaymentStatusId { get; set; }

    public string? BillPaymentCode { get; set; }

    public string? PayCode { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string? Token { get; set; }

    public string? HashCardNumber { get; set; }

    public string? Rrn { get; set; }

    public int? InstanceId { get; set; }

    public int? PaymentTypeId { get; set; }

    public string? Comment { get; set; }

    public string? Description { get; set; }

    public virtual Bill Bill { get; set; } = null!;

    public virtual PaymentStatus PaymentStatus { get; set; } = null!;

    public virtual PaymentType? PaymentType { get; set; }
}
