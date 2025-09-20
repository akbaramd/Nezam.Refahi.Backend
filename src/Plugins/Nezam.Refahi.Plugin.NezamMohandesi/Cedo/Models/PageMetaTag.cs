namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class PageMetaTag
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public string? Name { get; set; }

    public string? Content { get; set; }

    public virtual Project Project { get; set; } = null!;
}
