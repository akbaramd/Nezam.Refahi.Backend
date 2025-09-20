namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class TicketStatus
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<SupportTicket> SupportTickets { get; set; } = new List<SupportTicket>();
}
