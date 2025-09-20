namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class DepartmentUser
{
    public int Id { get; set; }

    public int DepartmentId { get; set; }

    public Guid UserAccountId { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual ParaUser UserAccount { get; set; } = null!;
}
