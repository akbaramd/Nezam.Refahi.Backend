namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ConstructionLicenseComment
{
    public int Id { get; set; }

    public int ConstructionLicenseId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime RegDate { get; set; }

    public Guid UserId { get; set; }

    public virtual ConstructionLicense ConstructionLicense { get; set; } = null!;

    public virtual ParaUser User { get; set; } = null!;
}
