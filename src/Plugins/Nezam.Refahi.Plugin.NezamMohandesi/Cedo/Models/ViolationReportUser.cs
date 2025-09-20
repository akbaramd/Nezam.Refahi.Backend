namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ViolationReportUser
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public int CityId { get; set; }

    public virtual City City { get; set; } = null!;

    public virtual ParaUser User { get; set; } = null!;
}
