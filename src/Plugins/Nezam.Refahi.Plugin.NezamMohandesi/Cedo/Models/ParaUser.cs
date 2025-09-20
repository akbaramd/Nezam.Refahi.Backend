namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ParaUser
{
    public Guid Id { get; set; }

    public bool Disabled { get; set; }

    public string? UserName { get; set; }

    public string? NormalizedUserName { get; set; }

    public string? Email { get; set; }

    public string? NormalizedEmail { get; set; }

    public bool EmailConfirmed { get; set; }

    public string? PasswordHash { get; set; }

    public string? SecurityStamp { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public string? PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }

    public bool LockoutEnabled { get; set; }

    public int AccessFailedCount { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime RefreshTokenExpiryTime { get; set; }

    public virtual ICollection<ChangeSiteManagerRequest> ChangeSiteManagerRequests { get; set; } = new List<ChangeSiteManagerRequest>();

    public virtual ICollection<ComplaintMember> ComplaintMembers { get; set; } = new List<ComplaintMember>();

    public virtual ICollection<ConstructionLicenseComment> ConstructionLicenseComments { get; set; } = new List<ConstructionLicenseComment>();

    public virtual CourseExecutor? CourseExecutor { get; set; }

    public virtual ICollection<DashboardGadget> DashboardGadgets { get; set; } = new List<DashboardGadget>();

    public virtual ICollection<DecisionTable> DecisionTables { get; set; } = new List<DecisionTable>();

    public virtual ICollection<DepartmentUser> DepartmentUsers { get; set; } = new List<DepartmentUser>();

    public virtual ICollection<FacilityRequest> FacilityRequests { get; set; } = new List<FacilityRequest>();

    public virtual ICollection<FlowStep> FlowSteps { get; set; } = new List<FlowStep>();

    public virtual GasBranchUser? GasBranchUser { get; set; }

    public virtual ICollection<GasRequestInvolvedMemberChangeLog> GasRequestInvolvedMemberChangeLogs { get; set; } = new List<GasRequestInvolvedMemberChangeLog>();

    public virtual ICollection<InstanceUserGroupMember> InstanceUserGroupMembers { get; set; } = new List<InstanceUserGroupMember>();

    public virtual ICollection<Member> Members { get; set; } = new List<Member>();

    public virtual ICollection<Operator> Operators { get; set; } = new List<Operator>();

    public virtual OrgPersonnel? OrgPersonnel { get; set; }

    public virtual ICollection<Owner> Owners { get; set; } = new List<Owner>();

    public virtual ICollection<ParaUserClaim> ParaUserClaims { get; set; } = new List<ParaUserClaim>();

    public virtual ICollection<ParaUserLogin> ParaUserLogins { get; set; } = new List<ParaUserLogin>();

    public virtual ICollection<ParaUserToken> ParaUserTokens { get; set; } = new List<ParaUserToken>();

    public virtual ICollection<PlanControlSetting> PlanControlSettings { get; set; } = new List<PlanControlSetting>();

    public virtual ICollection<RuleUserSource> RuleUserSources { get; set; } = new List<RuleUserSource>();

    public virtual ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();

    public virtual Signatory? Signatory { get; set; }

    public virtual ICollection<StepOwnerPool> StepOwnerPools { get; set; } = new List<StepOwnerPool>();

    public virtual ICollection<TicketReply> TicketReplies { get; set; } = new List<TicketReply>();

    public virtual ICollection<UserDelegation> UserDelegationDelegateRecievers { get; set; } = new List<UserDelegation>();

    public virtual ICollection<UserDelegation> UserDelegationDelegateSenders { get; set; } = new List<UserDelegation>();

    public virtual UserProfile? UserProfile { get; set; }

    public virtual ICollection<ViolationReportUser> ViolationReportUsers { get; set; } = new List<ViolationReportUser>();

    public virtual WebPushSetting? WebPushSetting { get; set; }

    public virtual ICollection<ParaRole> Roles { get; set; } = new List<ParaRole>();
}
