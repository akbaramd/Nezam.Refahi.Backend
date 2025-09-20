namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class GasRequestStatus
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<GasRequest> GasRequests { get; set; } = new List<GasRequest>();
}
