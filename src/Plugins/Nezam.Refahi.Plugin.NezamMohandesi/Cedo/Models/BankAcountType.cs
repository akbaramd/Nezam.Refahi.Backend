namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class BankAcountType
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<MemberBankAcount> MemberBankAcounts { get; set; } = new List<MemberBankAcount>();
}
