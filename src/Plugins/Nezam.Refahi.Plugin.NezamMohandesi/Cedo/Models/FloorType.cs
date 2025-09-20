namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class FloorType
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<Floor> Floors { get; set; } = new List<Floor>();
}
