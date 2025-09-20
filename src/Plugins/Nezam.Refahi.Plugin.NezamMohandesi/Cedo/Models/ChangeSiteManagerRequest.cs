namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ChangeSiteManagerRequest
{
    public int Id { get; set; }

    public int ConstructionLicenseId { get; set; }

    public int StatusId { get; set; }

    public string Description { get; set; } = null!;

    public DateTime RegDate { get; set; }

    public Guid RequesterId { get; set; }

    public virtual ConstructionLicense ConstructionLicense { get; set; } = null!;

    public virtual ParaUser Requester { get; set; } = null!;

    public virtual ChangeSiteManagerStatus Status { get; set; } = null!;
}
