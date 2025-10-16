namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class DeletedAttendee
{
    public int MemberId { get; set; }

    public int? StatusId { get; set; }

    public int PaymentStatusId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string MembershipCode { get; set; } = null!;

    public Guid? CourseHoldingId { get; set; }

    public Guid HoldingMemberId { get; set; }

    public decimal Amount { get; set; }
}
