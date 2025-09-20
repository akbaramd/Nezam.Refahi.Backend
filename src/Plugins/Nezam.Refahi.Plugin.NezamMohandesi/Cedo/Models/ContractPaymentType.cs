namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ContractPaymentType
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<ContractPaymentDetail> ContractPaymentDetails { get; set; } = new List<ContractPaymentDetail>();
}
