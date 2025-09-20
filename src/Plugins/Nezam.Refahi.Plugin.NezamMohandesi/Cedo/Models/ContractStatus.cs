namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ContractStatus
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}
