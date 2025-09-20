namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ActivityLicense
{
    public int Id { get; set; }

    public int MemberId { get; set; }

    public int IssuedCityId { get; set; }

    public string LicenseSerial { get; set; } = null!;

    public DateTime IssueDate { get; set; }

    public DateTime? ExtensionDate { get; set; }

    public DateTime ExpireDate { get; set; }

    public virtual City IssuedCity { get; set; } = null!;

    public virtual Member Member { get; set; } = null!;

    public virtual ICollection<MemberService> MemberServices { get; set; } = new List<MemberService>();
}
