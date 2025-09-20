namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class MemberContact
{
    public int Id { get; set; }

    public int MemberId { get; set; }

    public int ContactTypeId { get; set; }

    public string Data { get; set; } = null!;

    public virtual ContactInfoType ContactType { get; set; } = null!;

    public virtual Member Member { get; set; } = null!;
}
