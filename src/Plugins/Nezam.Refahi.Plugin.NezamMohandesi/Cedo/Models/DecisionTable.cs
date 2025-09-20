namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class DecisionTable
{
    public Guid Id { get; set; }

    public int ProjectId { get; set; }

    public string Name { get; set; } = null!;

    public string? Content { get; set; }

    public DateTime? LastChanged { get; set; }

    public DateTime? ArchiveDateTime { get; set; }

    public string? ArchiveComment { get; set; }

    public Guid? ArchivedById { get; set; }

    public bool IsLocked { get; set; }

    public virtual ParaUser? ArchivedBy { get; set; }

    public virtual Project Project { get; set; } = null!;
}
