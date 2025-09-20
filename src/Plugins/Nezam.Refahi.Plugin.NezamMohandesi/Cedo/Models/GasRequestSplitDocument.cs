namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class GasRequestSplitDocument
{
    public int Id { get; set; }

    public int GasRequestSplitId { get; set; }

    public int DocumentTypeId { get; set; }

    public DateTime RegDate { get; set; }

    public string? Description { get; set; }

    public int? SheetNumber { get; set; }

    public virtual DocumentType DocumentType { get; set; } = null!;

    public virtual GasRequestSplit GasRequestSplit { get; set; } = null!;
}
