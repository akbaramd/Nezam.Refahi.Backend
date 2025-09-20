namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class Operator
{
    public int Id { get; set; }

    public int OperatorTypeId { get; set; }

    public int CityId { get; set; }

    public Guid UserId { get; set; }

    public bool Active { get; set; }

    public virtual City City { get; set; } = null!;

    public virtual OperatorType OperatorType { get; set; } = null!;

    public virtual ParaUser User { get; set; } = null!;
}
