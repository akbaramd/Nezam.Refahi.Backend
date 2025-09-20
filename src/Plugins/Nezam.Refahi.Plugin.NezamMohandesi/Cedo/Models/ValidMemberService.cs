namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ValidMemberService
{
    public int Id { get; set; }

    public int MemberTypeId { get; set; }

    public int MembershipTypeId { get; set; }

    public int? LegalMemberTypeId { get; set; }

    public int MemberFieldId { get; set; }

    public int ServiceTypeId { get; set; }

    public int? ServiceFieldId { get; set; }

    public virtual LegalMemberType? LegalMemberType { get; set; }

    public virtual ServiceField MemberField { get; set; } = null!;

    public virtual MemberType MemberType { get; set; } = null!;

    public virtual MemberShipType MembershipType { get; set; } = null!;

    public virtual ServiceField? ServiceField { get; set; }

    public virtual ServiceType ServiceType { get; set; } = null!;
}
