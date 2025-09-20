namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class SupervisionCandidateStatus
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<SupervisionCandidate> SupervisionCandidates { get; set; } = new List<SupervisionCandidate>();
}
