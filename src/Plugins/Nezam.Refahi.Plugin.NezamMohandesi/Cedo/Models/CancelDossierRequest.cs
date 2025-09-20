namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class CancelDossierRequest
{
    public int Id { get; set; }

    public int ConstructionLicenseId { get; set; }

    public DateTime RegDate { get; set; }

    public bool IsOwnerRequest { get; set; }

    public int RequestStatusId { get; set; }

    public virtual ICollection<CancelDossierBill> CancelDossierBills { get; set; } = new List<CancelDossierBill>();

    public virtual ConstructionLicense ConstructionLicense { get; set; } = null!;

    public virtual CancelDossierStatus RequestStatus { get; set; } = null!;
}
