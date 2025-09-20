namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ManpowerSkillCard
{
    public int Id { get; set; }

    public int CategoryManpowerSkillCardId { get; set; }

    public string? Title { get; set; }

    public virtual CategoryManpowerSkillCard CategoryManpowerSkillCard { get; set; } = null!;

    public virtual ICollection<ManpowerSkillCardValue> ManpowerSkillCardValues { get; set; } = new List<ManpowerSkillCardValue>();
}
