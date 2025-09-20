namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class WebServiceMethodType
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<WebServiceMethod> WebServiceMethods { get; set; } = new List<WebServiceMethod>();
}
