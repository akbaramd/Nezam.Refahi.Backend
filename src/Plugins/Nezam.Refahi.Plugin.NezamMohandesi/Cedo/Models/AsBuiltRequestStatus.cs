namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class AsBuiltRequestStatus
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<AsBuiltRequest> AsBuiltRequests { get; set; } = new List<AsBuiltRequest>();
}
