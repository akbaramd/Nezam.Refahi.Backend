namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class CourseExecutor
{
    public Guid Id { get; set; }

    public string? Title { get; set; }

    public string? Tel { get; set; }

    public string? Address { get; set; }

    public string? Url { get; set; }

    public string? ContractNumber { get; set; }

    public int ContractDuration { get; set; }

    public string? LicenceNumber { get; set; }

    public DateTime ContractDate { get; set; }

    public DateTime? LicenceDate { get; set; }

    public virtual ICollection<CourseHolding> CourseHoldings { get; set; } = new List<CourseHolding>();

    public virtual ParaUser IdNavigation { get; set; } = null!;
}
