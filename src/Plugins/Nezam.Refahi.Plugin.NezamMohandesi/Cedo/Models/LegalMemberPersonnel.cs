namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class LegalMemberPersonnel
{
    public int Id { get; set; }

    public int LegalMemberId { get; set; }

    public int MemberId { get; set; }

    public int? PostId { get; set; }

    public DateTime? Entry { get; set; }

    public DateTime? ExitDate { get; set; }

    public virtual LegalMember LegalMember { get; set; } = null!;

    public virtual Member Member { get; set; } = null!;

    public virtual PersonnelPost? Post { get; set; }
}
