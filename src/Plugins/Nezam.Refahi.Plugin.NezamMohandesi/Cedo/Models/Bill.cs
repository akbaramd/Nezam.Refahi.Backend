namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class Bill
{
    public int Id { get; set; }

    public int? ConstructionLicenseId { get; set; }

    public int BillTypeId { get; set; }

    public DateTime RegDate { get; set; }

    public string? Description { get; set; }

    public string? BuildingGroup { get; set; }

    public string? FloorCount { get; set; }

    public string? Metraje { get; set; }

    public Guid? CourseHoldingMemberId { get; set; }

    public int? SeparationRequestId { get; set; }

    public int? GasRequestId { get; set; }

    public int? GasRequestOncallId { get; set; }

    public virtual ICollection<BillDetail> BillDetails { get; set; } = new List<BillDetail>();

    public virtual BillType BillType { get; set; } = null!;

    public virtual ConstructionLicense? ConstructionLicense { get; set; }

    public virtual CourseHoldingMember? CourseHoldingMember { get; set; }

    public virtual GasRequest? GasRequest { get; set; }

    public virtual GasRequestOncall? GasRequestOncall { get; set; }

    public virtual ICollection<PaymentComeBackRequest> PaymentComeBackRequests { get; set; } = new List<PaymentComeBackRequest>();

    public virtual ICollection<PaymentReceipt> PaymentReceipts { get; set; } = new List<PaymentReceipt>();

    public virtual SeparationPlanRequest? SeparationRequest { get; set; }
}
