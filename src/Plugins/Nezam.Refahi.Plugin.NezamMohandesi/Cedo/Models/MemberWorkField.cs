namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class MemberWorkField
{
    public int Id { get; set; }

    public int MemberId { get; set; }

    public int ServiceFieldId { get; set; }

    public virtual Member Member { get; set; } = null!;

    public virtual ServiceField ServiceField { get; set; } = null!;
}
