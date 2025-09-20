namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class GasRequest
{
    public int Id { get; set; }

    public Guid RequestId { get; set; }

    public int TypeId { get; set; }

    public int CityId { get; set; }

    public int ScopeId { get; set; }

    public string Address { get; set; } = null!;

    public string? AddressRegion { get; set; }

    public int OwnershipTypeId { get; set; }

    public string? FlagSerial { get; set; }

    public float LocationLat { get; set; }

    public float LocationLng { get; set; }

    public int StatusId { get; set; }

    public string? TrackCode { get; set; }

    public string? DossierNumber { get; set; }

    public bool IsFreeRequest { get; set; }

    public string? OncallTrackCode { get; set; }

    public string? FreeRequestComment { get; set; }

    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    public virtual City City { get; set; } = null!;

    public virtual ICollection<GasRequestInvolvedMember> GasRequestInvolvedMembers { get; set; } = new List<GasRequestInvolvedMember>();

    public virtual ICollection<GasRequestSplit> GasRequestSplits { get; set; } = new List<GasRequestSplit>();

    public virtual OwnershipDocumentType OwnershipType { get; set; } = null!;

    public virtual ServiceRequest Request { get; set; } = null!;

    public virtual GasRequestScopeType Scope { get; set; } = null!;

    public virtual GasRequestStatus Status { get; set; } = null!;

    public virtual GasRequestType Type { get; set; } = null!;
}
