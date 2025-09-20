namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class SentenceCancellation
{
    public Guid Id { get; set; }

    public string LetterNumber { get; set; } = null!;

    public DateTime LetterDate { get; set; }

    public string Description { get; set; } = null!;

    public DateTime RegDate { get; set; }

    public virtual Sentence IdNavigation { get; set; } = null!;
}
