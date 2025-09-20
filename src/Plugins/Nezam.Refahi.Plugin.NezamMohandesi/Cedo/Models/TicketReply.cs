namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class TicketReply
{
    public int Id { get; set; }

    public int TicketId { get; set; }

    public Guid? UserId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime? ReplyDate { get; set; }

    public virtual SupportTicket Ticket { get; set; } = null!;

    public virtual ParaUser? User { get; set; }
}
