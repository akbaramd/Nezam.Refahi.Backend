namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class WebService
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public string Url { get; set; } = null!;

    public string ClientName { get; set; } = null!;

    public int WebServiceTypeId { get; set; }

    public DateTime? LastChanged { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual ICollection<WebServiceMethod> WebServiceMethods { get; set; } = new List<WebServiceMethod>();

    public virtual WebServiceType WebServiceType { get; set; } = null!;
}
