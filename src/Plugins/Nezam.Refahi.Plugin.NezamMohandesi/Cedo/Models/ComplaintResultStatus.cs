namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ComplaintResultStatus
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<ComplaintCommunicated> ComplaintCommunicateds { get; set; } = new List<ComplaintCommunicated>();
}
