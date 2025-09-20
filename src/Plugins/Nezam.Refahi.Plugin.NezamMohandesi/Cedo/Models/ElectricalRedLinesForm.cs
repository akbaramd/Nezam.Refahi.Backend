namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ElectricalRedLinesForm
{
    public int Id { get; set; }

    public int InvolvedMemberId { get; set; }

    public bool Item1 { get; set; }

    public bool Item2 { get; set; }

    public bool Item3 { get; set; }

    public string? Description { get; set; }

    public DateTime RegDate { get; set; }

    public virtual InvolvedMember InvolvedMember { get; set; } = null!;
}
