namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class LegalMember
{
    public int Id { get; set; }

    public string RegNumber { get; set; } = null!;

    public string Title { get; set; } = null!;

    public DateTime RegDate { get; set; }

    public int LegalMemberTypeId { get; set; }

    public virtual Member IdNavigation { get; set; } = null!;

    public virtual ICollection<LegalMemberPersonnel> LegalMemberPersonnel { get; set; } = new List<LegalMemberPersonnel>();

    public virtual LegalMemberType LegalMemberType { get; set; } = null!;
}
