namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class CancelDossierStatus
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<CancelDossierRequest> CancelDossierRequests { get; set; } = new List<CancelDossierRequest>();
}
