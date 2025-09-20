namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class SeparationPlanRequest
{
    public int Id { get; set; }

    public Guid ServiceRequestId { get; set; }

    public string ReqPlaque { get; set; } = null!;

    public string? ReqUnit { get; set; }

    public string? ReqRegion { get; set; }

    public string Address { get; set; } = null!;

    public string? LicenseNumber { get; set; }

    public DateTime? LicenseDate { get; set; }

    public string? WorkEndNumber { get; set; }

    public DateTime? WorkEndDate { get; set; }

    public double InitialMetrage { get; set; }

    public int InitialRoofs { get; set; }

    public int UnitCount { get; set; }

    public string? CedoNumber { get; set; }

    public int StatusId { get; set; }

    public DateTime RegDate { get; set; }

    public int? MunicipalityId { get; set; }

    public int ScopeRequestTypeId { get; set; }

    public int CityId { get; set; }

    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    public virtual City City { get; set; } = null!;

    public virtual Municipality? Municipality { get; set; }

    public virtual SeparationPlanRequestType ScopeRequestType { get; set; } = null!;

    public virtual ICollection<SeparationExchangeFile> SeparationExchangeFiles { get; set; } = new List<SeparationExchangeFile>();

    public virtual ICollection<SeparationInvolvedMember> SeparationInvolvedMembers { get; set; } = new List<SeparationInvolvedMember>();

    public virtual ICollection<SeparationRequestSurveie> SeparationRequestSurveies { get; set; } = new List<SeparationRequestSurveie>();

    public virtual ServiceRequest ServiceRequest { get; set; } = null!;

    public virtual SeparationPlanRequestStatus Status { get; set; } = null!;
}
