namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class WorkingStatus
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public virtual ICollection<PersonelDossier> PersonelDossiers { get; set; } = new List<PersonelDossier>();
}
