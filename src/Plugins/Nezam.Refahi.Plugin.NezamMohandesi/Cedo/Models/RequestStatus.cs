namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class RequestStatus
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
}
