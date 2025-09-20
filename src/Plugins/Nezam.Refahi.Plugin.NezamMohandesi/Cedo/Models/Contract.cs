namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class Contract
{
    public int Id { get; set; }

    public int ConstructionLicenseId { get; set; }

    public int ContractTypeId { get; set; }

    public DateTime StartDate { get; set; }

    public int Duration { get; set; }

    public long UnitAmount { get; set; }

    public DateTime RegDate { get; set; }

    public string? ContractNumber { get; set; }

    public DateTime? SecretariatDate { get; set; }

    public DateTime? OfficeRegDate { get; set; }

    public string? SecretariatNumber { get; set; }

    public double? Metraje { get; set; }

    public int StatusId { get; set; }

    public virtual ConstructionLicense ConstructionLicense { get; set; } = null!;

    public virtual ICollection<ContractPaymentDetail> ContractPaymentDetails { get; set; } = new List<ContractPaymentDetail>();

    public virtual ContractType ContractType { get; set; } = null!;

    public virtual ExecuterContract? ExecuterContract { get; set; }

    public virtual ContractStatus Status { get; set; } = null!;
}
