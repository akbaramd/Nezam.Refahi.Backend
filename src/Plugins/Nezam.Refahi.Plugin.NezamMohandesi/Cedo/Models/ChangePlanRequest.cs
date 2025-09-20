namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ChangePlanRequest
{
    public int Id { get; set; }

    public int ConstructionLicenseId { get; set; }

    public int MemberServiceId { get; set; }

    public bool IsOwnerRequest { get; set; }

    public DateTime RegDate { get; set; }

    public int StatusId { get; set; }

    public int? FloorCountFromBase { get; set; }

    public int? FloorCountFromGround { get; set; }

    public double? Metraje { get; set; }

    public bool HasNewLicense { get; set; }

    public bool ChangeArchiteturePlan { get; set; }

    public bool ChangeSrtucturePlan { get; set; }

    public bool ChangeElectricPlan { get; set; }

    public bool ChangeMechanicPlan { get; set; }

    public string? TempDossierNumber { get; set; }

    public bool IsAfterBuildingCertificate { get; set; }

    public string? Description { get; set; }

    public double? FirstMetraje { get; set; }

    public int? FirstFloorCountFromBase { get; set; }

    public int? FirstFloorCountFromGround { get; set; }

    public Guid? RequestId { get; set; }

    public virtual ICollection<ChangePlanFeasibility> ChangePlanFeasibilities { get; set; } = new List<ChangePlanFeasibility>();

    public virtual ConstructionLicense ConstructionLicense { get; set; } = null!;

    public virtual MemberService MemberService { get; set; } = null!;

    public virtual ServiceRequest? Request { get; set; }

    public virtual ChangePlanRequestStatus Status { get; set; } = null!;
}
