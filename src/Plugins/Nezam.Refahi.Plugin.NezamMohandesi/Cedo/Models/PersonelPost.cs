namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class PersonelPost
{
    public int Id { get; set; }

    public Guid PersonelId { get; set; }

    public int OrganizationPostId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public virtual OrganizationPost OrganizationPost { get; set; } = null!;

    public virtual Personel Personel { get; set; } = null!;
}
