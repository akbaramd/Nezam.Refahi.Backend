namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class Member
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public int MemberTypeId { get; set; }

    public string MembershipCode { get; set; } = null!;

    public DateTime RegDate { get; set; }

    public DateTime ExpireDate { get; set; }

    public DateTime? ExtensionDate { get; set; }

    public bool IsActive { get; set; }

    public int? SyncCode { get; set; }

    public int MemberShipTypeId { get; set; }

    public string? FormalCode { get; set; }

    public virtual ICollection<ActivityLicense> ActivityLicenses { get; set; } = new List<ActivityLicense>();

    public virtual ICollection<ActivityTransferRequest> ActivityTransferRequests { get; set; } = new List<ActivityTransferRequest>();

    public virtual ICollection<CourseHoldingMember> CourseHoldingMembers { get; set; } = new List<CourseHoldingMember>();

    public virtual ICollection<EmploymentHistory> EmploymentHistories { get; set; } = new List<EmploymentHistory>();

    public virtual ICollection<FiscalBalance> FiscalBalances { get; set; } = new List<FiscalBalance>();

    public virtual ICollection<IntroductionRequest> IntroductionRequests { get; set; } = new List<IntroductionRequest>();

    public virtual LegalMember? LegalMember { get; set; }

    public virtual ICollection<LegalMemberPersonnel> LegalMemberPersonnel { get; set; } = new List<LegalMemberPersonnel>();

    public virtual ICollection<LicenseCapacity> LicenseCapacities { get; set; } = new List<LicenseCapacity>();

    public virtual ICollection<MemberBankAcount> MemberBankAcounts { get; set; } = new List<MemberBankAcount>();

    public virtual ICollection<MemberCapacity> MemberCapacities { get; set; } = new List<MemberCapacity>();

    public virtual ICollection<MemberContact> MemberContacts { get; set; } = new List<MemberContact>();

    public virtual MemberShipType MemberShipType { get; set; } = null!;

    public virtual MemberType MemberType { get; set; } = null!;

    public virtual ICollection<MemberWorkField> MemberWorkFields { get; set; } = new List<MemberWorkField>();

    public virtual RealMember? RealMember { get; set; }

    public virtual ICollection<Sentence> Sentences { get; set; } = new List<Sentence>();

    public virtual ICollection<StudyRecord> StudyRecords { get; set; } = new List<StudyRecord>();

    public virtual ICollection<SupervisionCandidate> SupervisionCandidates { get; set; } = new List<SupervisionCandidate>();

    public virtual ParaUser User { get; set; } = null!;
}
