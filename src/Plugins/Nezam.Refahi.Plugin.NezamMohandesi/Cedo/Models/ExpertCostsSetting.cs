namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ExpertCostsSetting
{
    public int Id { get; set; }

    public int CityId { get; set; }

    public int Clause27ExpertCost { get; set; }

    public int EsSupervisorCost { get; set; }

    public DateTime RegDate { get; set; }

    public bool IsActive { get; set; }

    public DateTime? ExpireDate { get; set; }

    public virtual City City { get; set; } = null!;
}
