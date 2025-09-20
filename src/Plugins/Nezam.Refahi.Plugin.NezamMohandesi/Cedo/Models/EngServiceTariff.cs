namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class EngServiceTariff
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public DateTime RegDate { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? ExpireDate { get; set; }

    public virtual ICollection<EngServiceTariffDetail> EngServiceTariffDetails { get; set; } = new List<EngServiceTariffDetail>();
}
