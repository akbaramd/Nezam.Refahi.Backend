namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class InvolvedMember
{
    public int Id { get; set; }

    public int ConstructionLicenseId { get; set; }

    public int MemberServiceId { get; set; }

    public int StatusId { get; set; }

    public DateTime RegDate { get; set; }

    public DateTime? ExpireDate { get; set; }

    public bool IsCoordinator { get; set; }

    public double? WorkItem { get; set; }

    public string? Description { get; set; }

    public bool ModifiedWorkItem { get; set; }

    public double? Metraje { get; set; }

    public int InvolvedMemberTypeId { get; set; }

    public bool IsManualSelect { get; set; }

    public int? QuotaYear { get; set; }

    public virtual ICollection<ArchitectureExecutiveOperationsForm> ArchitectureExecutiveOperationsForms { get; set; } = new List<ArchitectureExecutiveOperationsForm>();

    public virtual ICollection<ArchitecturePlanControlForm> ArchitecturePlanControlForms { get; set; } = new List<ArchitecturePlanControlForm>();

    public virtual ICollection<ArchitectureRedLinesForm> ArchitectureRedLinesForms { get; set; } = new List<ArchitectureRedLinesForm>();

    public virtual ICollection<ArchitectureSafetyForm> ArchitectureSafetyForms { get; set; } = new List<ArchitectureSafetyForm>();

    public virtual ICollection<ArchitectureSpecForm> ArchitectureSpecForms { get; set; } = new List<ArchitectureSpecForm>();

    public virtual ICollection<BuildingWorker> BuildingWorkers { get; set; } = new List<BuildingWorker>();

    public virtual ICollection<ChangeDesignerRequest> ChangeDesignerRequests { get; set; } = new List<ChangeDesignerRequest>();

    public virtual ConstructionLicense ConstructionLicense { get; set; } = null!;

    public virtual ICollection<ElectricalExecutiveOperationsForm> ElectricalExecutiveOperationsForms { get; set; } = new List<ElectricalExecutiveOperationsForm>();

    public virtual ICollection<ElectricalPlanControlForm> ElectricalPlanControlForms { get; set; } = new List<ElectricalPlanControlForm>();

    public virtual ICollection<ElectricalRedLinesForm> ElectricalRedLinesForms { get; set; } = new List<ElectricalRedLinesForm>();

    public virtual ICollection<ElectricalSpecForm> ElectricalSpecForms { get; set; } = new List<ElectricalSpecForm>();

    public virtual ICollection<ElectricitySafetyForm> ElectricitySafetyForms { get; set; } = new List<ElectricitySafetyForm>();

    public virtual InvolvedMemberType InvolvedMemberType { get; set; } = null!;

    public virtual ICollection<ManpowerSkillCardValue> ManpowerSkillCardValues { get; set; } = new List<ManpowerSkillCardValue>();

    public virtual ICollection<MaterialValue> MaterialValues { get; set; } = new List<MaterialValue>();

    public virtual ICollection<MechanicSafetyForm> MechanicSafetyForms { get; set; } = new List<MechanicSafetyForm>();

    public virtual ICollection<MechanicalExecutiveOperationsForm> MechanicalExecutiveOperationsForms { get; set; } = new List<MechanicalExecutiveOperationsForm>();

    public virtual ICollection<MechanicalPlanControlForm> MechanicalPlanControlForms { get; set; } = new List<MechanicalPlanControlForm>();

    public virtual ICollection<MechanicalRedLinesForm> MechanicalRedLinesForms { get; set; } = new List<MechanicalRedLinesForm>();

    public virtual ICollection<MechanicalSpecForm> MechanicalSpecForms { get; set; } = new List<MechanicalSpecForm>();

    public virtual MemberService MemberService { get; set; } = null!;

    public virtual ICollection<PlanControl> PlanControls { get; set; } = new List<PlanControl>();

    public virtual ICollection<SafetyReport> SafetyReports { get; set; } = new List<SafetyReport>();

    public virtual InvolvedMemberStatus Status { get; set; } = null!;

    public virtual ICollection<StructureExecutiveOperationsForm> StructureExecutiveOperationsForms { get; set; } = new List<StructureExecutiveOperationsForm>();

    public virtual ICollection<StructurePlanControlForm> StructurePlanControlForms { get; set; } = new List<StructurePlanControlForm>();

    public virtual ICollection<StructureRedLinesForm> StructureRedLinesForms { get; set; } = new List<StructureRedLinesForm>();

    public virtual ICollection<StructureSafetyForm> StructureSafetyForms { get; set; } = new List<StructureSafetyForm>();

    public virtual ICollection<StructureSpecForm> StructureSpecForms { get; set; } = new List<StructureSpecForm>();

    public virtual ICollection<SupervisionStepForm> SupervisionStepForms { get; set; } = new List<SupervisionStepForm>();

    public virtual ICollection<SupervisionStepPayment> SupervisionStepPayments { get; set; } = new List<SupervisionStepPayment>();

    public virtual ICollection<ViolationReport> ViolationReports { get; set; } = new List<ViolationReport>();
}
