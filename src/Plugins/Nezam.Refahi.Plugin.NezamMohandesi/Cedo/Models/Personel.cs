namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class Personel
{
    public Guid Id { get; set; }

    public string PersonelCode { get; set; } = null!;

    public DateTime? EmploymentDate { get; set; }

    public virtual UserProfile IdNavigation { get; set; } = null!;

    public virtual ICollection<PersonelDossier> PersonelDossiers { get; set; } = new List<PersonelDossier>();

    public virtual ICollection<PersonelGroup> PersonelGroups { get; set; } = new List<PersonelGroup>();

    public virtual ICollection<PersonelPost> PersonelPosts { get; set; } = new List<PersonelPost>();
}
