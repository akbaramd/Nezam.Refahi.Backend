namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class Setting
{
    public int Id { get; set; }

    public string NewTicketDescription { get; set; } = null!;

    public bool NotifyUserBySms { get; set; }

    public bool NotifyHelpDeskBySms { get; set; }
}
