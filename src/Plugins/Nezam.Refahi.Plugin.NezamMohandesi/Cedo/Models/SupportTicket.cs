namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class SupportTicket
{
    public int Id { get; set; }

    public int DepartmentId { get; set; }

    public int StatusId { get; set; }

    public string Subject { get; set; } = null!;

    public string? TrackCode { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual TicketStatus Status { get; set; } = null!;

    public virtual ICollection<TicketReply> TicketReplies { get; set; } = new List<TicketReply>();
}
