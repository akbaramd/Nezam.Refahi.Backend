namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class MemberService
{
    public int Id { get; set; }

    public int ActivityLicenseId { get; set; }

    public int ServiceTypeId { get; set; }

    public int? ServiceFieldId { get; set; }

    public int CityId { get; set; }

    public int? GradeId { get; set; }

    public bool IsActive { get; set; }

    public DateTime? AcquisitionDate { get; set; }

    public virtual ActivityLicense ActivityLicense { get; set; } = null!;

    public virtual ICollection<ChangePlanFeasibilityJudgement> ChangePlanFeasibilityJudgements { get; set; } = new List<ChangePlanFeasibilityJudgement>();

    public virtual ICollection<ChangePlanRequest> ChangePlanRequests { get; set; } = new List<ChangePlanRequest>();

    public virtual ICollection<ChangeSupervisorJudgment> ChangeSupervisorJudgments { get; set; } = new List<ChangeSupervisorJudgment>();

    public virtual ICollection<ChangeSupervisorRequest> ChangeSupervisorRequestNewSupervisroMemebrServices { get; set; } = new List<ChangeSupervisorRequest>();

    public virtual ICollection<ChangeSupervisorRequest> ChangeSupervisorRequestSupervisroMemebrServices { get; set; } = new List<ChangeSupervisorRequest>();

    public virtual City City { get; set; } = null!;

    public virtual ICollection<ExecuterContractTerminationJudgment> ExecuterContractTerminationJudgments { get; set; } = new List<ExecuterContractTerminationJudgment>();

    public virtual ICollection<GasRequestInvolvedMember> GasRequestInvolvedMembers { get; set; } = new List<GasRequestInvolvedMember>();

    public virtual EngineeringGrade? Grade { get; set; }

    public virtual ICollection<HoldingLesson> HoldingLessons { get; set; } = new List<HoldingLesson>();

    public virtual ICollection<InvolvedMember> InvolvedMembers { get; set; } = new List<InvolvedMember>();

    public virtual ICollection<MemberCapacityDetail> MemberCapacityDetails { get; set; } = new List<MemberCapacityDetail>();

    public virtual ICollection<SeparationInvolvedMember> SeparationInvolvedMembers { get; set; } = new List<SeparationInvolvedMember>();

    public virtual ServiceField? ServiceField { get; set; }

    public virtual ServiceType ServiceType { get; set; } = null!;

    public virtual ICollection<TeacherLesson> TeacherLessons { get; set; } = new List<TeacherLesson>();
}
