namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ChangeSupervisorRequestStatus
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<ChangeSupervisorRequest> ChangeSupervisorRequests { get; set; } = new List<ChangeSupervisorRequest>();
}
