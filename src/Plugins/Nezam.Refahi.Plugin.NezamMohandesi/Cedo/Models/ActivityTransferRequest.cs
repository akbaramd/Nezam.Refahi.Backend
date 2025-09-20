namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ActivityTransferRequest
{
    public int Id { get; set; }

    public int MemberId { get; set; }

    public int? CityId { get; set; }

    public DateTime RequestDate { get; set; }

    public string? Description { get; set; }

    public string? DestinationAddress { get; set; }

    public string? DestinationTell { get; set; }

    public string? Workplace { get; set; }

    public string? WorkplaceTell { get; set; }

    public int StatusId { get; set; }

    public virtual City? City { get; set; }

    public virtual Member Member { get; set; } = null!;

    public virtual ActivityTransferRequestStatus Status { get; set; } = null!;
}
