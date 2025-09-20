namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class CategoryManpowerSkillCard
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<ManpowerSkillCard> ManpowerSkillCards { get; set; } = new List<ManpowerSkillCard>();
}
