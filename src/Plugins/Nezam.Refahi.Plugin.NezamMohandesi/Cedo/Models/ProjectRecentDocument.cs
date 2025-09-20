namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ProjectRecentDocument
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public int DocType { get; set; }

    public string DocId { get; set; } = null!;

    public int? SubId { get; set; }

    public string DocTitle { get; set; } = null!;

    public DateTime LastOpenedAt { get; set; }

    public virtual Project Project { get; set; } = null!;
}
