namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class Sentence
{
    public Guid Id { get; set; }

    public string PlaintiffName { get; set; } = null!;

    public bool IsLegal { get; set; }

    public int MemberId { get; set; }

    public string Description { get; set; } = null!;

    public int Degree { get; set; }

    public DateTime DateNotification { get; set; }

    public int? DurationInitial { get; set; }

    public DateTime? DateFinalVerdict { get; set; }

    public int? Duration { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime RegDate { get; set; }

    public string JudgmentLetterNumber { get; set; } = null!;

    public DateTime DateSentenceLetter { get; set; }

    public int SentenceStatusId { get; set; }

    public virtual Member Member { get; set; } = null!;

    public virtual SentenceCancellation? SentenceCancellation { get; set; }

    public virtual SentenceStatus SentenceStatus { get; set; } = null!;
}
