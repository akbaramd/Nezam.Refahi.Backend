namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class Owner
{
    public int Id { get; set; }

    public int ConstructionLicenseId { get; set; }

    public bool IsAgent { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string FatherName { get; set; } = null!;

    public string NationalCode { get; set; } = null!;

    public string IssuePlace { get; set; } = null!;

    public string Job { get; set; } = null!;

    public float SharePercentage { get; set; }

    public string ResidenceAddress { get; set; } = null!;

    public string? ResidencePostalCode { get; set; }

    public string? WorkAddress { get; set; }

    public string? Phone { get; set; }

    public string Mobile { get; set; } = null!;

    public bool IsMainOwner { get; set; }

    public int OwnerTypeId { get; set; }

    public string? OtherOwnerType { get; set; }

    public Guid? UserId { get; set; }

    public string? AttorneyNumber { get; set; }

    public DateTime BirthDate { get; set; }

    public string IdNumber { get; set; } = null!;

    public DateTime? ChangeDate { get; set; }

    public bool IsCahnged { get; set; }

    public bool IsMainPossessor { get; set; }

    public string? SanaCode { get; set; }

    public virtual ICollection<ChangeOwnerRequestDetail> ChangeOwnerRequestDetails { get; set; } = new List<ChangeOwnerRequestDetail>();

    public virtual ConstructionLicense ConstructionLicense { get; set; } = null!;

    public virtual OwnerType OwnerType { get; set; } = null!;

    public virtual ParaUser? User { get; set; }
}
