namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class Signatory
{
    public Guid Id { get; set; }

    public string Position { get; set; } = null!;

    public virtual ICollection<CourseHoldingSignatoriese> CourseHoldingSignatorieses { get; set; } = new List<CourseHoldingSignatoriese>();

    public virtual ParaUser IdNavigation { get; set; } = null!;
}
