namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ChangeSupervisorRequest
{
    public int Id { get; set; }

    public int ConstructionLicenseId { get; set; }

    public string? SupervisorDescription { get; set; }

    public string? OwnerDescription { get; set; }

    public bool SupervisorResponsibilityAccept { get; set; }

    public bool OwnerResponsibilityAccept { get; set; }

    public string? EngineeringServiceDescription { get; set; }

    public string? ArbitrationDescription { get; set; }

    public bool IsOwnerPayer { get; set; }

    public bool IsNeedSupervisorConfirm { get; set; }

    public bool IsNeedArbitration { get; set; }

    public string? EngineeringServiceManagerDescription { get; set; }

    public bool HasBuildingProgress { get; set; }

    public double? RemainingMetraje { get; set; }

    public double? RemainingWorkItem { get; set; }

    public double? RemainingCharge { get; set; }

    public double? RemainingCoordinatorCharge { get; set; }

    public int? NewSupervisroMemebrServiceId { get; set; }

    public bool IsAfterBuildingCertificate { get; set; }

    public bool? IsOwnerRequest { get; set; }

    public DateTime RegDate { get; set; }

    public int StatusId { get; set; }

    public int? SupervisroMemebrServiceId { get; set; }

    public string? NewDossierNumber { get; set; }

    public bool AsBuiltRequest { get; set; }

    public double BuildingProgress { get; set; }

    public Guid? RequestId { get; set; }

    public virtual ICollection<ChangeSupervisorJudgment> ChangeSupervisorJudgments { get; set; } = new List<ChangeSupervisorJudgment>();

    public virtual ConstructionLicense ConstructionLicense { get; set; } = null!;

    public virtual MemberService? NewSupervisroMemebrService { get; set; }

    public virtual ServiceRequest? Request { get; set; }

    public virtual ChangeSupervisorRequestStatus Status { get; set; } = null!;

    public virtual MemberService? SupervisroMemebrService { get; set; }
}
