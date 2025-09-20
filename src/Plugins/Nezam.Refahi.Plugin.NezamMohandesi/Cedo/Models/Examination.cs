namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class Examination
{
    public int Id { get; set; }

    public int ConstructionLicenseId { get; set; }

    public int ExaminationTypeId { get; set; }

    public int ExaminationPlaceId { get; set; }

    public int CeilingCount { get; set; }

    public int ExaminationStatusId { get; set; }

    public string? Description { get; set; }

    public int? ExaminationFileId { get; set; }

    public DateTime RegDate { get; set; }

    public virtual ConstructionLicense ConstructionLicense { get; set; } = null!;

    public virtual ConstructionLicenseDocument? ExaminationFile { get; set; }

    public virtual ExaminationPlace ExaminationPlace { get; set; } = null!;

    public virtual ExaminationStatus ExaminationStatus { get; set; } = null!;

    public virtual ExaminationType ExaminationType { get; set; } = null!;
}
