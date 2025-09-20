namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class DocumentIndex
{
    public int HelpId { get; set; }

    public string Title { get; set; } = null!;

    public string? Summary { get; set; }

    public string? Content { get; set; }

    public int Order { get; set; }

    public Guid Id { get; set; }

    public Guid? ParentId { get; set; }

    public virtual ProjectHelp Help { get; set; } = null!;

    public virtual ICollection<DocumentIndex> InverseParent { get; set; } = new List<DocumentIndex>();

    public virtual DocumentIndex? Parent { get; set; }
}
