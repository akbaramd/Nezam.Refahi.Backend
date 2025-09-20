namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class LicenseInquery
{
    public int Id { get; set; }

    public int MunicipalityId { get; set; }

    public string Code { get; set; } = null!;

    public string LicenseNumber { get; set; } = null!;

    public string NationalCode { get; set; } = null!;

    public int LicenseInqueryStatusId { get; set; }

    public string? Description { get; set; }

    public int DossierTypeId { get; set; }

    public string OwnerFullName { get; set; } = null!;

    public DateTime? ConfirmDate { get; set; }

    public DateTime? RegDate { get; set; }

    public string? RefNumber { get; set; }

    public bool IsShahrdariService { get; set; }

    public virtual DossierType DossierType { get; set; } = null!;

    public virtual LicenseInqueryStatus LicenseInqueryStatus { get; set; } = null!;

    public virtual Municipality Municipality { get; set; } = null!;
}
