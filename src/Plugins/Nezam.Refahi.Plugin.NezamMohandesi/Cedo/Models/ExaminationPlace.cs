namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ExaminationPlace
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<Examination> Examinations { get; set; } = new List<Examination>();
}
