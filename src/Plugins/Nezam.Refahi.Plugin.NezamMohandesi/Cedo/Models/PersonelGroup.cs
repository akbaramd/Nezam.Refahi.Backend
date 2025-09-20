namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class PersonelGroup
{
    public int Id { get; set; }

    public Guid PersonelId { get; set; }

    public int OrganizationGroupId { get; set; }

    public virtual OrganizationGroup OrganizationGroup { get; set; } = null!;

    public virtual Personel Personel { get; set; } = null!;
}
