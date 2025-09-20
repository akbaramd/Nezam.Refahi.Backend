namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class FacilityRequestFor
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<FacilityRequest> FacilityRequests { get; set; } = new List<FacilityRequest>();
}
