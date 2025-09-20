namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class MemberBankAcount
{
    public int Id { get; set; }

    public string? AcountNumber { get; set; }

    public int BankAcountTypeId { get; set; }

    public int MemberId { get; set; }

    public virtual BankAcountType BankAcountType { get; set; } = null!;

    public virtual Member Member { get; set; } = null!;
}
