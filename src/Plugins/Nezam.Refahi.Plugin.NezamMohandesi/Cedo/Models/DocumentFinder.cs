namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class DocumentFinder
{
    public string? DossierNumber { get; set; }

    public int Id { get; set; }

    public int ConstructionLicenseId { get; set; }

    public int DocumentTypeId { get; set; }

    public string? Description { get; set; }

    public DateTime? RegDate { get; set; }

    public string Title { get; set; } = null!;
}
