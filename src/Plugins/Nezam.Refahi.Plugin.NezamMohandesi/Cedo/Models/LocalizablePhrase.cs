namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class LocalizablePhrase
{
    public int Id { get; set; }

    public string Phrase { get; set; } = null!;

    public virtual ICollection<ProjectTranslation> ProjectTranslations { get; set; } = new List<ProjectTranslation>();
}
