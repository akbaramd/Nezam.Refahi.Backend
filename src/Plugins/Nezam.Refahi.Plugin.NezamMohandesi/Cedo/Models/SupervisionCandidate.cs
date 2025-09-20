namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class SupervisionCandidate
{
    public Guid Id { get; set; }

    public Guid SupervisionFieldId { get; set; }

    public int MemberId { get; set; }

    public DateTime RegDate { get; set; }

    public double InitialScore { get; set; }

    public double CloseScore { get; set; }

    public int StatusId { get; set; }

    public string? Description { get; set; }

    public virtual Member Member { get; set; } = null!;

    public virtual SupervisionCandidateStatus Status { get; set; } = null!;

    public virtual SupervisionField SupervisionField { get; set; } = null!;
}
