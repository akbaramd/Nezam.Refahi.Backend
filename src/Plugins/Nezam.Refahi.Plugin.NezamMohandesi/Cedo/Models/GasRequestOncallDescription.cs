namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class GasRequestOncallDescription
{
    public int Id { get; set; }

    public int GasRequestOncallId { get; set; }

    public int GasRequestOncallStatusId { get; set; }

    public DateTime RegDate { get; set; }

    public string Description { get; set; } = null!;

    public virtual GasRequestOncall GasRequestOncall { get; set; } = null!;

    public virtual GasRequestOncallStatus GasRequestOncallStatus { get; set; } = null!;
}
