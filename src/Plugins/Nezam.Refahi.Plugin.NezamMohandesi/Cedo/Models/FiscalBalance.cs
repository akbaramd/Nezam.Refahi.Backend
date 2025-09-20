namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class FiscalBalance
{
    public int Id { get; set; }

    public int ServiceTypeId { get; set; }

    public DateTime Date { get; set; }

    public int Amount { get; set; }

    public int MemberId { get; set; }

    public int ServiceFieldId { get; set; }

    public virtual Member Member { get; set; } = null!;

    public virtual ServiceField ServiceField { get; set; } = null!;

    public virtual ServiceType ServiceType { get; set; } = null!;
}
