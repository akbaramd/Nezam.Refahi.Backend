namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ProjectTranslation
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public int CultureId { get; set; }

    public int LocalizablePhraseId { get; set; }

    public string? Translation { get; set; }

    public virtual SupportedCulture Culture { get; set; } = null!;

    public virtual LocalizablePhrase LocalizablePhrase { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;
}
