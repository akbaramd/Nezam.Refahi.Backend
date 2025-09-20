namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class WebServiceMethod
{
    public int Id { get; set; }

    public int WebServiceId { get; set; }

    public int WebServiceMethodTypeId { get; set; }

    public string Name { get; set; } = null!;

    public string? Path { get; set; }

    public string? ReturnClrType { get; set; }

    public string? BodyClrType { get; set; }

    public DateTime? LastChanged { get; set; }

    public virtual WebService WebService { get; set; } = null!;

    public virtual ICollection<WebServiceMethodRequestHeader> WebServiceMethodRequestHeaders { get; set; } = new List<WebServiceMethodRequestHeader>();

    public virtual ICollection<WebServiceMethodRouteArgument> WebServiceMethodRouteArguments { get; set; } = new List<WebServiceMethodRouteArgument>();

    public virtual WebServiceMethodType WebServiceMethodType { get; set; } = null!;

    public virtual ICollection<WebServiceMethodUrlArgument> WebServiceMethodUrlArguments { get; set; } = new List<WebServiceMethodUrlArgument>();
}
