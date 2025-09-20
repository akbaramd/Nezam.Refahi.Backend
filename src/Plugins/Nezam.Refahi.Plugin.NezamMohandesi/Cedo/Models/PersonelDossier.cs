namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class PersonelDossier
{
    public int Id { get; set; }

    public Guid PersonelId { get; set; }

    public int WorkingStatusId { get; set; }

    public DateTime RegDate { get; set; }

    public virtual Personel Personel { get; set; } = null!;

    public virtual WorkingStatus WorkingStatus { get; set; } = null!;
}
