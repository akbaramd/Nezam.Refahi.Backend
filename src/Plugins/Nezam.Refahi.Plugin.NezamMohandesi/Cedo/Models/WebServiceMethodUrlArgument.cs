namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class WebServiceMethodUrlArgument
{
    public int Id { get; set; }

    public int MethodId { get; set; }

    public string Name { get; set; } = null!;

    public string ClrDataType { get; set; } = null!;

    public DateTime? LastChanged { get; set; }

    public virtual WebServiceMethod Method { get; set; } = null!;
}
