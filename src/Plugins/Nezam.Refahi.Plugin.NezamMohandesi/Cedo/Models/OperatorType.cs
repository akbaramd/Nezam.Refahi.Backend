namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class OperatorType
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<Operator> Operators { get; set; } = new List<Operator>();
}
